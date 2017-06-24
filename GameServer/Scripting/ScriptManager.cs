using Microsoft.CSharp;
using SmartEngine.Core;
using SmartEngine.Network;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SagaBNS.GameServer.Scripting
{
    public class ScriptManager : Singleton<ScriptManager>
    {
        #region Members

        private string path;

        #endregion

        #region Properties

        public Dictionary<ulong, MapObjectScriptHandler> MapObjectScripts { get; } = new Dictionary<ulong, MapObjectScriptHandler>();

        public Dictionary<ushort, NPCScriptHandler> NpcScripts { get; } = new Dictionary<ushort, NPCScriptHandler>();

        #endregion

        #region Methods

        public void LoadScript(string path)
        {
            Logger.Log.Info("Loading uncompiled scripts");
            Dictionary<string, string> dic = new Dictionary<string, string>() { ["CompilerVersion"] = "v4.0" };
            CSharpCodeProvider provider = new CSharpCodeProvider(dic);
            int eventcount = 0;
            this.path = path;
            try
            {
                string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                Assembly newAssembly;
                int tmp;
                if (files.Length > 0)
                {
                    newAssembly = CompileScript(files, provider);
                    if (newAssembly != null)
                    {
                        tmp = LoadAssembly(newAssembly);
                        Logger.Log.Info($"Containing {tmp} Scripts");
                        eventcount += tmp;
                    }
                }
                Logger.Log.Info("Loading compiled scripts....");
                files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                foreach (string i in files)
                {
                    newAssembly = Assembly.UnsafeLoadFrom(Path.GetFullPath(i));
                    if (newAssembly != null)
                    {
                        tmp = LoadAssembly(newAssembly);
                        Logger.Log.Info($"Loading {i}, Containing {tmp} Scripts");
                        eventcount += tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            Logger.Log.Info($"Totally {eventcount} Scripts Added");
        }

        private Assembly CompileScript(string[] Source, CodeDomProvider Provider)
        {
            //ICodeCompiler compiler = Provider.;
            CompilerParameters parms = new CompilerParameters();
            CompilerResults results;

            // Configure parameters
            parms.CompilerOptions = "/target:library /optimize";
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = true;

            //parms.ReferencedAssemblies.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Reference Assemblies\Microsoft\Framework\v3.5\System.Data.DataSetExtensions.dll");
            //parms.ReferencedAssemblies.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Reference Assemblies\Microsoft\Framework\v3.5\System.Core.dll");
            //parms.ReferencedAssemblies.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Reference Assemblies\Microsoft\Framework\v3.5\System.Xml.Linq.dll");
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("System.Core.dll");
            parms.ReferencedAssemblies.Add("SmartEngine.Network.dll");
            parms.ReferencedAssemblies.Add("Common.dll");
            parms.ReferencedAssemblies.Add("GameServer.exe");
            /*foreach (string i in Configuration.Instance.ScriptReference)
            {
                parms.ReferencedAssemblies.Add(i);
            }*/

            // Compile
            results = Provider.CompileAssemblyFromFile(parms, Source);
            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        Logger.Log.Error("Compile Error:" + error.ErrorText);
                        Logger.Log.Error("File:" + error.FileName + ":" + error.Line);
                    }
                }
                return null;
            }

            //get a hold of the actual assembly that was generated
            return results.CompiledAssembly;
        }

        private int LoadAssembly(Assembly newAssembly)
        {
            Module[] newScripts = newAssembly.GetModules();
            int count = 0;
            foreach (Module newScript in newScripts)
            {
                foreach (Type npcType in newScript.GetTypes())
                {
                    try
                    {
                        if (npcType.IsAbstract || npcType.GetCustomAttributes(false).Length > 0)
                        {
                            continue;
                        }

                        if (npcType.IsSubclassOf(typeof(NPCScriptHandler)))
                        {
                            NPCScriptHandler newEvent;
                            try
                            {
                                newEvent = (NPCScriptHandler)Activator.CreateInstance(npcType);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (!NpcScripts.ContainsKey(newEvent.NpcID) && newEvent.NpcID != 0)
                            {
                                NpcScripts.Add(newEvent.NpcID, newEvent);
                            }
                            else
                            {
                                if (newEvent.NpcID != 0)
                                {
                                    Logger.Log.Warn($"NpcID:{newEvent.NpcID} already exists, Class:{npcType.FullName} droped");
                                }
                            }
                        }
                        else if (npcType.IsSubclassOf(typeof(MapObjectScriptHandler)))
                        {
                            MapObjectScriptHandler newEvent;
                            try
                            {
                                newEvent = (MapObjectScriptHandler)Activator.CreateInstance(npcType);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            ulong id = (ulong)newEvent.MapID << 32 | newEvent.ObjectID;
                            if (!MapObjectScripts.ContainsKey(id))
                            {
                                MapObjectScripts.Add(id, newEvent);
                            }
                            else
                            {
                                if (newEvent.ObjectID != 0 && newEvent.MapID != 0)
                                {
                                    Logger.Log.Warn($"MapID:{newEvent.MapID} ObjectID:{newEvent.ObjectID} already exists, Class:{npcType.FullName} droped");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(ex);
                    }
                    count++;
                }
            }
            return count;
        }

        #endregion
    }
}