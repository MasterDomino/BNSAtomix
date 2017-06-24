using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SkillData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<string, XmlElement> SkillList;
        private XmlDocument SkillXml;
        private string SkillFile = string.Empty;
        private Dictionary<string, XmlElement> NpcList;
        private XmlDocument NpcXml;
        private string NpcFile = string.Empty;

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip()
            {
                AutoPopDelay = 10000,
                InitialDelay = 500,
                ReshowDelay = 100,
                ShowAlways = true
            };
            toolTip1.SetToolTip(this.Skill_NoTargetTypes, "Linear 直线\r\nAngular 扇形");
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SkillList = new Dictionary<string, XmlElement>();
            SkillListBox.Items.Clear();
            OD.Filter = "*.xml|*.xml";
            MessageBox.Show("Please select skill_templates.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            List<string> skillListTemp = new List<string>();
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TSL.Text = "正在载入数据";
                SkillXml = new XmlDocument();
                SkillFile = OD.FileName;
                SkillXml.Load(SkillFile);
                XmlElement root = SkillXml["skill"];
                XmlNodeList list = root.ChildNodes;
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
                        string npcid = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "id")
                            {
                                npcid = k.InnerText;
                                break;
                            }
                        }
                        SkillList[npcid] = i;
                        skillListTemp.Add(npcid);
                    }
                }
            }
            else
            {
                TSL.Text = "未选择文件";
            }

            if (SkillList.Count > 0)
            {
                button1.Enabled = true;
                TSL.Text = "文件载入成功,共" + SkillList.Count + "项数据";
                //skillListTemp.Sort();
                SkillListBox.Items.AddRange(skillListTemp.ToArray());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Skill_ID.Text = string.Empty;
            Skill_name.Text = string.Empty;
            Skill_SkillType.Text = string.Empty;
            Skill_MinAtk.Text = string.Empty;
            Skill_MaxAtk.Text = string.Empty;
            Skill_CastRangeMin.Text = string.Empty;
            Skill_CastRangeMax.Text = string.Empty;
            Skill_ManaCost.Text = string.Empty;
            Skill_CastTime.Text = string.Empty;
            Skill_CoolDown.Text = string.Empty;
            Skill_ActionTime.Text = string.Empty;
            Skill_ActivationTime.Text = string.Empty;
            Skill_NoTargetTypes.Text = string.Empty;
            Skill_NoTargetWidth.Text = string.Empty;
            Skill_NoTargetAngle.Text = string.Empty;
            Skill_ShouldApproach.Checked = false;
            Skill_ApproachTimeRate.Text = string.Empty;
            Skill_Duration.Text = string.Empty;
            Skill_BonusAddition.Text = string.Empty;
            Skill_BonusRate.Text = string.Empty;
            Skill_MovementLockOnCasting.Text = string.Empty;
            Skill_MovementLockOnAction.Text = string.Empty;
            Skill_RequiredCasterStance.Text = string.Empty;
            Skill_RequiredTargetStance.Text = string.Empty;
            Skill_RelatedSkills.Text = string.Empty;
            Skill_PreviousSkills.Text = string.Empty;
            Skill_Remark.Text = string.Empty;

            if (SkillList?.ContainsKey(Skill_search.Text) == true)
            {
                foreach (object l in SkillList[Skill_search.Text].ChildNodes)
                {
                    XmlElement current = l as XmlElement;
                    if (current == null)
                    {
                        continue;
                    }

                    switch (current.Name.ToLower())
                    {
                        case "id":
                            Skill_ID.Text = current.InnerText;
                            break;
                        case "name":
                            Skill_name.Text = current.InnerText;
                            break;
                        case "skilltype":
                            Skill_SkillType.Text = current.InnerText;
                            break;
                        case "minatk":
                            Skill_MinAtk.Text = current.InnerText;
                            break;
                        case "maxatk":
                            Skill_MaxAtk.Text = current.InnerText;
                            break;
                        case "castrangemin":
                            Skill_CastRangeMin.Text = current.InnerText;
                            break;
                        case "castrangemax":
                            Skill_CastRangeMax.Text = current.InnerText;
                            break;
                        case "manacost":
                            Skill_ManaCost.Text = current.InnerText;
                            break;
                        case "casttime":
                            Skill_CastTime.Text = current.InnerText;
                            break;
                        case "cooldown":
                            Skill_CoolDown.Text = current.InnerText;
                            break;
                        case "actiontime":
                            Skill_ActionTime.Text = current.InnerText;
                            break;
                        case "activationtime":
                            Skill_ActivationTime.Text = current.InnerText;
                            break;
                        case "notargettype":
                            Skill_NoTargetTypes.Text = current.InnerText;
                            break;
                        case "notargetwidth":
                            Skill_NoTargetWidth.Text = current.InnerText;
                            break;
                        case "notargetangle":
                            Skill_NoTargetAngle.Text = current.InnerText;
                            break;
                        case "shouldapproach":
                            Skill_ShouldApproach.Checked = current.InnerText == "1" ? true : false;
                            break;
                        case "approachtimerate":
                            Skill_ApproachTimeRate.Text = current.InnerText;
                            break;
                        case "duration":
                            Skill_Duration.Text = current.InnerText;
                            break;
                        case "bonusaddition":
                            Skill_BonusAddition.Text = current.InnerText;
                            break;
                        case "bonusrate":
                            Skill_BonusRate.Text = current.InnerText;
                            break;
                        case "movementlockoncasting":
                            Skill_MovementLockOnCasting.Text = current.InnerText;
                            break;
                        case "movementlockonaction":
                            Skill_MovementLockOnAction.Text = current.InnerText;
                            break;
                        case "casterstance":
                            Skill_RequiredCasterStance.Text = current.InnerText;
                            break;
                        case "targetstance":
                            Skill_RequiredTargetStance.Text = current.InnerText;
                            break;
                        case "relatedskills":
                            Skill_RelatedSkills.Text = current.InnerText;
                            break;
                        case "previousskills":
                            Skill_PreviousSkills.Text = current.InnerText;
                            break;
                        case "remark":
                            Skill_Remark.Text = current.InnerText;
                            break;
                    }
                }
                ShouldApproach_CheckedChanged(sender, e);
                TSL.Text = "数据已打开";
                SkillListBox.SelectedIndex = SkillListBox.Items.IndexOf(Skill_search.Text);
            }
            else
            {
                TSL.Text = "ID不匹配";
            }
        }

        private void ShouldApproach_CheckedChanged(object sender, EventArgs e)
        {
            if (Skill_ShouldApproach.Checked)
            {
                Skill_ActionTime.Text = string.Empty;
                Skill_ActionTime.Enabled = false;
            }
            else
            {
                Skill_ActionTime.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Skill_ID.Text != string.Empty && SkillList.ContainsKey(Skill_ID.Text))
            {
                SkillList[Skill_search.Text].RemoveAll();
                XmlElement xe = SkillXml.CreateElement("ID");
                xe.InnerText = Skill_ID.Text;
                SkillList[Skill_search.Text].AppendChild(xe);
                if (Skill_name.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("Name");
                    xe.InnerText = Skill_name.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_SkillType.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("SkillType");
                    xe.InnerText = Skill_SkillType.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_MinAtk.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("MinAtk");
                    xe.InnerText = Skill_MinAtk.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_MaxAtk.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("MaxAtk");
                    xe.InnerText = Skill_MaxAtk.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_CastRangeMin.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("CastRangeMin");
                    xe.InnerText = Skill_CastRangeMin.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_CastRangeMax.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("CastRangeMax");
                    xe.InnerText = Skill_CastRangeMax.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_ManaCost.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("ManaCost");
                    xe.InnerText = Skill_ManaCost.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_CastTime.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("CastTime");
                    xe.InnerText = Skill_CastTime.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_CoolDown.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("CoolDown");
                    xe.InnerText = Skill_CoolDown.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_ActionTime.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("ActionTime");
                    xe.InnerText = Skill_ActionTime.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_ActivationTime.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("ActivationTime");
                    xe.InnerText = Skill_ActivationTime.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_NoTargetTypes.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("NoTargetType");
                    xe.InnerText = Skill_NoTargetTypes.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_NoTargetWidth.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("NoTargetWidth");
                    xe.InnerText = Skill_NoTargetWidth.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_NoTargetAngle.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("NoTargetAngle");
                    xe.InnerText = Skill_NoTargetAngle.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_ShouldApproach.Checked)
                {
                    xe = SkillXml.CreateElement("ShouldApproach");
                    xe.InnerText = "1";
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_ApproachTimeRate.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("ApproachTimeRate");
                    xe.InnerText = Skill_ApproachTimeRate.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_Duration.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("Duration");
                    xe.InnerText = Skill_Duration.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_BonusAddition.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("BonusAddition");
                    xe.InnerText = Skill_BonusAddition.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_BonusRate.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("BonusRate");
                    xe.InnerText = Skill_BonusRate.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_MovementLockOnCasting.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("MovementLockOnCasting");
                    xe.InnerText = Skill_MovementLockOnCasting.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_MovementLockOnAction.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("MovementLockOnAction");
                    xe.InnerText = Skill_MovementLockOnAction.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_RequiredCasterStance.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("CasterStance");
                    xe.InnerText = Skill_RequiredCasterStance.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_RequiredTargetStance.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("TargetStance");
                    xe.InnerText = Skill_RequiredTargetStance.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_RelatedSkills.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("RelatedSkills");
                    xe.InnerText = Skill_RelatedSkills.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_PreviousSkills.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("PreviousSkills");
                    xe.InnerText = Skill_PreviousSkills.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                if (Skill_Remark.Text != string.Empty)
                {
                    xe = SkillXml.CreateElement("Remark");
                    xe.InnerText = Skill_Remark.Text;
                    SkillList[Skill_search.Text].AppendChild(xe);
                }
                
                TSL.Text = "数据已保存到库";
            }
            else
            {
                TSL.Text = "错误,无效的数据";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (SkillList != null)
            {
                foreach (KeyValuePair<string, XmlElement> i in SkillList)
                {
                    Skill_search.Text = i.Key;
                    button1_Click(sender, e);
                    button2_Click(sender, e);
                }
            }

            MessageBox.Show("Fin");
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SkillFile == "")
            {
                TSL.Text = "没有数据";
                return;
            }
            else
            {
                SkillXml.Save(SkillFile);
                TSL.Text = "文件保存成功";
            }
        }

        private void SkillType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Skill_SkillType.Text == "NoTarget")
            {
                Skill_NoTargetTypes.Enabled = true;
            }
            else
            {
                Skill_NoTargetTypes.Enabled = false;
                Skill_NoTargetAngle.Enabled = false;
                Skill_NoTargetWidth.Enabled = false;
                Skill_NoTargetTypes.Text = string.Empty;
            }
        }

        private void NoTargetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Skill_NoTargetTypes.Text == "")
            {
                if (Skill_SkillType.Text == "NoTarget")
                {
                    Skill_NoTargetTypes.Text = "Angular";
                    Skill_NoTargetAngle.Enabled = true;
                }
                else
                {
                    Skill_NoTargetAngle.Enabled = false;
                }

                Skill_NoTargetWidth.Enabled = false;
            }
            else if (Skill_NoTargetTypes.Text == "Angular")
            {
                Skill_NoTargetAngle.Enabled = true;
                Skill_NoTargetWidth.Enabled = false;
            }
            else
            {
                Skill_NoTargetAngle.Enabled = false;
                Skill_NoTargetWidth.Enabled = true;
            }
        }

        private void SkillListBox_DoubleClick(object sender, EventArgs e)
        {
            Skill_search.Text = SkillListBox.SelectedItem.ToString();
            button1_Click(sender, e);
        }

        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NpcList = new Dictionary<string, XmlElement>();
            NpcListBox.Items.Clear();
            OD.Filter = "*.xml|*.xml";
            MessageBox.Show("Please select Npc_templates.xml", "Please select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            List<string> NpcListTemp = new List<string>();
            if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TSL.Text = "正在载入数据";
                NpcXml = new XmlDocument();
                NpcFile = OD.FileName;
                NpcXml.Load(NpcFile);
                XmlElement root = NpcXml["npc"];
                XmlNodeList list = root.ChildNodes;
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
                    if (i.Name.ToLower() == "npc")
                    {
                        XmlNodeList children = i.ChildNodes;
                        string npcid = string.Empty;
                        foreach (object l in children)
                        {
                            XmlElement k = l as XmlElement;
                            if (k == null)
                            {
                                continue;
                            }

                            if (k.Name.ToLower() == "id")
                            {
                                npcid = k.InnerText;
                                break;
                            }
                        }
                        NpcList[npcid] = i;
                        NpcListTemp.Add(npcid);
                    }
                }
            }
            else
            {
                TSL.Text = "未选择文件";
            }

            if (NpcList.Count > 0)
            {
                button1.Enabled = true;
                TSL.Text = "文件载入成功,共" + NpcList.Count + "项数据";
                NpcListBox.Items.AddRange(NpcListTemp.ToArray());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
