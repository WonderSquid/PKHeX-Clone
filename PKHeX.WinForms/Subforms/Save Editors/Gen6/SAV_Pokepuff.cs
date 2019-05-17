﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Pokepuff : Form
    {
        private readonly IPokePuff SAV;

        public SAV_Pokepuff(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (IPokePuff)sav;

            var puffs = SAV.Puffs;
            Setup(puffs.Length);
            LoadPuffs(puffs);

            new ToolTip().SetToolTip(B_Sort, "Hold CTRL to reverse sort.");
            new ToolTip().SetToolTip(B_All, "Hold CTRL to best instead of varied.");
        }

        private readonly string[] pfa = GameInfo.Strings.puffs;
        private int PuffCount { get; set; }

        private void Setup(int rowCount)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Slot";
                dgvIndex.DisplayIndex = 0;
                dgvIndex.Width = 45;
                dgvIndex.ReadOnly = true;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvPuff = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in pfa)
                    dgvPuff.Items.Add(t);

                dgvPuff.DisplayIndex = 1;
                dgvPuff.Width = 135;
                dgvPuff.FlatStyle = FlatStyle.Flat;
            }
            dgv.Columns.Add(dgvIndex);
            dgv.Columns.Add(dgvPuff);
            dgv.Rows.Add(rowCount);
        }

        private void LoadPuffs(byte[] Puffs)
        {
            PuffCount = Puffs.Length;
            for (int i = 0; i < Puffs.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = (i + 1).ToString();
                int puffval = Puffs[i];
                if (puffval >= pfa.Length)
                {
                    WinFormsUtil.Error($"Invalid Puff Index: {i}", $"Expected < ${pfa.Length}");
                    puffval = 0;
                }
                dgv.Rows[i].Cells[1].Value = pfa[puffval];
            }
        }

        private void DropClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;
            ((ComboBox)((DataGridView) sender).EditingControl).DroppedDown = true;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            int[] plus10 = {21, 22};
            byte[] newpuffs = new byte[PuffCount];

            if (ModifierKeys == Keys.Control)
            {
                for (int i = 0; i < PuffCount; i++)
                    newpuffs[i] = (byte)plus10[Util.Rand.Next(2)];
            }
            else
            {
                for (int i = 0; i < PuffCount; i++)
                    newpuffs[i] = (byte)((i % (pfa.Length - 1)) + 1);
                Util.Shuffle(newpuffs);
            }

            LoadPuffs(newpuffs);
        }

        private void B_None_Click(object sender, EventArgs e)
        {
            byte[] newpuffs = new byte[PuffCount];
            newpuffs[0] = 1;
            newpuffs[1] = 2;
            newpuffs[2] = 3;
            newpuffs[3] = 4;
            newpuffs[4] = 5;
            LoadPuffs(newpuffs);
        }

        private void B_Sort_Click(object sender, EventArgs e)
        {
            bool reverse = ModifierKeys == Keys.Control;
            var puffs = GetPuffs().GroupBy(z => z != 0);
            var result = puffs.SelectMany(z => reverse ? z.OrderByDescending(x => x) : z.OrderBy(x => x)).ToArray();
            LoadPuffs(result);
        }

        private byte[] GetPuffs()
        {
            var puffs = new List<byte>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                string puff = dgv.Rows[i].Cells[1].Value.ToString();
                int index = (byte)Array.IndexOf(pfa, puff);
                puffs.Add((byte)index);
            }
            return puffs.ToArray();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            var puffs = GetPuffs();
            SAV.Puffs = puffs;
            SAV.PuffCount = puffs.Length;
            Close();
        }
    }
}
