﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class StatEditor : UserControl
    {
        public StatEditor()
        {
            InitializeComponent();
            MT_EVs   = new[] {TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPEEV, TB_SPAEV, TB_SPDEV};
            MT_IVs   = new[] {TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV};
            MT_Stats = new[] {Stat_HP, Stat_ATK, Stat_DEF, Stat_SPE, Stat_SPA, Stat_SPD};
            L_Stats  = new[] {Label_HP, Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD};
            MT_Base  = new[] {TB_HPBase, TB_ATKBase, TB_DEFBase, TB_SPEBase, TB_SPABase, TB_SPDBase};

            TB_BST.ResetForeColor();
            TB_IVTotal.ForeColor = MT_EVs[0].ForeColor;
            TB_EVTotal.ForeColor = MT_EVs[0].ForeColor;
        }

        public Color EVsInvalid { get; set; } = Color.Red;
        public Color EVsMaxed { get; set; } = Color.Honeydew;
        public Color EVsFishy { get; set; } = Color.LightYellow;
        public Color StatIncreased { get; set; } = Color.Red;
        public Color StatDecreased { get; set; } = Color.Blue;
        public Color StatHyperTrained { get; set; } = Color.LightGreen;

        private IMainEditor MainEditor { get; set; }

        public void SetMainEditor(IMainEditor editor)
        {
            MainEditor = editor;
            CHK_HackedStats.Enabled = CHK_HackedStats.Visible = editor.HaX;
        }

        public bool Valid => pkm.Format < 3 || Convert.ToUInt32(TB_EVTotal.Text) <= 510 || CHK_HackedStats.Checked;

        private readonly Label[] L_Stats;
        private readonly MaskedTextBox[] MT_EVs, MT_IVs, MT_Stats, MT_Base;
        private readonly ToolTip EVTip = new ToolTip();
        private PKM pkm => MainEditor.pkm;

        private bool ChangingFields
        {
            get => MainEditor.ChangingFields;
            set => MainEditor.ChangingFields = value;
        }

        private bool FieldsInitialized => MainEditor.FieldsInitialized;

        private void ClickIV(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox t))
                return;

            if (ModifierKeys.HasFlag(Keys.Alt)) // Min
            {
                t.Text = 0.ToString();
            }
            else if (ModifierKeys.HasFlag(Keys.Control))
            {
                var index = Array.IndexOf(MT_IVs, t);
                t.Text = pkm.GetMaximumIV(index, true).ToString();
            }
            else if (pkm is IHyperTrain h && ModifierKeys.HasFlag(Keys.Shift))
            {
                var index = Array.IndexOf(MT_IVs, t);
                bool flag = h.HyperTrainInvert(index);
                UpdateHyperTrainingFlag(index, flag);
                UpdateStats();
            }
        }

        private void ClickEV(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox t))
                return;

            if (!ModifierKeys.HasFlag(Keys.Control)) // Max
            {
                if (ModifierKeys.HasFlag(Keys.Alt)) // Min
                    t.Text = 0.ToString();
                return;
            }

            int index = Array.IndexOf(MT_EVs, t);
            int newEV = pkm.GetMaximumEV(index);
            t.Text = newEV.ToString();
        }

        public void UpdateIVs(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox m)
            {
                int value = Util.ToInt32(m.Text);
                if (value > pkm.MaxIV)
                {
                    m.Text = pkm.MaxIV.ToString();
                    return; // recursive on text set
                }

                int index = Array.IndexOf(MT_IVs, m);
                pkm.SetIV(index, value);
            }
            RefreshDerivedValues(e);
            UpdateStats();
        }

        private void RefreshDerivedValues(object sender)
        {
            if (pkm.Format < 3)
            {
                TB_HPIV.Text = pkm.IV_HP.ToString();
                TB_SPDIV.Text = pkm.IV_SPD.ToString();

                MainEditor.UpdateIVsGB(sender == null);
            }

            if (!ChangingFields)
            {
                ChangingFields = true;
                CB_HPType.SelectedValue = pkm.HPType;
                ChangingFields = false;
            }

            // Potential Reading
            L_Potential.Text = pkm.GetPotentialString(MainEditor.Unicode);

            TB_IVTotal.Text = pkm.IVTotal.ToString();
            UpdateCharacteristic(pkm.Characteristic);
        }

        private void UpdateEVs(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox m)
            {
                int value = Util.ToInt32(m.Text);
                if (value > pkm.MaxEV)
                {
                    m.Text = pkm.MaxEV.ToString();
                    return; // recursive on text set
                }

                int index = Array.IndexOf(MT_EVs, m);
                pkm.SetEV(index, value);
            }

            UpdateEVTotals();

            if (pkm.Format < 3)
            {
                ChangingFields = true;
                TB_SPDEV.Text = TB_SPAEV.Text;
                ChangingFields = false;
            }

            UpdateStats();
        }

        private void UpdateRandomEVs(object sender, EventArgs e)
        {
            bool zero = ModifierKeys.HasFlag(Keys.Control);
            var evs = zero ? new int[6] : PKX.GetRandomEVs(pkm.Format);
            LoadEVs(evs);
            UpdateEVs(null, null);
        }

        private void UpdateHackedStats(object sender, EventArgs e)
        {
            foreach (var s in MT_Stats)
                s.Enabled = CHK_HackedStats.Checked;
            if (!CHK_HackedStats.Checked)
                UpdateStats();
        }

        private void UpdateHackedStatText(object sender, EventArgs e)
        {
            if (!CHK_HackedStats.Checked || !(sender is TextBox tb))
                return;

            string text = tb.Text;
            if (string.IsNullOrWhiteSpace(text))
                tb.Text = "0";

            if (Convert.ToUInt32(text) > ushort.MaxValue)
                tb.Text = "65535";
        }

        private void UpdateHyperTrainingFlag(int i, bool value)
        {
            if (value)
                MT_IVs[i].BackColor = StatHyperTrained;
            else
                MT_IVs[i].ResetBackColor();
        }

        private void UpdateHPType(object sender, EventArgs e)
        {
            if (ChangingFields || !FieldsInitialized)
                return;

            // Change IVs to match the new Hidden Power
            int hpower = WinFormsUtil.GetIndex(CB_HPType);
            int[] newIVs = PKX.SetHPIVs(hpower, pkm.IVs, pkm.Format);
            LoadIVs(newIVs);
        }

        private void ClickStatLabel(object sender, MouseEventArgs e)
        {
            if (sender == Label_SPC)
                sender = Label_SPA;

            int index = Array.IndexOf(L_Stats, sender as Label);
            if (ModifierKeys.HasFlag(Keys.Alt)) // EV
            {
                var value = e.Button != MouseButtons.Left ? 0 : pkm.GetMaximumEV(index);
                MT_EVs[index].Text = value.ToString();
            }
            else if (ModifierKeys.HasFlag(Keys.Control)) // IV
            {
                var value = e.Button != MouseButtons.Left ? 0 : pkm.GetMaximumIV(index, true);
                MT_IVs[index].Text = value.ToString();
            }
        }

        private void LoadHyperTraining()
        {
            if (!(pkm is IHyperTrain h))
            {
                foreach (var iv in MT_IVs)
                    iv.ResetBackColor();
                return;
            }

            for (int i = 0; i < MT_IVs.Length; i++)
                UpdateHyperTrainingFlag(i, h.GetHT(i));
        }

        private void UpdateEVTotals()
        {
            int evtotal = pkm.EVTotal;

            if (evtotal > 510) // Background turns Red
                TB_EVTotal.BackColor = EVsInvalid;
            else if (evtotal == 510) // Maximum EVs
                TB_EVTotal.BackColor = EVsMaxed;
            else if (evtotal == 508) // Fishy EVs
                TB_EVTotal.BackColor = EVsFishy;
            else TB_EVTotal.BackColor = TB_IVTotal.BackColor;

            TB_EVTotal.Text = evtotal.ToString();
            EVTip.SetToolTip(TB_EVTotal, $"Remaining: {510 - evtotal}");
        }

        public void UpdateStats()
        {
            // Generate the stats.
            if (!CHK_HackedStats.Checked || pkm.Stat_HPCurrent == 0) // no stats when initially loaded from non-partyformat slot
            {
                var pi = pkm.PersonalInfo;
                pkm.SetStats(pkm.GetStats(pi));
                LoadBST(pi);
                LoadPartyStats(pkm);
            }
            RecolorStatLabels(pkm.Nature);
        }

        private void LoadBST(PersonalInfo pi)
        {
            var stats = pi.Stats;
            for (int i = 0; i < stats.Length; i++)
            {
                MT_Base[i].Text = stats[i].ToString("000");
                MT_Base[i].BackColor = ImageUtil.ColorBaseStat(stats[i]);
            }
            var bst = pi.BST;
            TB_BST.Text = bst.ToString("000");
            TB_BST.BackColor = ImageUtil.ColorBaseStat((int)(Math.Max(0, bst - 175) / 3f));
        }

        public void UpdateRandomIVs(object sender, EventArgs e)
        {
            int? flawless = ModifierKeys.HasFlag(Keys.Control) ? (int?)6 : null;
            var IVs = pkm.SetRandomIVs(flawless);
            LoadIVs(IVs);
        }

        public void UpdateCharacteristic() => UpdateCharacteristic(pkm.Characteristic);

        private void UpdateCharacteristic(int characteristic)
        {
            L_Characteristic.Visible = Label_CharacteristicPrefix.Visible = characteristic > -1;
            if (characteristic > -1)
                L_Characteristic.Text = GameInfo.Strings.characteristics[characteristic];
        }

        private void RecolorStatLabels(int nature)
        {
            // Reset Label Colors
            foreach (var label in L_Stats.Skip(1))
                label.ResetForeColor();

            // Set Colored StatLabels only if Nature isn't Neutral
            if (PKX.GetNatureModification(nature, out int incr, out int decr))
                return;
            L_Stats[incr].ForeColor = StatIncreased;
            L_Stats[decr].ForeColor = StatDecreased;
        }

        public string UpdateNatureModification(int nature)
        {
            // Reset Label Colors
            foreach (var label in L_Stats.Skip(1))
                label.ResetForeColor();

            // Set Colored StatLabels only if Nature isn't Neutral
            if (PKX.GetNatureModification(nature, out int incr, out int decr))
                return "-/-";
            return $"+{L_Stats[incr].Text} / -{L_Stats[decr].Text}".Replace(":", "");
        }

        public void SetATKIVGender(int gender)
        {
            pkm.SetATKIVGender(gender);
            TB_ATKIV.Text = pkm.IV_ATK.ToString();
        }

        public void LoadPartyStats(PKM pk)
        {
            int size = pk.SIZE_PARTY;
            if (pk.Data.Length != size)
                Array.Resize(ref pk.Data, size);

            Stat_HP.Text = pk.Stat_HPCurrent.ToString();
            Stat_ATK.Text = pk.Stat_ATK.ToString();
            Stat_DEF.Text = pk.Stat_DEF.ToString();
            Stat_SPA.Text = pk.Stat_SPA.ToString();
            Stat_SPD.Text = pk.Stat_SPD.ToString();
            Stat_SPE.Text = pk.Stat_SPE.ToString();
        }

        public void SavePartyStats(PKM pk)
        {
            int size = pk.SIZE_PARTY;
            if (pk.Data.Length != size)
                Array.Resize(ref pk.Data, size);

            pk.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk.Stat_SPD = Util.ToInt32(Stat_SPD.Text);
        }

        public void LoadEVs(int[] EVs)
        {
            ChangingFields = true;
             TB_HPEV.Text = EVs[0].ToString();
            TB_ATKEV.Text = EVs[1].ToString();
            TB_DEFEV.Text = EVs[2].ToString();
            TB_SPEEV.Text = EVs[3].ToString();
            TB_SPAEV.Text = EVs[4].ToString();
            TB_SPDEV.Text = EVs[5].ToString();
            ChangingFields = false;
        }

        public void LoadIVs(int[] IVs)
        {
            ChangingFields = true;
             TB_HPIV.Text = IVs[0].ToString();
            TB_ATKIV.Text = IVs[1].ToString();
            TB_DEFIV.Text = IVs[2].ToString();
            TB_SPEIV.Text = IVs[3].ToString();
            TB_SPAIV.Text = IVs[4].ToString();
            TB_SPDIV.Text = IVs[5].ToString();
            ChangingFields = false;
            LoadHyperTraining();
            RefreshDerivedValues(TB_SPDIV);
            UpdateStats();
        }

        public void ToggleInterface(int gen)
        {
            FLP_StatsTotal.Visible = gen >= 3;
            FLP_Characteristic.Visible = gen >= 3;

            switch (gen)
            {
                case 1:
                    FLP_SpD.Visible = false;
                    Label_SPA.Visible = false;
                    Label_SPC.Visible = true;
                    TB_HPIV.Enabled = false;
                    SetMaskSize(Stat_HP.Size, "00000");
                    break;
                case 2:
                    FLP_SpD.Visible = true;
                    Label_SPA.Visible = true;
                    Label_SPC.Visible = false;
                    TB_HPIV.Enabled = false;
                    SetMaskSize(Stat_HP.Size, "00000");
                    TB_SPDEV.Enabled = TB_SPDIV.Enabled = false;
                    break;
                default:
                    FLP_SpD.Visible = true;
                    Label_SPA.Visible = true;
                    Label_SPC.Visible = false;
                    TB_HPIV.Enabled = true;
                    SetMaskSize(TB_EVTotal.Size, "000");
                    TB_SPDEV.Enabled = TB_SPDIV.Enabled = true;
                    break;
            }

            void SetMaskSize(Size s, string Mask)
            {
                foreach (var ctrl in MT_EVs)
                {
                    ctrl.Size = s;
                    ctrl.Mask = Mask;
                }
            }
        }

        public void InitializeDataSources()
        {
            CB_HPType.InitializeBinding();
            CB_HPType.DataSource = Util.GetCBList(GameInfo.Strings.types.Skip(1).Take(16).ToArray(), null);
        }
    }
}
