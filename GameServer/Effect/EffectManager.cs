using System;
using System.Collections.Generic;
using System.Linq;

using SmartEngine.Network.Utils;

namespace SagaBNS.GameServer.Effect
{
    public class Effect
    {
        public uint EffectID { get; set; }
        public uint Cooldown { get; set; }
        public uint Duration { get; set; }
        public Dictionary<int, int> Effects = new Dictionary<int, int>();
    }

    public class EffectManager : Factory<EffectManager, Effect>
    {
        private string[] ids = new string[0];

        public EffectManager()
        {
            loadingTab = "Loading effect template database";
            loadedTab = " effect templates loaded.";
            databaseName = "effect Templates";
            FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(Effect effect)
        {
            return effect.EffectID;
        }

        protected override void ParseCSV(Effect effect, string[] paras)
        {
            throw new NotImplementedException();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, Effect effect)
        {
            switch (root.Name.ToLower())
            {
                case "effect":
                    {
                        switch (current.Name.ToLower())
                        {
                            case "id":
                                effect.EffectID = uint.Parse(current.InnerText);
                                break;
                            case "cooldown":
                                effect.Cooldown = uint.Parse(current.InnerText);
                                break;
                            case "duration":
                                effect.Duration = uint.Parse(current.InnerText);
                                break;
                            case "stats":
                                ids = current.InnerText.Split(',');
                                break;
                            case "statsvalue":
                                {
                                    string[] values = current.InnerText.Split(',');
                                    for (int i = 0; i < ids.Count(); i++)
                                    {
                                        int temp = int.Parse(ids[i]);
                                        if (temp != 0)
                                        {
                                            effect.Effects.Add(temp, int.Parse(values[i]));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
