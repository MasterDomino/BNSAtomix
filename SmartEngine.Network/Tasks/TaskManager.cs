﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using SmartEngine.Core;
using System.Threading;

namespace SmartEngine.Network.Tasks
{
    public class TaskManager : Singleton<TaskManager>
    {
        private readonly HashSet<Task> registered = new HashSet<Task>();
        private readonly ConcurrentQueue<Task> fifo = new ConcurrentQueue<Task>();
        private readonly ConcurrentQueue<Task> slowFifo = new ConcurrentQueue<Task>();
        private readonly AutoResetEvent waiter = new AutoResetEvent(false);
        private readonly AutoResetEvent waiterSlow = new AutoResetEvent(false);
        private readonly List<Thread> threadpool = new List<Thread>();
        private int exeCount;
        private int exeTime;
        private DateTime exeStamp = DateTime.Now, schedulerStamp = DateTime.Now;
        private int schedulerTime;
        private int schedulerCount;
        private readonly Stopwatch watch = new Stopwatch();
        private Task[] tasks = new Task[0];

        /// <summary>
        /// 平均调度器调度时间
        /// </summary>
        public int AverageScheduleTime { get; set; }
        /// <summary>
        /// Task的平均执行时间
        /// </summary>
        public int AverageExecutionTime { get; set; }
        /// <summary>
        /// 总Task数
        /// </summary>
        public int RegisteredCount { get { return registered.Count; } }

        public Dictionary<string, int> TenTasksWithMostInstances
        {
            get
            {
                Dictionary<string, int> tmp = new Dictionary<string, int>();
                Task[] tasks;
                lock (registered)
                {
                    tasks = new Task[registered.Count];
                    registered.CopyTo(tasks);
                }
                foreach (Task task in tasks)
                {
                    if (!tmp.ContainsKey(task.Name))
                    {
                        tmp[task.Name] = 1;
                    }
                    else
                    {
                        tmp[task.Name]++;
                    }
                }
                Dictionary<string, int> res = new Dictionary<string, int>();
                foreach(KeyValuePair<string,int> i in (from t in tmp orderby t.Value descending select t).Take(10))
                {
                    res[i.Key] = i.Value;
                }
                return res;
            }
        }

        /// <summary>
        /// 每分钟的Task执行量
        /// </summary>
        public int ExecutionCountPerMinute { get; set; }
        private Thread main;
        public TaskManager()
        {
            //DefaultValue;
            SetWorkerCount(2, 4);
            Start();
        }

        /// <summary>
        /// 设置Worker线程数量
        /// </summary>
        /// <param name="count">普通Task线程数</param>
        /// <param name="slowCount">执行时间较长的Task线程数</param>
        public void SetWorkerCount(int count, int slowCount)
        {
            foreach (Thread i in threadpool)
            {
                ClientManager.RemoveThread(i.Name);
                i.Abort();
            }
            threadpool.Clear();
            for (int i = 0; i < count; i++)
            {
                Thread thread = new Thread(Worker)
                {
                    Priority = ThreadPriority.Highest
                };
                thread.Name = string.Format("Worker({0})", thread.ManagedThreadId);
                ClientManager.AddThread(thread);
                thread.Start();
                threadpool.Add(thread);
            }
            for (int i = 0; i < slowCount; i++)
            {
                Thread thread = new Thread(WorkerSlow);
                thread.Name = string.Format("WorkerSlow({0})", thread.ManagedThreadId);
                ClientManager.AddThread(thread);
                thread.Start();
                threadpool.Add(thread);
            }
        }

        /// <summary>
        /// 启动任务管理器线程池
        /// </summary>
        public void Start()
        {
            if (main != null)
            {
                ClientManager.RemoveThread(main.Name);
                main.Abort();
            }
            main = new Thread(MainLoop);
            main.Name = string.Format("ThreadPoolMainLoop({0})", main.ManagedThreadId);
            ClientManager.AddThread(main);
            main.Start();
        }

        /// <summary>
        /// 停止任务管理器线程池
        /// </summary>
        public void Stop()
        {
            foreach (Thread i in threadpool)
            {
                ClientManager.RemoveThread(i.Name);
                i.Abort();
            }
            threadpool.Clear();
            if (main != null)
            {
                ClientManager.RemoveThread(main.Name);
                main.Abort();
            }
        }

        /// <summary>
        /// 注册任务，通常不需要调用，直接调用Task.Activate()即可
        /// </summary>
        /// <param name="task">任务</param>
        public void RegisterTask(Task task)
        {
            lock (registered)
            {
                registered.Add(task);
            }
        }

        /// <summary>
        /// 注销任务，通常不需要调用，直接调用Task.Deactivate()即可
        /// </summary>
        /// <param name="task"></param>
        public void RemoveTask(Task task)
        {
            lock (registered)
            {
                registered.Remove(task);
            }
        }

        private void PushTaskes()
        {
            DateTime now = DateTime.Now;
            if ((now - exeStamp).TotalMinutes > 1)
            {
                AverageExecutionTime = exeCount > 0 ? exeTime / exeCount : 0;
                ExecutionCountPerMinute = exeCount;
                Interlocked.Exchange(ref exeCount, 0);
                Interlocked.Exchange(ref exeTime, 0);
                exeStamp = now;
            }
            if ((now - schedulerStamp).TotalMinutes > 1)
            {
                AverageScheduleTime = schedulerCount > 0 ? schedulerTime / schedulerCount : 0;
                Interlocked.Exchange(ref schedulerCount, 0);
                Interlocked.Exchange(ref schedulerTime, 0);
                schedulerStamp = now;
            }
            Interlocked.Increment(ref schedulerCount);
            watch.Restart();
            int length;
            lock (registered)
            {
                int count = registered.Count;
                if (tasks.Length < count)
                {
                    tasks = new Task[count];
                }

                length = count;
                registered.CopyTo(tasks);
            }
            for (int i = 0; i < length; i++)
            {
                Task task = tasks[i];
                try
                {
                    if (!task.executing && now > task.NextUpdateTime)
                    {
                        task.executing = true;
                        task.NextUpdateTime = now.AddMilliseconds(task.Period);
                        task.TaskBeginTime = now;
                        if (task.IsSlowTask)
                        {
                            slowFifo.Enqueue(task);
                            waiterSlow.Set();
                        }
                        else
                        {
                            fifo.Enqueue(task);
                            waiter.Set();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                }
            }
            watch.Stop();
            Interlocked.Add(ref schedulerTime, (int)watch.ElapsedMilliseconds);
        }

        private void MainLoop()
        {
            try
            {
                while (true)
                {
                    PushTaskes();
                    if (registered.Count > 1000)
                    {
                        int waitTime = 10000 / registered.Count;
                        if (waitTime > 10)
                        {
                            waitTime = 10;
                        }

                        if (waitTime == 0)
                        {
                            waitTime = 1;
                        }

                        Thread.Sleep(waitTime);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                ClientManager.RemoveThread(Thread.CurrentThread.Name);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
            ClientManager.RemoveThread(Thread.CurrentThread.Name);
        }

        private void Worker()
        {
            WorkerIntern(fifo, waiter);
        }

        private void WorkerSlow()
        {
            WorkerIntern(slowFifo, waiterSlow);
        }

        private void WorkerIntern(ConcurrentQueue<Task> fifo, AutoResetEvent waiter)
        {
            try
            {
                while (true)
                {
                    while (fifo.TryDequeue(out Task task))
                    {
                        try
                        {
                            task.CallBack();
                            Interlocked.Add(ref exeTime, (int)(DateTime.Now - task.TaskBeginTime).TotalMilliseconds);
                            Interlocked.Increment(ref exeCount);
                            task.executing = false;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Error(ex);
                        }
                    }
                    waiter.WaitOne(5);
                }
            }
            catch (ThreadAbortException)
            {
                ClientManager.RemoveThread(Thread.CurrentThread.Name);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Critical ERROR! Worker terminated unexpected!");
                Logger.Log.Error(ex);
            }
            ClientManager.RemoveThread(Thread.CurrentThread.Name);
        }
    }
}
