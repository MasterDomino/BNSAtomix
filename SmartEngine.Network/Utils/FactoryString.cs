﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartEngine.Core;
using SmartEngine.Network.VirtualFileSystem;

namespace SmartEngine.Network.Utils
{
    public abstract class FactoryString<K, T>
        where K : new()
        where T : new()
    {
        protected Dictionary<string, T> items = new Dictionary<string, T>();
        private FactoryType type;
        protected string loadingTab = string.Empty;
        protected string loadedTab = string.Empty;
        protected string databaseName = string.Empty;

        public Dictionary<string, T> Items { get { return items; } }
        public T this[string id]
        {
            get
            {
                return items[id];
            }
            set
            {
                items[id] = value;
            }
        }
public FactoryType FactoryType { get { return this.type; } set { this.type = value; } }
        private string path;
        private Encoding encoding;
        private bool isFolder;

        public FactoryString()
        {

        }

        protected abstract string GetKey(T item);

        protected abstract void ParseCSV(T item, string[] paras);

        protected abstract void ParseXML(XmlElement root, XmlElement current, T item);

        public void Reload()
        {
            items.Clear();
            Init(path, encoding, isFolder);
        }

        public void Init(string[] files, System.Text.Encoding encoding)
        {
            int count = 0;
            this.encoding = encoding;
            switch (this.type)
            {
                case FactoryType.CSV:
                    foreach (string i in files)
                    {
                        count += InitCSV(i, encoding);
                    }

                    break;
                case FactoryType.XML:
                    foreach (string i in files)
                    {
                        count += InitXML(i, encoding);
                    }

                    break;
                default:
                    throw new Exception(string.Format("No FactoryType set for class:{0}", this.ToString()));
            }
        }

        public void Init(string path, System.Text.Encoding encoding, bool isFolder)
        {
            string[] files = null;
            int count = 0;
            this.path = path;
            this.encoding = encoding;
            this.isFolder = isFolder;
            if (isFolder)
            {
                string pattern = "*.*";
                if (this.FactoryType == FactoryType.CSV)
                {
                    pattern = "*.csv";
                }
                else if (this.FactoryType == FactoryType.XML)
                {
                    pattern = "*.xml";
                }

                files = VirtualFileSystemManager.Instance.FileSystem.SearchFile(path, pattern);
            }
            else
            {
                files = new string[1];
                files[0] = path;
            }
            switch (this.type)
            {
                case FactoryType.CSV:
                    foreach (string i in files)
                    {
                        count += InitCSV(i, encoding);
                    }

                    break;
                case FactoryType.XML:
                    foreach (string i in files)
                    {
                        count += InitXML(i, encoding);
                    }

                    break;
                default:
                    throw new Exception(string.Format("No FactoryType set for class:{0}", this.ToString()));
            }
        }

        public void Init(string path, System.Text.Encoding encoding)
        {
            Init(path, encoding, false);
        }

        private XmlElement FindRoot(XmlDocument doc)
        {
            foreach (object i in doc.ChildNodes)
            {
                if (i.GetType() == typeof(XmlElement))
                {
                    return (XmlElement)i;
                }
            }
            return null;
        }

        private void ParseNode(XmlElement ele, T item)
        {
            XmlNodeList list;
            list = ele.ChildNodes;
            foreach (object j in list)
            {
                XmlElement i;
                if (j.GetType() != typeof(XmlElement))
                {
                    continue;
                }

                i = (XmlElement)j;
                ParseXML(ele, i, item);
                if (i.ChildNodes.Count != 0)
                {
                    ParseNode(i, item);
                }
            }
        }

        private int InitXML(string path, System.Text.Encoding encoding)
        {
            XmlDocument xml = new XmlDocument();
            int count = 0;
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path));
                root = FindRoot(xml);
                list = root.ChildNodes;
                DateTime time = DateTime.Now;
                string label = this.loadingTab;

                foreach (object j in list)
                {
                    T item = new T();
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    i = (XmlElement)j;
                    ParseXML(root, i, item);
                    if (i.ChildNodes.Count != 0)
                    {
                        ParseNode(i, item);
                    }

                    string key = GetKey(item);
                    if (!items.ContainsKey(key))
                    {
                        items.Add(key, item);
                    }

                    if ((DateTime.Now - time).TotalMilliseconds > 10)
                    {
                        time = DateTime.Now;
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
            }
            return count;
        }

        private int InitCSV(string path, System.Text.Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            int count = 0;
            int lines = 0;
            string label = this.loadingTab;
            DateTime time = DateTime.Now;
            while (!sr.EndOfStream)
            {
                string line;
                lines++;
                line = sr.ReadLine();
                string[] paras;
                try
                {
                    T item = new T();
                    if (line.IndexOf('#') != -1)
                    {
                        line = line.Substring(0, line.IndexOf('#'));
                    }

                    if (line == "")
                    {
                        continue;
                    }

                    paras = line.Split(',');
                    if (paras.Length < 2)
                    {
                        continue;
                    }

                    for (int i = 0; i < paras.Length; i++)
                    {
                        if (paras[i] == "")
                        {
                            paras[i] = "0";
                        }
                    }
                    ParseCSV(item, paras);

                    string key = GetKey(item);
                    if (!items.ContainsKey(key))
                    {
                        items.Add(key, item);
                    }

                    if ((DateTime.Now - time).TotalMilliseconds > 10)
                    {
                        time = DateTime.Now;
                    }
                    count++;
                }
                catch (Exception)
                {
                    Logger.Log.Error("Error on parsing " + this.databaseName + " db!\r\n       File:" + path + ":" + lines.ToString() + "\r\n       Content:" + line);
                }
            }
            sr.Close();
            return count;
        }

        /// <summary>
        /// Return an instance of 
        /// </summary>
        public static K Instance
        {
            get { return SingletonHolder.instance; }
            set { SingletonHolder.instance = value; }
        }

        /// <summary>
        /// Sealed class to avoid any heritage from this helper class
        /// </summary>
        private sealed class SingletonHolder
        {
            internal static K instance = new K();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            /// </summary>
            static SingletonHolder()
            {
            }
        }
    }
}
