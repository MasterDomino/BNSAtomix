using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Mono.Math;

namespace Unpacker
{
    public unsafe partial class FrmMain : Form
    {
        private Archive archive;
        private readonly List<string> tblName = new List<string>();
        private readonly Dictionary<string, Table.TableSchema> schemas = new Dictionary<string, Table.TableSchema>();
        private bool isCBT3;
        public FrmMain()
        {
            InitializeComponent();
            InitTblNameV114();
            InitTblSchema();
        }

        private void InitTblNameCBT1()
        {
            isCBT3 = false;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("animset");
            tblName.Add("campfire");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("DesignerPreset");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("dynamicfilter");
            tblName.Add("effect");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("GatherSource");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-social");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("item-recipe");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("NpcAppearance");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npctalkmessage");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-animset");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc-customizebase-cb");
            tblName.Add("pc-customize-cb");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("profession");
            tblName.Add("questlooting");
            tblName.Add("race");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("store");
            tblName.Add("summoned");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("text");
            tblName.Add("user-command");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportSwitch");
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }

        private void InitTblNameCBT2()
        {
            isCBT3 = false;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("animset");
            tblName.Add("AutoTargetParameter");
            tblName.Add("campfire");
            tblName.Add("chat-domain");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("craft-recipe");
            tblName.Add("craft-recipe-step");
            tblName.Add("CreatureAppearance");
            tblName.Add("CustomizingDesignerPreset");
            tblName.Add("CustomizingDetailIcon");
            tblName.Add("CustomizingDetailSlider");
            tblName.Add("CustomizingPreset");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("dynamicfilter");
            tblName.Add("effect");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("game-tip");
            tblName.Add("GatherSource");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-social");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("item-recipe");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("lobby-pc");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npcMoveAnim");
            tblName.Add("npctalkmessage");
            tblName.Add("partymatch");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-animset");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("profession");
            tblName.Add("questlooting");
            tblName.Add("race");
            tblName.Add("reward2");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("skillAttributeRule");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("standIdle");
            tblName.Add("store-by-item");
            tblName.Add("store");
            tblName.Add("summoned");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("text");
            tblName.Add("user-command");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("ZoneCampfire");
            tblName.Add("ZoneConvoy");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportSwitch");
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }

        private void InitTblNameCBT3()
        {
            isCBT3 = true;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("ArenaPortal");
            tblName.Add("AutoTargetParameter");
            tblName.Add("BossNpc");
            tblName.Add("campfire");
            tblName.Add("cave");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("Craft");
            tblName.Add("CraftLevel");
            tblName.Add("craft-recipe");
            tblName.Add("craft-recipe-step");
            tblName.Add("CreatureAppearance");
            tblName.Add("CustomizingDesignerPreset");
            tblName.Add("CustomizingDetailIcon");
            tblName.Add("CustomizingDetailSlider");
            tblName.Add("CustomizingPreset");
            tblName.Add("CustomizingPreview");
            tblName.Add("CustomizingUIMatchParam");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("dungeon");
            tblName.Add("effect");
            tblName.Add("envResponse");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("faction-level");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("game-tip");
            tblName.Add("GatherSource");
            tblName.Add("guildlevel");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-social");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("linkMoveAnim");
            tblName.Add("lobby-pc");
            tblName.Add("map-area");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("npcCombatMoveAnim");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npcMoveAnim");
            tblName.Add("npcResponse");
            tblName.Add("npctalkmessage");
            tblName.Add("partymatch");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("pc-voice");
            tblName.Add("pc-voice-set");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("QuestReward");
            tblName.Add("race");
            tblName.Add("reward");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("skillAttributeRule");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skill-train");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("standIdle");
            tblName.Add("static-chat-channel");
            tblName.Add("store-by-item");
            tblName.Add("store");
            tblName.Add("summoned-appearance");
            tblName.Add("SummonedBeautyShop");
            tblName.Add("summoned");
            tblName.Add("SummonedDesignerPreset");
            tblName.Add("summonedLevel");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("SummonedPreset");
            tblName.Add("SummonedStandIdle");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("text");
            tblName.Add("Title");
            tblName.Add("user-command");
            tblName.Add("virtual-item");
            tblName.Add("weapon-gem-effect");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("ZoneCampfire");
            tblName.Add("ZoneConvoy");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportPosition");
            tblName.Add("ZoneTeleportSwitch");
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }

        private void InitTblNameOBT()
        {
            isCBT3 = true;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("ArenaPortal");
            tblName.Add("AutoTargetParameter");
            tblName.Add("BattleMessage");
            tblName.Add("BossNpc");
            tblName.Add("campfire");
            tblName.Add("cave");
            tblName.Add("chat-channel-option");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("Craft");
            tblName.Add("craft-recipe");
            tblName.Add("craft-recipe-step");
            tblName.Add("CreatureAppearance");
            tblName.Add("CustomizingDesignerPreset");
            tblName.Add("CustomizingDetailIcon");
            tblName.Add("CustomizingDetailSlider");
            tblName.Add("CustomizingPreset");
            tblName.Add("CustomizingPreview");
            tblName.Add("CustomizingUIMatchParam");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("dungeon");
            tblName.Add("effect");
            tblName.Add("envResponse");
            tblName.Add("equip-gem-piece");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("faction-level");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("game-tip");
            tblName.Add("GatherSource");
            tblName.Add("GoodsIcon");
            tblName.Add("guildlevel");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-social");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("linkMoveAnim");
            tblName.Add("lobby-pc");
            tblName.Add("map-area");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("npcCombatMoveAnim");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npcMoveAnim");
            tblName.Add("npcResponse");
            tblName.Add("npctalkmessage");
            tblName.Add("partymatch");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("pc-voice");
            tblName.Add("pc-voice-set");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("QuestReward");
            tblName.Add("race");
            tblName.Add("reward");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("skillAttributeRule");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skill-train");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("standIdle");
            tblName.Add("StateSocial");
            tblName.Add("static-chat-channel");
            tblName.Add("store-by-item");
            tblName.Add("store");
            tblName.Add("summoned-appearance");
            tblName.Add("SummonedBeautyShop");
            tblName.Add("summoned");
            tblName.Add("SummonedDesignerPreset");
            tblName.Add("summonedLevel");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("SummonedPreset");
            tblName.Add("SummonedStandIdle");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("text");
            tblName.Add("Title");
            tblName.Add("user-command");
            tblName.Add("virtual-item");
            tblName.Add("weapon-gem-effect");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("ZoneCampfire");
            tblName.Add("ZoneConvoy");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportPosition");
            tblName.Add("ZoneTeleportSwitch");
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }

        private void InitTblNameV82()
        {
            isCBT3 = true;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("ArenaPortal");
            tblName.Add("AutoTargetParameter");
            tblName.Add("BattleMessage");
            tblName.Add("BossNpc");
            tblName.Add("campfire");
            tblName.Add("cave");
            tblName.Add("chat-channel-option");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("Craft");
            tblName.Add("craft-recipe");
            tblName.Add("craft-recipe-step");
            tblName.Add("CreatureAppearance");
            tblName.Add("CustomizingDesignerPreset");
            tblName.Add("CustomizingDetailIcon");
            tblName.Add("CustomizingDetailSlider");
            tblName.Add("CustomizingPreset");
            tblName.Add("CustomizingPreview");
            tblName.Add("CustomizingUIMatchParam");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("dungeon");
            tblName.Add("effect");
            tblName.Add("envResponse");
            tblName.Add("equip-gem-piece");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("faction-level");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("game-tip");
            tblName.Add("GatherSource");
            tblName.Add("GoodsIcon");
            tblName.Add("guildlevel");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-social");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("linkMoveAnim");
            tblName.Add("LoadingImage");
            tblName.Add("lobby-pc");
            tblName.Add("map-area");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("npcCombatMoveAnim");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npcMoveAnim");
            tblName.Add("npcResponse");
            tblName.Add("npctalkmessage");
            tblName.Add("partymatch");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("pc-voice");
            tblName.Add("pc-voice-set");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("QuestReward");
            tblName.Add("race");
            tblName.Add("reward");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("skillAttributeRule");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skill-train");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("standIdle");
            tblName.Add("StateSocial");
            tblName.Add("static-chat-channel");
            tblName.Add("store-by-item");
            tblName.Add("store");
            tblName.Add("summoned-appearance");
            tblName.Add("SummonedBeautyShop");
            tblName.Add("summoned");
            tblName.Add("SummonedDesignerPreset");
            tblName.Add("summonedLevel");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("SummonedPreset");
            tblName.Add("SummonedStandIdle");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("dummy");
            tblName.Add("dummy2");
            tblName.Add("dummy3");
            tblName.Add("dummy4");
            tblName.Add("text");
            tblName.Add("Title");
            tblName.Add("user-command");
            tblName.Add("virtual-item");
            tblName.Add("weapon-gem-effect");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("ZoneCampfire");
            tblName.Add("ZoneConvoy");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportPosition");
            tblName.Add("ZoneTeleportSwitch");
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }

        private void InitTblNameV114()
        {
            isCBT3 = true;
            tblName.Clear();
            tblName.Add("abnormalCamera");
            tblName.Add("abnormalMoveAnim");
            tblName.Add("ArenaPortal");
            tblName.Add("arena-zone");
            tblName.Add("AutoTargetParameter");
            tblName.Add("BattleMessage");
            tblName.Add("boast");
            tblName.Add("BossNpc");
            tblName.Add("campfire");
            tblName.Add("cave2");
            tblName.Add("cave2-group");
            tblName.Add("cave");
            tblName.Add("chat-channel-option");
            tblName.Add("cinematic");
            tblName.Add("commonAnim");
            tblName.Add("Craft");
            tblName.Add("craft-introduction");
            tblName.Add("craft-recipe");
            tblName.Add("craft-recipe-step");
            tblName.Add("CreatureAppearance");
            tblName.Add("CustomizingDesignerPreset");
            tblName.Add("CustomizingDetailIcon");
            tblName.Add("CustomizingDetailSlider");
            tblName.Add("CustomizingIgnoreParam");
            tblName.Add("CustomizingPreset");
            tblName.Add("CustomizingPreview");
            tblName.Add("CustomizingUIMatchParam");
            tblName.Add("dieAnim");
            tblName.Add("discovery-area");
            tblName.Add("district");
            tblName.Add("duel");
            tblName.Add("duel-grade");
            tblName.Add("dungeon");
            tblName.Add("effect");
            tblName.Add("envResponse");
            tblName.Add("equip-gem-piece");
            tblName.Add("event-skill");
            tblName.Add("exhaustionAnim");
            tblName.Add("expand-inventory");
            tblName.Add("faction");
            tblName.Add("faction-level");
            tblName.Add("fielditem");
            tblName.Add("FielditemDrop");
            tblName.Add("field-item-move-anim");
            tblName.Add("field-zone");
            tblName.Add("filter");
            tblName.Add("game-message");
            tblName.Add("game-tip");
            tblName.Add("GatherSource");
            tblName.Add("GoodsIcon");
            tblName.Add("guildlevel");
            tblName.Add("IconTexture");
            tblName.Add("indicator-idle");
            tblName.Add("indicator-image");
            tblName.Add("indicator-social");
            tblName.Add("item-buy-price");
            tblName.Add("item");
            tblName.Add("ItemPouchMesh");
            tblName.Add("ItemSound");
            tblName.Add("job");
            tblName.Add("jobskillset");
            tblName.Add("key-cap");
            tblName.Add("key-command");
            tblName.Add("level");
            tblName.Add("linkMoveAnim");
            tblName.Add("LoadingImage");
            tblName.Add("lobby-pc");
            tblName.Add("map-area");
            tblName.Add("map-group-1");
            tblName.Add("map-group-2");
            tblName.Add("mapinfo");
            tblName.Add("mapoverlay");
            tblName.Add("mapunit");
            tblName.Add("moveAnim");
            tblName.Add("npcCombatMoveAnim");
            tblName.Add("npc");
            tblName.Add("npcIndicatorMoveAnim");
            tblName.Add("npcMoveAnim");
            tblName.Add("npcResponse");
            tblName.Add("npctalkmessage");
            tblName.Add("partychatchannel");
            tblName.Add("partymatch");
            tblName.Add("passive-effect-move-anim");
            tblName.Add("pc-appearance");
            tblName.Add("pc-cam-dist");
            tblName.Add("pc");
            tblName.Add("pc-race-sex-job");
            tblName.Add("pc-voice");
            tblName.Add("pc-voice-set");
            tblName.Add("petition-faq-list");
            tblName.Add("phantomSword");
            tblName.Add("poseTransit");
            tblName.Add("QuestReward");
            tblName.Add("race");
            tblName.Add("reward");
            tblName.Add("set-item");
            tblName.Add("skillAcquireCondition");
            tblName.Add("skillAttributeRule");
            tblName.Add("SkillCastCondition");
            tblName.Add("skill-combo-2");
            tblName.Add("skill-combo");
            tblName.Add("skillcontext");
            tblName.Add("skill");
            tblName.Add("skill-inheritance");
            tblName.Add("skillshow2");
            tblName.Add("skill-train");
            tblName.Add("skillVariation");
            tblName.Add("social");
            tblName.Add("stance");
            tblName.Add("StanceTransit");
            tblName.Add("standIdle");
            tblName.Add("StateSocial");
            tblName.Add("static-chat-channel");
            tblName.Add("store2");
            tblName.Add("store-by-item");
            tblName.Add("store");
            tblName.Add("summoned-appearance");
            tblName.Add("SummonedBeautyShop");
            tblName.Add("summoned");
            tblName.Add("SummonedDesignerPreset");
            tblName.Add("summonedLevel");
            tblName.Add("SummonedMoveAnim");
            tblName.Add("SummonedPreset");
            tblName.Add("SummonedStandIdle");
            tblName.Add("survey");
            tblName.Add("TalkSocial");
            tblName.Add("teleport");
            tblName.Add("Terrain");
            tblName.Add("text");
            tblName.Add("Title");
            tblName.Add("user-command");
            tblName.Add("virtual-item");
            tblName.Add("weapon-gem-effect");
            tblName.Add("ZoneArea");
            tblName.Add("ZoneBaseCamp");
            tblName.Add("ZoneCampfire");
            tblName.Add("ZoneConvoy");
            tblName.Add("zone");
            tblName.Add("ZoneEnv");
            tblName.Add("ZoneGatherSource");
            tblName.Add("ZonePathway");
            tblName.Add("ZonePcSpawn");
            tblName.Add("ZoneRespawn");
            tblName.Add("ZoneTeleportPosition");
            tblName.Add("ZoneTeleportSwitch"); 
            listBox2.Items.Clear();
            foreach (string i in tblName)
            {
                listBox2.Items.Add(i);
            }
        }


        private void InitTblSchema()
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root;
            XmlNodeList list;
            xml.Load("TableSchema.xml");
            root = xml["TableSchema"];
            list = root.ChildNodes;
            foreach (object j in list)
            {
                XmlElement i;
                if (j.GetType() != typeof(XmlElement))
                {
                    continue;
                }

                i = (XmlElement)j;
                switch (i.Name.ToLower())
                {
                    case "table":
                        Table.TableSchema schema = new Table.TableSchema();
                        foreach (object l in i.ChildNodes)
                        {
                            XmlElement k;
                            if (l.GetType() != typeof(XmlElement))
                            {
                                continue;
                            }

                            k = (XmlElement)l;
                            switch (k.Name.ToLower())
                            {
                                case "name":
                                    schema.Name = k.InnerText;
                                    break;
                                case "elementname":
                                    schema.ElementName = k.InnerText;
                                    break;
                                case "field":
                                    Table.Field field = new Table.Field()
                                    {
                                        Name = k.Attributes["name"].Value,
                                        Type = GetFieldType(k.Attributes["type"].Value)
                                    };
                                    if (k.HasAttribute("size"))
                                    {
                                        field.Size = int.Parse(k.Attributes["size"].Value);
                                    }

                                    if (k.HasAttribute("subtype"))
                                    {
                                        field.SubType = GetFieldType(k.Attributes["subtype"].Value);
                                    }
                                    if (k.HasAttribute("export"))
                                    {
                                        field.Export = bool.Parse(k.Attributes["export"].Value);
                                    }

                                    schema.Fields.Add(field.Name, field);
                                    break;
                            }
                        }
                        schemas.Add(schema.Name, schema);
                        break;
                }
            }
            xml = null;
            foreach (string i in schemas.Keys)
            {
                listBox1.Items.Add(i);
            }
        }

        private Table.FieldType GetFieldType(string name)
        {
            switch (name.ToLower())
            {
                case "long":
                    return Table.FieldType.Long;
                case "int":
                    return Table.FieldType.Integer;
                case "string":
                    return Table.FieldType.String;
                case "sizedstring":
                    return Table.FieldType.SizedString;
                case "array":
                    return Table.FieldType.Array;
                case "short":
                    return Table.FieldType.Short;
                case "byte":
                    return Table.FieldType.Byte;
                default:
                    throw new NotSupportedException();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.dat|*.dat";
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                archive = new Archive();
                archive.OpenFile(OD.FileName);
                archive.ExtractAllFiles(System.IO.Path.GetFileNameWithoutExtension(OD.FileName),BetaUnpack.Checked);
                archive.Close();
                MessageBox.Show("Finished");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FD.SelectedPath = Environment.CurrentDirectory;
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Archive archive = new Archive();
                archive.RepackFolder(FD.SelectedPath, BetaUnpack.Checked);
                MessageBox.Show("Finished");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExportTable(false);
        }

        private string GetString(System.IO.BinaryReader br, int offset)
        {
            long ori = br.BaseStream.Position;
            string res;
            br.BaseStream.Position = offset;
            short c;
            int size = 0;
            do
            {
                c = br.ReadInt16();
                size++;
            } while (c != 0);

            br.BaseStream.Position = offset;
            res = Encoding.Unicode.GetString(br.ReadBytes(size * 2)).Trim('\0');
            br.BaseStream.Position = ori;
            return res;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.bin|*.bin";
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileStream fs = new System.IO.FileStream(OD.FileName, System.IO.FileMode.Open);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                OD.Filter = "*.xml|*.xml";
                if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!System.IO.Directory.Exists("datafile"))
                    {
                        System.IO.Directory.CreateDirectory("datafile");
                    }

                    string xmlPath = OD.FileName;
                    string tblName = System.IO.Path.GetFileNameWithoutExtension(OD.FileName);
                    if (schemas.ContainsKey(tblName))
                    {
                        System.IO.FileStream fs2 = new System.IO.FileStream("datafile\\datafile.bin", System.IO.FileMode.Create);
                        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs2);

                        int tblCount, offset;
                        fs.Position = 21;
                        tblCount = br.ReadInt32();
                        offset = br.ReadInt32() + 99;
                        fs.Position = 0;
                        CopyStream(fs, fs2, offset);
                        fs.Position = offset;

                        for (int i = 0; i < tblCount; i++)
                        {
                            long begin = fs.Position;
                            Table.Table tbl = new Table.Table()
                            {
                                Name = this.tblName[i],
                                Unknown1 = br.ReadByte(),
                                Unknown2 = br.ReadByte()
                            };
                            if (isCBT3)
                            {
                                tbl.Unknown3 = br.ReadByte();
                            }

                            tbl.Size = br.ReadInt32();
                            tbl.EntryCount = br.ReadInt32();
                            tbl.EntrySize = br.ReadInt32();
                            tbl.DataSize = br.ReadInt32();
                            if(isCBT3)
                            {
                                tbl.Unknown4 = br.ReadByte();
                            }
                            else
                            {
                                tbl.Unknown3 = br.ReadByte();
                            }

                            tbl.EntryOffset = (int)fs.Position;
                            fs.Position += tbl.EntrySize;
                            tbl.DataOffset = (int)fs.Position;
                            fs.Position = tbl.EntryOffset;
                            if (tbl.Name == tblName)
                            {
                                Table.TableSchema schema = schemas[tbl.Name];
                                BuildTable(xmlPath, schema, out System.IO.MemoryStream ms);
                                ms.Position = 0;
                                CopyStream(ms, fs2, (int)ms.Length);
                            }
                            else
                            {
                                fs.Position = begin;
                                if (isCBT3)
                                {
                                    CopyStream(fs, fs2, tbl.Size + 7);
                                }
                                else
                                {
                                    CopyStream(fs, fs2, tbl.Size + 6);
                                }
                            }
                            if(isCBT3)
                            {
                                fs.Position = begin + tbl.Size + 7;
                            }
                            else
                            {
                                fs.Position = begin + tbl.Size + 6;
                            }
                        }
                        fs2.Close();
                    }
                    else
                    {
                        MessageBox.Show("No schema defined for table:" + tblName);
                    }
                }
                fs.Close();
            }
        }

        private void BuildTable(string xmlPath, Table.TableSchema schema, out System.IO.MemoryStream buf)
        {
            buf = new System.IO.MemoryStream();
            System.IO.MemoryStream entries = new System.IO.MemoryStream();
            System.IO.MemoryStream data = new System.IO.MemoryStream();
            System.IO.BinaryWriter bwEntry = new System.IO.BinaryWriter(entries);
            System.IO.BinaryWriter bwData = new System.IO.BinaryWriter(data);
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);
            XmlElement root = xml[schema.Name];
            Table.Table tbl = new Table.Table()
            {
                Unknown1 = byte.Parse(root.Attributes["unknown1"].Value),
                Unknown2 = byte.Parse(root.Attributes["unknown2"].Value),
                Unknown3 = byte.Parse(root.Attributes["unknown3"].Value)
            };
            if (isCBT3)
            {
                tbl.Unknown4 = byte.Parse(root.Attributes["unknown4"].Value);
            }

            XmlNodeList list = root.ChildNodes;
            tbl.EntryCount = list.Count;
            foreach (object j in list)
            {
                XmlElement i;
                if (j.GetType() != typeof(XmlElement))
                {
                    continue;
                }

                i = (XmlElement)j;
                if (i.Name.ToLower() == schema.ElementName.ToLower())
                {
                    XmlNodeList children = i.ChildNodes;
                    long begin = entries.Position;
                    int unknown = 0, id = 0, id2 = 0;
                    short unknown2 = 0, unknown3 = 0;
                    entries.Position += 20;
                    foreach (object l in children)
                    {
                        XmlElement k;
                        if (l.GetType() != typeof(XmlElement))
                        {
                            continue;
                        }

                        k = (XmlElement)l;
                        switch (k.Name)
                        {
                            case "Unknown1":
                                unknown = int.Parse(k.InnerText);
                                break;
                            case "Unknown2":
                                unknown2 = short.Parse(k.InnerText);
                                break;
                            case "Unknown3":
                                unknown3 = short.Parse(k.InnerText);
                                break;
                            case "ID":
                                id = int.Parse(k.InnerText);
                                break;
                            case "ID2":
                                id2 = int.Parse(k.InnerText);
                                break;
                            default:
                                Table.Field field = schema.Fields[k.Name];
                                switch (field.Type)
                                {
                                    case Table.FieldType.Long:
                                        bwEntry.Write(long.Parse(k.InnerText));
                                        break;
                                    case Table.FieldType.Integer:
                                        bwEntry.Write(int.Parse(k.InnerText));
                                        break;
                                    case Table.FieldType.Short:
                                        bwEntry.Write(short.Parse(k.InnerText));
                                        break;
                                    case Table.FieldType.Byte:
                                        bwEntry.Write(byte.Parse(k.InnerText));
                                        break;
                                    case Table.FieldType.Array:
                                        {
                                            switch (field.SubType)
                                            {
                                                case Table.FieldType.Byte:
                                                    {
                                                        /*
                                                        byte[] tmp = Conversions.HexStr2Bytes(k.InnerText);
                                                        bwEntry.Write(tmp);
                                                        */
                                                        foreach (string t in k.InnerText.Split(','))
                                                        {
                                                            bwEntry.Write(byte.Parse(t));
                                                        }
                                                    }
                                                    break;
                                                case Table.FieldType.Short:
                                                    {
                                                        foreach (string t in k.InnerText.Split(','))
                                                        {
                                                            bwEntry.Write(short.Parse(t));
                                                        }
                                                    }
                                                    break;
                                                case Table.FieldType.Integer:
                                                    {
                                                        foreach (string t in k.InnerText.Split(','))
                                                        {
                                                            bwEntry.Write(int.Parse(t));
                                                        }
                                                    }
                                                    break;
                                                case Table.FieldType.Long:
                                                    {
                                                        foreach (string t in k.InnerText.Split(','))
                                                        {
                                                            bwEntry.Write(long.Parse(t));
                                                        }
                                                    }
                                                    break;
                                                case Table.FieldType.String:
                                                    {
                                                        foreach (string t in k.InnerText.Split('|'))
                                                        {
                                                            byte[] buffer = Encoding.Unicode.GetBytes(t + "\0");
                                                            bwEntry.Write((int)data.Position);
                                                            bwData.Write(buffer);
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case Table.FieldType.String:
                                        {
                                            byte[] buffer = Encoding.Unicode.GetBytes(k.InnerText + "\0");
                                            bwEntry.Write((int)data.Position);
                                            bwData.Write(buffer);
                                        }
                                        break;
                                    case Table.FieldType.SizedString:
                                        {
                                            byte[] buffer = Encoding.Unicode.GetBytes(k.InnerText + "\0");
                                            bwEntry.Write(buffer.Length);
                                            bwEntry.Write((int)data.Position);
                                            bwData.Write(buffer);
                                        }
                                        break;
                                }
                                break;
                        }
                    }

                    long end = entries.Position;
                    entries.Position = begin;
                    bwEntry.Write(unknown);
                    bwEntry.Write(unknown2);
                    bwEntry.Write(unknown3);
                    bwEntry.Write((int)(end - begin - 4));
                    bwEntry.Write(id);
                    bwEntry.Write(id2);
                    entries.Position = end;
                }
            }

            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(buf);
            bw.Write(tbl.Unknown1);
            bw.Write(tbl.Unknown2);
            if (isCBT3)
            {
                bw.Write(tbl.Unknown3);
            }

            bw.Write((int)(entries.Length + data.Length + 13));
            bw.Write(tbl.EntryCount);
            bw.Write((int)entries.Length);
            bw.Write((int)data.Length);
            if(isCBT3)
            {
                bw.Write(tbl.Unknown4);
            }
            else
            {
                bw.Write(tbl.Unknown3);
            }

            entries.Position = 0;
            CopyStream(entries, buf, (int)entries.Length);
            data.Position = 0;
            CopyStream(data, buf, (int)data.Length);
        }

        private void CopyStream(System.IO.Stream s1, System.IO.Stream s2, int size)
        {
            int count = size / 4096;
            int rest = size % 4096;
            byte[] buf = new byte[4096];

            for (int i = 0; i < count; i++)
            {
                s1.Read(buf, 0, 4096);
                s2.Write(buf, 0, 4096);
            }
            if (rest > 0)
            {
                s1.Read(buf, 0, rest);
                s2.Write(buf, 0, rest);
            }
        }

        private void ExportTable(bool export)
        {
            OD.Filter = "*.bin|*.bin";
            List<Table.Table> tbls = new List<Table.Table>();
            if (!System.IO.Directory.Exists("datafile"))
            {
                System.IO.Directory.CreateDirectory("datafile");
            }

            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileStream fs = new System.IO.FileStream(OD.FileName, System.IO.FileMode.Open);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                int tblCount, offset;
                fs.Position = 21;
                tblCount = br.ReadInt32();
                offset = br.ReadInt32() + 99;

                fs.Position = offset;
                for (int i = 0; i < tblCount; i++)
                {
                    long begin = fs.Position;
                    Table.Table tbl = new Table.Table()
                    {
                        Name = tblName[i],
                        Unknown1 = br.ReadByte(),
                        Unknown2 = br.ReadByte()
                    };
                    if (isCBT3)
                    {
                        tbl.Unknown3 = br.ReadByte();
                    }

                    tbl.Size = br.ReadInt32();
                    tbl.EntryCount = br.ReadInt32();
                    tbl.EntrySize = br.ReadInt32();
                    tbl.DataSize = br.ReadInt32();
                    if (isCBT3)
                    {
                        tbl.Unknown4 = br.ReadByte();
                    }
                    else
                    {
                        tbl.Unknown3 = br.ReadByte();
                    }

                    tbl.EntryOffset = (int)fs.Position;
                    fs.Position += tbl.EntrySize;
                    tbl.DataOffset = (int)fs.Position;
                    fs.Position = tbl.EntryOffset;
                    if (schemas.ContainsKey(tbl.Name) && listBox1.SelectedItems.Contains(tbl.Name))
                    {
                        Table.TableSchema schema = schemas[tbl.Name];
                        XmlDocument xml = new XmlDocument();
                        XmlElement root = xml.CreateElement(tbl.Name);
                        if (!export)
                        {
                            root.SetAttribute("unknown1", tbl.Unknown1.ToString());
                            root.SetAttribute("unknown2", tbl.Unknown2.ToString());
                            root.SetAttribute("unknown3", tbl.Unknown3.ToString());
                            if (isCBT3)
                            {
                                root.SetAttribute("unknown4", tbl.Unknown4.ToString());
                            }
                        }
                        for (int j = 0; j < tbl.EntryCount && fs.Position < tbl.DataOffset; j++)
                        {
                            long entryBegin = fs.Position;
                            Table.Entry entry = new Table.Entry()
                            {
                                Unknown1 = br.ReadInt32(),
                                Unknown2 = br.ReadInt16(),
                                Unknown3 = br.ReadInt16(),
                                Size = br.ReadInt32(),
                                Index = br.ReadInt32(),
                                Index2 = br.ReadInt32()
                            };
                            XmlElement child = xml.CreateElement(schema.ElementName);
                            XmlElement ele = null;
                            if (!export)
                            {
                                ele = xml.CreateElement("Unknown1");
                                ele.InnerText = entry.Unknown1.ToString();
                                child.AppendChild(ele);
                                ele = xml.CreateElement("Unknown2");
                                ele.InnerText = entry.Unknown2.ToString();
                                child.AppendChild(ele);
                                ele = xml.CreateElement("Unknown3");
                                ele.InnerText = entry.Unknown3.ToString();
                                child.AppendChild(ele);
                            }
                            ele = xml.CreateElement("ID");
                            ele.InnerText = entry.Index.ToString();
                            child.AppendChild(ele);
                            if (!export)
                            {
                                ele = xml.CreateElement("ID2");
                                ele.InnerText = entry.Index2.ToString();
                                child.AppendChild(ele);
                            }

                            foreach (Table.Field k in schema.Fields.Values)
                            {
                                XmlElement field = xml.CreateElement(k.Name);
                                switch (k.Type)
                                {
                                    case Table.FieldType.Long:
                                        field.InnerText = br.ReadInt64().ToString();
                                        break;
                                    case Table.FieldType.Integer:
                                        field.InnerText = br.ReadInt32().ToString();
                                        break;
                                    case Table.FieldType.Short:
                                        field.InnerText = br.ReadInt16().ToString();
                                        break;
                                    case Table.FieldType.Byte:
                                        field.InnerText = br.ReadByte().ToString();
                                        break;
                                    case Table.FieldType.String:
                                        field.InnerText = GetString(br, tbl.DataOffset + br.ReadInt32());
                                        break;
                                    case Table.FieldType.SizedString:
                                        {
                                            int size = br.ReadInt32();
                                            field.InnerText = GetString(br, tbl.DataOffset + br.ReadInt32());
                                            int rSize = (field.InnerText.Length + 1) * 2;
                                            if (rSize != size)
                                            {
                                                throw new ArgumentException(string.Format("Size mismatch!({0}/{1})", rSize, size));
                                            }
                                        }
                                        break;
                                    case Table.FieldType.Array:
                                        {
                                            switch (k.SubType)
                                            {
                                                case Table.FieldType.Byte:
                                                    {
                                                        /*
                                                        byte[] buf = br.ReadBytes(k.Size);
                                                        field.InnerText = Conversions.bytes2HexString(buf);
                                                         */
                                                        for (int idx = 0; idx < k.Size; idx++)
                                                        {
                                                            field.InnerText += br.ReadByte() + ",";
                                                        }

                                                        field.InnerText = field.InnerText.Substring(0, field.InnerText.Length - 1);
                                                    }
                                                    break;
                                                case Table.FieldType.Short:
                                                    {
                                                        for (int idx = 0; idx < k.Size; idx++)
                                                        {
                                                            field.InnerText += br.ReadInt16() + ",";
                                                        }

                                                        field.InnerText = field.InnerText.Substring(0, field.InnerText.Length - 1);
                                                    }
                                                    break;
                                                case Table.FieldType.Integer:
                                                    {
                                                        for (int idx = 0; idx < k.Size; idx++)
                                                        {
                                                            field.InnerText += br.ReadInt32() + ",";
                                                        }

                                                        field.InnerText = field.InnerText.Substring(0, field.InnerText.Length - 1);
                                                    }
                                                    break;
                                                case Table.FieldType.Long:
                                                    {
                                                        for (int idx = 0; idx < k.Size; idx++)
                                                        {
                                                            field.InnerText += br.ReadInt64() + ",";
                                                        }

                                                        field.InnerText = field.InnerText.Substring(0, field.InnerText.Length - 1);
                                                    }
                                                    break;
                                                case Table.FieldType.String:
                                                    {
                                                        for (int idx = 0; idx < k.Size; idx++)
                                                        {
                                                            field.InnerText += GetString(br, tbl.DataOffset + br.ReadInt32()) + "|";
                                                        }

                                                        field.InnerText = field.InnerText.Substring(0, field.InnerText.Length - 1);
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                }
                                if (!export || k.Export)
                                {
                                    child.AppendChild(field);
                                }
                            }
                            root.AppendChild(child);
                            fs.Position = entryBegin + entry.Size + 4;
                        }
                        xml.AppendChild(root);
                        xml.Save("datafile\\" + tbl.Name + ".xml");
                        xml = null;
                    }
                    if (isCBT3)
                    {
                        fs.Position = begin + tbl.Size + 7;
                    }
                    else
                    {
                        fs.Position = begin + tbl.Size + 6;
                    }
                }
                fs.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExportTable(true);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            int addr = Convert.ToInt32(tb_addr.Text.Replace("0x", ""), 16);
            const uint NORMAL_PRIORITY_CLASS = 0x0020;
            OD.Filter = "*.exe|*.exe";
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                /*bool retValue;
                string Application = OD.FileName;
                string CommandLine = @" /LaunchByLauncher";
                /*Win32API.PROCESS_INFORMATION pInfo = new Win32API.PROCESS_INFORMATION();
                Win32API.STARTUPINFO sInfo = new Win32API.STARTUPINFO();
                
                Win32API.SECURITY_ATTRIBUTES pSec = new Win32API.SECURITY_ATTRIBUTES();
                Win32API.SECURITY_ATTRIBUTES tSec = new Win32API.SECURITY_ATTRIBUTES();
                pSec.nLength = Marshal.SizeOf(pSec);
                tSec.nLength = Marshal.SizeOf(tSec);

                //Open Notepad
                retValue = Win32API.CreateProcess(Application, CommandLine,
                    ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
                    IntPtr.Zero, System.IO.Path.GetDirectoryName(OD.FileName), ref sInfo, out pInfo);*/
                /*System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(Application, CommandLine);
                info.UseShellExecute = true;
                
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);
                
                IntPtr process = Win32API.OpenProcess(Win32API.ProcessAccessFlags.VMWrite, false, (int)p.Handle);
                byte[] buffer = new byte[4];
                int readed;
                Win32API.ReadProcessMemory(process, new IntPtr(addr + 44), buffer, 4, out readed);*/

                int offset = 0x400000;
                System.IO.FileStream fs = new System.IO.FileStream(OD.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(string.Format("0x{0:X}.txt", addr));
                int v9Addr = addr - offset + 44;
                fs.Position = v9Addr;
                int v8Addr = v9Addr + 8;
                fs.Position = v8Addr;
                int v8 = br.ReadInt32();
                v8Addr = v8 - offset;
                fs.Position = v8Addr;
                int typeAddr = br.ReadInt32();
                short count = br.ReadInt16();
                sw.WriteLine(string.Format("Total Column:{0}", count));
                for (int i = 0; i < count; i++)
                {
                    int colAddr = typeAddr + 60 * i;
                    fs.Position = colAddr - offset;
                    Table.FieldType type;
                    byte sCount;
                    fs.Position += 4;
                    type = (Table.FieldType)br.ReadByte();//4
                    byte u1 = br.ReadByte();//5
                    sCount = br.ReadByte();//6
                    byte u2 = br.ReadByte();//7
                    fs.Position += 4;
                    byte u3 = br.ReadByte();//12
                    fs.Position += 8;
                    int u4 = br.ReadInt32();//21
                    if (isCBT3)
                    {
                        fs.Position += 20;
                        bool hide = br.ReadBoolean();//45
                        short u5 = br.ReadInt16();//45
                        short off = br.ReadInt16();//48
                        short len = br.ReadInt16();//50
                        sw.WriteLine(string.Format("Idx:{0} Type:{1} Count:{2} Offset:{9} Length:{10} Hide:{3} U1:{4} U2:{5} U3:{6} U4:{7} U5:{8}", i, type, sCount, hide, u1, u2, u3, u4, u5, off, len));
                    }
                    else
                    {
                        fs.Position += 19;
                        bool hide = br.ReadBoolean();//44
                        byte u5 = br.ReadByte();//45
                        short off = br.ReadInt16();//46
                        short len = br.ReadInt16();//48
                        sw.WriteLine(string.Format("Idx:{0} Type:{1} Count:{2} Offset:{9} Length:{10} Hide:{3} U1:{4} U2:{5} U3:{6} U4:{7} U5:{8}", i, type, sCount, hide, u1, u2, u3, u4, u5, off, len));
                    }                    
                }
                fs.Close();
                sw.Flush();
                sw.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.bin|*.bin";
            List<Table.Table> tbls = new List<Table.Table>();
            if (!System.IO.Directory.Exists("datafile"))
            {
                System.IO.Directory.CreateDirectory("datafile");
            }

            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileStream fs = new System.IO.FileStream(OD.FileName, System.IO.FileMode.Open);
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                int tblCount, offset;
                fs.Position = 21;
                tblCount = br.ReadInt32();
                offset = br.ReadInt32() + 99;

                fs.Position = offset;
                for (int i = 0; i < tblCount; i++)
                {
                    long begin = fs.Position;
                    Table.Table tbl = new Table.Table()
                    {
                        Name = tblName[i],
                        Unknown1 = br.ReadByte(),
                        Unknown2 = br.ReadByte(),
                        Size = br.ReadInt32(),
                        EntryCount = br.ReadInt32(),
                        EntrySize = br.ReadInt32(),
                        DataSize = br.ReadInt32(),
                        Unknown3 = br.ReadByte(),
                        EntryOffset = (int)fs.Position
                    };
                    fs.Position += tbl.EntrySize;
                    tbl.DataOffset = (int)fs.Position;
                    fs.Position = tbl.EntryOffset;
                    if (listBox2.SelectedItems.Contains(tbl.Name))
                    {
                        if (tbl.EntryCount == 0)
                        {
                            continue;
                        }
                        System.IO.FileStream fs2 = new System.IO.FileStream("datafile/" + tbl.Name + ".bin", System.IO.FileMode.Create);
                        int id = tbl.EntryCount / 2;
                        for (int j = 0; j < id && fs.Position < tbl.DataOffset; j++)
                        {
                            long entryBegin = fs.Position;
                            Table.Entry entry = new Table.Entry()
                            {
                                Unknown1 = br.ReadInt32(),
                                Unknown2 = br.ReadInt16(),
                                Unknown3 = br.ReadInt16(),
                                Size = br.ReadInt32(),
                                Index = br.ReadInt32(),
                                Index2 = br.ReadInt32()
                            };
                            fs.Position = entryBegin + entry.Size + 4;
                        }
                        {
                            long entryBegin = fs.Position;
                            Table.Entry entry = new Table.Entry()
                            {
                                Unknown1 = br.ReadInt32(),
                                Unknown2 = br.ReadInt16(),
                                Unknown3 = br.ReadInt16(),
                                Size = br.ReadInt32(),
                                Index = br.ReadInt32(),
                                Index2 = br.ReadInt32()
                            };
                            byte[] buf = br.ReadBytes(entry.Size - 16);
                            fs2.Write(buf, 0, buf.Length);
                            fs.Position = entryBegin + entry.Size + 4;
                            fs2.Flush();
                            fs2.Close();
                        }
                    }
                    fs.Position = begin + tbl.Size + 6;
                }
                fs.Close();
            }
        }

        private void cBT1ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (cBT1ToolStripMenuItem.Checked)
            {
                cBT2ToolStripMenuItem.Checked = false;
                oBTToolStripMenuItem.Checked = false;
                cBT3ToolStripMenuItem.Checked = false;
                retailV82ToolStripMenuItem.Checked = false;
                retailV114ToolStripMenuItem.Checked = false;
                InitTblNameCBT1();
            }
        }

        private void cBT2ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (cBT2ToolStripMenuItem.Checked)
            {
                cBT1ToolStripMenuItem.Checked = false;
                cBT3ToolStripMenuItem.Checked = false;
                oBTToolStripMenuItem.Checked = false;
                retailV82ToolStripMenuItem.Checked = false;
                retailV114ToolStripMenuItem.Checked = false;
                InitTblNameCBT2();
            }
        }

        private void cBT3ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (cBT3ToolStripMenuItem.Checked)
            {
                cBT1ToolStripMenuItem.Checked = false;
                cBT2ToolStripMenuItem.Checked = false;
                oBTToolStripMenuItem.Checked = false;
                retailV82ToolStripMenuItem.Checked = false;
                retailV114ToolStripMenuItem.Checked = false;
                InitTblNameCBT3();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OD.Filter = "text.xml|text.xml";
            MessageBox.Show("Please select old text.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Dictionary<string, string> translations = new Dictionary<string, string>();
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(OD.FileName);
                XmlElement root = xml["text"];
                foreach (object j in root.ChildNodes)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "entry")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string name = "", value = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "name")
                            {
                                name = k.InnerText;
                            }
                            if (k.Name.ToLower() == "value")
                            {
                                value = k.InnerText;
                            }
                        }
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        {
                            translations[name] = value;
                        }
                    }
                }                
            }
            MessageBox.Show("Please select new text.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(OD.FileName);
                XmlElement root = xml["text"];
                foreach (object j in root.ChildNodes)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "entry")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string name = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "name")
                            {
                                name = k.InnerText;
                            }
                            if (k.Name.ToLower() == "value")
                            {
                                if (translations.ContainsKey(name))
                                {
                                    k.InnerText = translations[name];
                                }
                            }
                        }
                    }
                }
                xml.Save(OD.FileName);
                MessageBox.Show("Finished!", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OD.Filter = "*.xml|*.xml";
            MessageBox.Show("Please select skill.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(OD.FileName);
                XmlElement root = xml["skill"];
                XmlNodeList list = root.ChildNodes;
                Dictionary<string, XmlElement> skills = new Dictionary<string, XmlElement>();
                List<string> already = new List<string>();
                List<XmlElement> alreadyNode = new List<XmlElement>();
                foreach (object j in list)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement))
                    {
                        continue;
                    }

                    i = (XmlElement)j;
                    if (i.Name.ToLower() == "skill")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string name = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "name")
                            {
                                name = k.InnerText;
                            }
                            if (k.Name.ToLower() == "id")
                            {
                                if (already.Contains(k.InnerText))
                                {
                                    alreadyNode.Add(i);
                                }
                                else
                                {
                                    already.Add(k.InnerText);
                                }
                            }
                        }
                        skills[name] = i;
                    }
                }
                foreach (XmlElement i in alreadyNode)
                {
                    root.RemoveChild(i);
                }

                MessageBox.Show("Please select skillCastCondition.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    XmlDocument xml2 = new XmlDocument();
                    xml2.Load(OD.FileName);
                    XmlElement root2 = xml2["SkillCastCondition"];
                    foreach (object j in root2.ChildNodes)
                    {
                        XmlElement i;
                        if (j.GetType() != typeof(XmlElement))
                        {
                            continue;
                        }

                        i = (XmlElement)j;
                        if (i.Name.ToLower() == "condition")
                        {
                            XmlNodeList children = i.ChildNodes;
                            string name = string.Empty;
                            XmlElement castMin = null, castMax = null, bladeSpirit = null, force = null, related = null;
                            bool hasMana = true, hasForce = false;
                            foreach (object l in children)
                            {
                                XmlElement k = l as XmlElement;
                                if (k == null)
                                {
                                    continue;
                                }

                                switch (k.Name.ToLower())
                                {
                                    case "name":
                                        name = k.InnerText;
                                        break;
                                    case "castrangemin":
                                        {
                                            castMin = xml.CreateElement("CastRangeMin");
                                            castMin.InnerText = k.InnerText;
                                        }
                                        break;
                                    case "castrangemax":
                                        {
                                            castMax = xml.CreateElement("CastRangeMax");
                                            castMax.InnerText = k.InnerText;
                                        }
                                        break;
                                    case "forcecost":
                                        {
                                            if (k.InnerText != "0")
                                            {
                                                force = xml.CreateElement("ManaCost");
                                                force.InnerText = k.InnerText;
                                                hasForce = true;
                                                hasMana = false;
                                            }
                                        }
                                        break;
                                    case "manacost":
                                        {
                                            bladeSpirit = xml.CreateElement("ManaCost");
                                            bladeSpirit.InnerText = k.InnerText;
                                        }
                                        break;
                                    case "previousskills":
                                        {
                                            related = xml.CreateElement("PreviousSkills");
                                            related.InnerText = k.InnerText;
                                        }
                                        break;
                                }                              
                            }
                            if (skills.ContainsKey(name) && skills[name]["CastRangeMin"] == null)
                            {
                                skills[name].AppendChild(castMin);
                                skills[name].AppendChild(castMax);
                                if (hasMana)
                                {
                                    skills[name].AppendChild(bladeSpirit);
                                }

                                if (hasForce)
                                {
                                    skills[name].AppendChild(force);
                                }

                                skills[name].AppendChild(related);
                            }
                        }
                    }
                    string path = System.IO.Path.GetDirectoryName(OD.FileName) + "\\skill_templates.xml";
                    xml.Save(path);                    
                }
            }
        }

        private void oBTToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cBT3ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cBT2ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void BetaUnpack_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void oBTToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (oBTToolStripMenuItem.Checked)
            {
                cBT1ToolStripMenuItem.Checked = false;
                cBT2ToolStripMenuItem.Checked = false;
                cBT3ToolStripMenuItem.Checked = false;
                retailV82ToolStripMenuItem.Checked = false;
                retailV114ToolStripMenuItem.Checked = false;
                InitTblNameOBT();
            }
        }

        private void retailV82ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (retailV82ToolStripMenuItem.Checked)
            {
                cBT1ToolStripMenuItem.Checked = false;
                cBT2ToolStripMenuItem.Checked = false;
                cBT3ToolStripMenuItem.Checked = false;
                oBTToolStripMenuItem.Checked = false;
                retailV114ToolStripMenuItem.Checked = false;
                InitTblNameV82();
            }
        }

        private void retailV114ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (retailV114ToolStripMenuItem.Checked)
            {
                cBT1ToolStripMenuItem.Checked = false;
                cBT2ToolStripMenuItem.Checked = false;
                cBT3ToolStripMenuItem.Checked = false;
                oBTToolStripMenuItem.Checked = false;
                retailV82ToolStripMenuItem.Checked = false;
                InitTblNameV114();
            }
        }
    }
}
