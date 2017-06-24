using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkillData
{
    public partial class NpcSkill : UserControl
    {
        public NpcSkill()
        {
            InitializeComponent();
        }

        public string ID { get { return SkillID.Text; } }
        public string Rate { get { return SkillRate.Text; } }
        public bool Being { get { if (SkillRate.Text != null && SkillRate.Text != string.Empty) { return true;
                }
                else
                {
                    return false; } } }
    }
}
