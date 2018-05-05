﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class SAVEditor : UserControl
    {
        public SaveFile SAV;
        public readonly PictureBox[] SlotPictureBoxes;
        public readonly SlotChangeManager M;
        public readonly Stack<SlotChange> UndoStack = new Stack<SlotChange>();
        public readonly Stack<SlotChange> RedoStack = new Stack<SlotChange>();
        public readonly ContextMenuSAV menu = new ContextMenuSAV();
        public readonly ContextMenuStrip SortMenu;

        public bool HaX;
        public bool ModifyPKM;
        public ToolStripMenuItem Menu_Redo;
        public ToolStripMenuItem Menu_Undo;
        private bool FieldsLoaded;
        public PKMEditor PKME_Tabs;

        public bool FlagIllegal
        {
            get => Box.FlagIllegal;
            set
            {
                Box.FlagIllegal = value && !HaX;
                ReloadSlots();
            }
        }
        public void ReloadSlots()
        {
            UpdateBoxViewers(all: true);
            ResetNonBoxSlots();
        }

        public SAVEditor()
        {
            var z = Task.Run(() => SaveUtil.GetBlankSAV(GameVersion.US, "PKHeX"));
            InitializeComponent();
            var SupplementarySlots = new[]
            {
                ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                bbpkx1, bbpkx2, bbpkx3, bbpkx4, bbpkx5, bbpkx6,

                dcpkx1, dcpkx2
            };
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            SAV = z.Result;
            Box.Setup(M = new SlotChangeManager(this));
            SL_Extra.M = M;
            foreach (PictureBox pb in SupplementarySlots)
            {
                InitializeDragDrop(pb);
            }
            foreach (TabPage tab in tabBoxMulti.TabPages)
                tab.AllowDrop = true;

            Box.SlotPictureBoxes.AddRange(SupplementarySlots);
            SlotPictureBoxes = Box.SlotPictureBoxes.ToArray();
            foreach (PictureBox pb in SlotPictureBoxes)
                pb.ContextMenuStrip = menu.mnuVSD;

            GB_Daycare.Click += SwitchDaycare;
            FLP_SAVtools.Scroll += WinFormsUtil.PanelScroll;
            SortMenu = this.GetSortStrip();
        }
        private void InitializeDragDrop(Control pb)
        {
            pb.MouseEnter += M.MouseEnter;
            pb.MouseLeave += M.MouseLeave;
            pb.MouseClick += M.MouseClick;
            pb.MouseMove += BoxSlot_MouseMove;
            pb.MouseDown += M.MouseDown;
            pb.MouseUp += M.MouseUp;

            pb.DragEnter += M.DragEnter;
            pb.DragDrop += BoxSlot_DragDrop;
            pb.QueryContinueDrag += M.QueryContinueDrag;
            pb.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            pb.AllowDrop = true;
            pb.ContextMenuStrip = menu.mnuVSD;
        }

        /// <summary>Occurs when the Control Collection requests a cloning operation to the current box.</summary>
        public event EventHandler RequestCloneData;
        /// <summary>Occurs when the Control Collection requests a save to be reloaded.</summary>
        public event EventHandler RequestReloadSave;

        public Cursor GetDefaultCursor => DefaultCursor;
        private Image GetSprite(PKM p, int slot) => p.Sprite(SAV, Box.CurrentBox, slot, Box.FlagIllegal);

        public void EnableDragDrop(DragEventHandler enter, DragEventHandler drop)
        {
            AllowDrop = true;
            DragDrop += drop;
            foreach (TabPage tab in tabBoxMulti.TabPages)
            {
                tab.AllowDrop = true;
                tab.DragEnter += enter;
                tab.DragDrop += drop;
            }
            M.RequestExternalDragDrop += drop;
        }
        
        // Generic Subfunctions //
        public int GetPKMOffset(int slot, int box = -1)
        {
            if (slot < (int)SlotIndex.Party) // Box Slot
                return Box.GetOffset(slot, box);

            if (slot < (int)SlotIndex.BattleBox) // Party Slot
                return SAV.GetPartyOffset(slot - (int)SlotIndex.Party);
            if (slot < (int)SlotIndex.Daycare) // Battle Box Slot
                return SAV.BattleBox + (slot - (int)SlotIndex.BattleBox) * SAV.SIZE_STORED;
            if (slot < (int)SlotIndex.GTS) // Daycare
                return SAV.GetDaycareSlotOffset(SAV.DaycareIndex, slot - (int)SlotIndex.Daycare);

            slot -= 30+6+6+2;
            return SL_Extra.GetSlotOffset(slot);
        }
        public int GetSlot(object sender)
        {
            int slot = Array.IndexOf(SlotPictureBoxes, WinFormsUtil.GetUnderlyingControl(sender));
            if (slot < 0) // check extra slots
                slot = SL_Extra.GetSlot(sender) + SlotPictureBoxes.Length;
            return slot;
        }

        public int SwapBoxesViewer(int viewBox)
        {
            int mainBox = Box.CurrentBox;
            Box.CurrentBox = viewBox;
            return mainBox;
        }
        public void UpdateBoxViewers(bool all = false)
        {
            foreach (var v in M.Boxes.Where(v => v.CurrentBox == Box.CurrentBox || all))
            {
                v.FlagIllegal = Box.FlagIllegal;
                v.ResetSlots();
            }
        }
        public void SetPKMBoxes()
        {
            if (SAV.HasBox)
                Box.ResetSlots();

            ResetNonBoxSlots();

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (M?.ColorizedSlot >= (int)SlotIndex.Party)
                SlotPictureBoxes[M.ColorizedSlot].BackgroundImage = M.ColorizedColor;
        }
        private void ResetNonBoxSlots()
        {
            ResetParty();
            ResetBattleBox();
            ResetDaycare();
            ResetMiscSlots();
        }
        private void ResetMiscSlots()
        {
            var slots = SL_Extra.SlotPictureBoxes;
            for (int i = 0; i < SL_Extra.SlotCount; i++)
                GetSlotFiller(SL_Extra.GetSlotOffset(i), slots[i]);
        }
        private void ResetParty()
        {
            if (!SAV.HasParty)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.GetPartyOffset(i), SlotPictureBoxes[i + (int)SlotIndex.Party]);
        }
        private void ResetBattleBox()
        {
            if (!SAV.HasBattleBox)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.BattleBox + SAV.SIZE_STORED * i, SlotPictureBoxes[i + (int)SlotIndex.BattleBox]);
        }
        private void ResetDaycare()
        {
            if (!SAV.HasDaycare)
                return;

            Label[] L_SlotOccupied = {L_DC1, L_DC2};
            TextBox[] TB_SlotEXP = {TB_Daycare1XP, TB_Daycare2XP};
            Label[] L_SlotEXP = {L_XP1, L_XP2};

            for (int i = 0; i < 2; i++)
            {
                var pb = SlotPictureBoxes[i + (int)SlotIndex.Daycare];
                GetSlotFiller(SAV.GetDaycareSlotOffset(SAV.DaycareIndex, i), pb);
                uint? exp = SAV.GetDaycareEXP(SAV.DaycareIndex, i);
                TB_SlotEXP[i].Visible = L_SlotEXP[i].Visible = exp != null;
                TB_SlotEXP[i].Text = exp.ToString();
                bool? occ = SAV.IsDaycareOccupied(SAV.DaycareIndex, i);
                L_SlotOccupied[i].Visible = occ != null;
                if (occ == true) // If Occupied
                    L_SlotOccupied[i].Text = $"{i + 1}: ✓";
                else
                {
                    L_SlotOccupied[i].Text = $"{i + 1}: ✘";
                    pb.Image = ImageUtil.ChangeOpacity(pb.Image, 0.6);
                }
            }

            bool? egg = SAV.IsDaycareHasEgg(SAV.DaycareIndex);
            DayCare_HasEgg.Visible = egg != null;
            DayCare_HasEgg.Checked = egg == true;

            var seed = SAV.GetDaycareRNGSeed(SAV.DaycareIndex);
            if (seed != null)
            {
                TB_RNGSeed.MaxLength = SAV.DaycareSeedSize;
                TB_RNGSeed.Text = seed;
            }
            L_DaycareSeed.Visible = TB_RNGSeed.Visible = seed != null;
        }
        public void SetParty()
        {
            // Refresh slots
            if (SAV.HasParty)
            {
                var party = SAV.PartyData;
                for (int i = 0; i < party.Count; i++)
                    SlotPictureBoxes[i + (int)SlotIndex.Party].Image = GetSprite(party[i], i + (int)SlotIndex.Party);
                for (int i = party.Count; i < 6; i++)
                    SlotPictureBoxes[i + (int)SlotIndex.Party].Image = null;
            }
            if (SAV.HasBattleBox)
            {
                var battle = SAV.BattleBoxData;
                for (int i = 0; i < battle.Count; i++)
                    SlotPictureBoxes[i + (int)SlotIndex.BattleBox].Image = GetSprite(battle[i], i + (int)SlotIndex.BattleBox);
                for (int i = battle.Count; i < 6; i++)
                    SlotPictureBoxes[i + (int)SlotIndex.BattleBox].Image = null;
            }
        }
        public void ClickUndo()
        {
            if (!UndoStack.Any())
                return;

            SlotChange change = UndoStack.Pop();
            if (change.Slot >= (int)SlotIndex.Party)
                return;

            RedoStack.Push(new SlotChange
            {
                Slot = change.Slot,
                Box = change.Box,
                Offset = change.Offset,
                PKM = SAV.GetStoredSlot(change.Offset)
            });
            UndoSlotChange(change);
            M.SetColor(change.Box, change.Slot, Resources.slotSet);
        }
        public void ClickRedo()
        {
            if (!RedoStack.Any())
                return;

            SlotChange change = RedoStack.Pop();
            if (change.Slot >= (int)SlotIndex.Party)
                return;

            UndoStack.Push(new SlotChange
            {
                Slot = change.Slot,
                Box = change.Box,
                Offset = change.Offset,
                PKM = SAV.GetStoredSlot(change.Offset)
            });
            UndoSlotChange(change);
            M.SetColor(change.Box, change.Slot, Resources.slotSet);
        }
        public void SetClonesToBox(PKM pk)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
                    string.Format(MsgSaveBoxCloneFromTabs, Box.CurrentBoxName)) != DialogResult.Yes)
                return;

            int slotSkipped = 0;
            for (int i = 0; i < SAV.BoxSlotCount; i++) // set to every slot in box
            {
                if (SAV.IsSlotLocked(Box.CurrentBox, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(pk, GetPKMOffset(i));
                Box.SetSlotFiller(pk, Box.CurrentBox, i);
            }

            if (slotSkipped > 0)
                WinFormsUtil.Alert(string.Format(MsgSaveBoxImportSkippedLocked, slotSkipped));

            UpdateBoxViewers();
        }
        public void ClickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control | Keys.Alt: ClickClone(sender, e); break;
                default:
                    menu.OmniClick(sender, e, ModifierKeys);
                    break;
            }
        }

        private void UndoSlotChange(SlotChange change)
        {
            int box = change.Box;
            int slot = change.Slot;
            int offset = change.Offset;
            PKM pk = change.PKM;

            if (Box.CurrentBox != change.Box)
                Box.CurrentBox = change.Box;
            SAV.SetStoredSlot(pk, offset);
            Box.SetSlotFiller(pk, box, slot);
            M?.SetColor(box, slot, Resources.slotSet);

            if (Menu_Undo != null)
                Menu_Undo.Enabled = UndoStack.Any();
            if (Menu_Redo != null)
                Menu_Redo.Enabled = RedoStack.Any();

            SystemSounds.Asterisk.Play();
        }
        private void GetSlotFiller(int offset, PictureBox pb)
        {
            if (!SAV.IsPKMPresent(offset))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            PKM p = SAV.GetStoredSlot(offset);
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }

            int slot = GetSlot(pb);
            pb.Image = GetSprite(p, slot);
            pb.BackColor = Color.Transparent;
        }

        #region Box Manipulation
        private void ClickBoxSort(object sender, MouseEventArgs e)
        {
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!tabBoxMulti.GetTabRect(tabBoxMulti.SelectedIndex).Contains(PointToClient(MousePosition)))
                return;
            if (!e.Button.HasFlag(MouseButtons.Right))
            {
                if (ModifierKeys.HasFlag(Keys.Alt))
                    ((ToolStripMenuItem)SortMenu.Items[0]).DropDownItems[0].PerformClick(); // Clear
                else if (ModifierKeys.HasFlag(Keys.Control))
                    ((ToolStripMenuItem)SortMenu.Items[1]).DropDownItems[0].PerformClick(); // Sort
                return;
            }
            var pt = Tab_Box.PointToScreen(new Point(0, 0));
            SortMenu.Show(pt);
        }
        public void ClearAll(Func<PKM, bool> criteria)
        {
            if (!CanManipulateRegion(0, SAV.BoxCount - 1, MsgSaveBoxClearAll, MsgSaveBoxClearAllFailBattle))
                return;
            SAV.ClearBoxes(deleteCriteria: criteria);
            FinishBoxManipulation(MsgSaveBoxClearAllSuccess, true);
        }
        public void ClearCurrent(Func<PKM, bool> criteria)
        {
            if (!CanManipulateRegion(Box.CurrentBox, Box.CurrentBox, MsgSaveBoxClearCurrent, MsgSaveBoxClearCurrentFailBattle))
                return;
            SAV.ClearBoxes(Box.CurrentBox, Box.CurrentBox, criteria);
            FinishBoxManipulation(MsgSaveBoxClearCurrentSuccess, false);
        }
        public void SortAll(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter, bool reverse)
        {
            if (!CanManipulateRegion(0, SAV.BoxCount - 1, MsgSaveBoxSortAll, MsgSaveBoxSortAllFailBattle))
                return;
            SAV.SortBoxes(sortMethod: sorter, reverse: reverse);
            FinishBoxManipulation(MsgSaveBoxSortAllSuccess, true);
        }
        public void SortCurrent(Func<IEnumerable<PKM>, IEnumerable<PKM>> sorter, bool reverse)
        {
            if (!CanManipulateRegion(Box.CurrentBox, Box.CurrentBox, MsgSaveBoxSortCurrent, MsgSaveBoxSortCurrentFailBattle))
                return;
            SAV.SortBoxes(Box.CurrentBox, Box.CurrentBox, sorter, reverse: reverse);
            FinishBoxManipulation(MsgSaveBoxSortCurrentSuccess, false);
        }
        public void ModifyAll(Action<PKM> action)
        {
            SAV.ModifyBoxes(action);
            FinishBoxManipulation(null, true);
            SystemSounds.Asterisk.Play();
        }
        public void ModifyCurrent(Action<PKM> action)
        {
            SAV.ModifyBoxes(action, Box.CurrentBox, Box.CurrentBox);
            FinishBoxManipulation(null, true);
            SystemSounds.Asterisk.Play();
        }
        private void FinishBoxManipulation(string message, bool all)
        {
            SetPKMBoxes();
            UpdateBoxViewers(all);
            if (message != null)
                WinFormsUtil.Alert(message);
        }
        private bool CanManipulateRegion(int start, int end, string prompt, string fail)
        {
            if (prompt != null && WinFormsUtil.Prompt(MessageBoxButtons.YesNo, prompt) != DialogResult.Yes)
                return false;
            if (!SAV.IsAnySlotLockedInBox(start, end))
                return true;
            if (fail != null)
                WinFormsUtil.Alert(fail);
            return false;
        }
        #endregion
        private void ClickBoxDouble(object sender, MouseEventArgs e)
        {
            if (tabBoxMulti.SelectedTab == Tab_SAV)
            {
                RequestReloadSave?.Invoke(sender, e);
                return;
            }
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!SAV.HasBox)
                return;
            if (ModifierKeys == Keys.Shift)
            {
                if (M.Boxes.Count > 1) // subview open
                {
                    // close all subviews
                    for (int i = 1; i < M.Boxes.Count; i++)
                        M.Boxes[i].ParentForm.Close();
                }
                new SAV_BoxList(this, M).Show();
                return;
            }
            if (M.Boxes.Count > 1) // subview open
            { var z = M.Boxes[1].ParentForm; z.CenterToForm(ParentForm); z.BringToFront(); return; }
            new SAV_BoxViewer(this, M).Show();
        }
        private void ClickClone(object sender, EventArgs e)
        {
            if (GetSlot(sender) >= (int)SlotIndex.Party)
                return; // only perform action if cloning to boxes
            RequestCloneData?.Invoke(sender, e);
        }
        private void UpdateSaveSlot(object sender, EventArgs e)
        {
            if (SAV.Version != GameVersion.BATREV)
                return;
            ((SAV4BR)SAV).CurrentSlot = WinFormsUtil.GetIndex(CB_SaveSlot);
            SetPKMBoxes();
        }
        private void UpdateStringSeed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            if (tb.Text.Length == 0)
            {
                tb.Undo();
                return;
            }

            string filterText = Util.GetOnlyHex(tb.Text);
            if (filterText.Length != tb.Text.Length)
            {
                WinFormsUtil.Alert(MsgProgramErrorExpectedHex, tb.Text);
                tb.Undo();
                return;
            }

            // Write final value back to the save
            if (tb == TB_RNGSeed)
            {
                var value = filterText.PadLeft(SAV.DaycareSeedSize, '0');
                SAV.SetDaycareRNGSeed(SAV.DaycareIndex, value);
                SAV.Edited = true;
            }
            else if (tb == TB_GameSync)
            {
                var value = filterText.PadLeft(SAV.GameSyncIDSize, '0');
                SAV.GameSyncID = value;
                SAV.Edited = true;
            }
            else if (SAV.Generation >= 6)
            {
                var value = Convert.ToUInt64(filterText, 16);
                if (tb == TB_Secure1)
                    SAV.Secure1 = value;
                else if (tb == TB_Secure2)
                    SAV.Secure2 = value;
                SAV.Edited = true;
            }
        }
        private void SwitchDaycare(object sender, EventArgs e)
        {
            if (!SAV.HasTwoDaycares) return;
            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveSwitchDaycareView,
                    string.Format(MsgSaveSwitchDaycareCurrent, SAV.DaycareIndex + 1)))
                // If ORAS, alter the daycare offset via toggle.
                SAV.DaycareIndex ^= 1;

            // Refresh Boxes
            SetPKMBoxes();
        }
        private void B_SaveBoxBin_Click(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
            { WinFormsUtil.Alert(MsgSaveBoxFailNone); return; }
            Box.SaveBoxBinary();
        }

        // Subfunction Save Buttons //
        private void B_OpenWondercards_Click(object sender, EventArgs e) => new SAV_Wondercard(SAV, sender as MysteryGift).ShowDialog();
        private void B_OpenPokepuffs_Click(object sender, EventArgs e) => new SAV_Pokepuff(SAV).ShowDialog();
        private void B_OpenPokeBeans_Click(object sender, EventArgs e) => new SAV_Pokebean(SAV).ShowDialog();
        private void B_OpenItemPouch_Click(object sender, EventArgs e) => new SAV_Inventory(SAV).ShowDialog();
        private void B_OpenBerryField_Click(object sender, EventArgs e) => new SAV_BerryFieldXY(SAV).ShowDialog();
        private void B_OpenPokeblocks_Click(object sender, EventArgs e) => new SAV_PokeBlockORAS(SAV).ShowDialog();
        private void B_OpenSuperTraining_Click(object sender, EventArgs e) => new SAV_SuperTrain(SAV).ShowDialog();
        private void B_OpenSecretBase_Click(object sender, EventArgs e) => new SAV_SecretBase(SAV).ShowDialog();
        private void B_CellsStickers_Click(object sender, EventArgs e) => new SAV_ZygardeCell(SAV).ShowDialog();
        private void B_LinkInfo_Click(object sender, EventArgs e) => new SAV_Link6(SAV).ShowDialog();
        private void B_Roamer_Click(object sender, EventArgs e) => new SAV_Roamer3(SAV).ShowDialog();
        private void B_OpenApricorn_Click(object sender, EventArgs e) => new SAV_Apricorn(SAV).ShowDialog();
        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            var form = SAV.Generation == 1 ? new SAV_EventReset1(SAV) as Form : new SAV_EventFlags(SAV);
            form.ShowDialog();
        }
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            new SAV_BoxLayout(SAV, Box.CurrentBox).ShowDialog();
            Box.ResetBoxNames(); // fix box names
            Box.ResetSlots(); // refresh box background
            UpdateBoxViewers(all: true); // update subviewers
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            if (SAV.Generation < 6)
                new SAV_SimpleTrainer(SAV).ShowDialog();
            else if (SAV.Generation == 6)
                new SAV_Trainer(SAV).ShowDialog();
            else if (SAV.Generation == 7)
                new SAV_Trainer7(SAV).ShowDialog();
            // Refresh conversion info
            PKMConverter.UpdateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender, SAV.Language);
        }
        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (SAV.ORAS)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6OPower, MsgSaveGen6OPowerCheatDesc);
                if (dr != DialogResult.Yes) return;
                new byte[]
                {
                    0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00,
                    0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x00, 0x00,
                }.CopyTo(SAV.Data, ((SAV6)SAV).OPower);
            }
            else if (SAV.XY)
                new SAV_OPower(SAV).ShowDialog();
        }
        private void B_OpenFriendSafari_Click(object sender, EventArgs e)
        {
            if (!SAV.XY)
                return;

            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6FriendSafari, MsgSaveGen6FriendSafariCheatDesc);
            if (dr != DialogResult.Yes) return;

            // Unlock + reveal all safari slots if friend data is present
            for (int i = 1; i < 101; i++)
                if (SAV.Data[0x1E7FF + 0x15 * i] != 0x00) // no friend data == 0x00
                    SAV.Data[0x1E7FF + 0x15 * i] = 0x3D;
            SAV.Edited = true;
        }
        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 1:
                case 2:
                    new SAV_SimplePokedex(SAV).ShowDialog(); break;
                case 3:
                    if (SAV.GameCube)
                        return;
                    new SAV_SimplePokedex(SAV).ShowDialog(); break;
                case 4:
                    if (SAV is SAV4BR)
                        return;
                    new SAV_Pokedex4(SAV).ShowDialog(); break;
                case 5:
                    new SAV_Pokedex5(SAV).ShowDialog(); break;
                case 6:
                    if (SAV.ORAS)
                        new SAV_PokedexORAS(SAV).ShowDialog();
                    else if (SAV.XY)
                        new SAV_PokedexXY(SAV).ShowDialog();
                    break;
                case 7:
                    if (SAV.SM || SAV.USUM)
                        new SAV_PokedexSM(SAV).ShowDialog();
                    break;
            }
        }
        private void B_OpenMiscEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 3:
                    new SAV_Misc3(SAV).ShowDialog(); break;
                case 4:
                    new SAV_Misc4(SAV).ShowDialog(); break;
                case 5:
                    new SAV_Misc5(SAV).ShowDialog(); break;
            }
        }
        private void B_OpenRTCEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 2:
                    WinFormsUtil.Alert(string.Format(MsgSaveGen2RTCResetPassword, ((SAV2) SAV).ResetKey)); break;
                case 3:
                    new SAV_RTC3(SAV).ShowDialog(); break;
            }
        }
        private void B_OpenHoneyTreeEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Version)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                    new SAV_HoneyTree(SAV).ShowDialog(); break;
            }
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6Passerby))
                return;
            var result = new List<string> {"PSS List"};
            string[] headers = { "PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby", };
            int offset = ((SAV6)SAV).PSS;
            for (int g = 0; g < 3; g++)
            {
                result.Add("----");
                result.Add(headers[g]);
                result.Add("----");
                // uint count = BitConverter.ToUInt32(savefile, offset + 0x4E20);
                int r_offset = offset;

                for (int i = 0; i < 100; i++)
                {
                    ulong unkn = BitConverter.ToUInt64(SAV.Data, r_offset);
                    if (unkn == 0) break; // No data present here
                    if (i > 0)
                        result.Add("");

                    string otname = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 8, 0x1A));
                    string message = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 0x22, 0x22));

                    // Trim terminated

                    // uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
                    // ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
                    // uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
                    // uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
                    byte region = SAV.Data[r_offset + 0x56];
                    byte country = SAV.Data[r_offset + 0x57];
                    byte game = SAV.Data[r_offset + 0x5A];
                    // ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
                    int favpkm = BitConverter.ToUInt16(SAV.Data, r_offset + 0x9C) & 0x7FF;
                    string gamename;
                    try { gamename = GameInfo.Strings.gamelist[game]; }
                    catch { gamename = "UNKNOWN GAME"; }

                    var cr = GameInfo.GetCountryRegionText(country, region, GameInfo.CurrentLanguage);
                    result.Add($"OT: {otname}");
                    result.Add($"Message: {message}");
                    result.Add($"Game: {gamename}");
                    result.Add($"Country: {cr.Item1}");
                    result.Add($"Region: {cr.Item2}");
                    result.Add($"Favorite: {GameInfo.Strings.specieslist[favpkm]}");

                    r_offset += 0xC8; // Advance to next entry
                }
                offset += 0x5000; // Advance to next block
            }
            Clipboard.SetText(string.Join(Environment.NewLine, result));
        }
        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            if (SAV.Generation == 6)
                new SAV_HallOfFame(SAV).ShowDialog();
            else if (SAV.Generation == 7)
                new SAV_HallOfFame7(SAV).ShowDialog();
        }
        private void B_CGearSkin_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 5)
                return; // can never be too safe
            new SAV_CGearSkin(SAV).ShowDialog();
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            byte[] jpeg = SAV.JPEGData;
            if (SAV.JPEGData == null)
            { WinFormsUtil.Alert(MsgSaveJPEGExportFail); return; }
            string filename = SAV.JPEGTitle + "'s picture";
            SaveFileDialog sfd = new SaveFileDialog { FileName = filename, Filter = "JPEG|*.jpeg" };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(sfd.FileName, jpeg);
        }
        private void ClickVerifyCHK(object sender, EventArgs e)
        {
            if (SAV.Edited) { WinFormsUtil.Alert(MsgSaveChecksumFailEdited); return; }

            if (SAV.ChecksumsValid) { WinFormsUtil.Alert(MsgSaveChecksumValid); return; }
            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveChecksumFailExport))
                Clipboard.SetText(SAV.ChecksumInfo);
        }

        // File I/O
        public bool GetBulkImportSettings(out bool clearAll, out bool? noSetb)
        {
            clearAll = false; noSetb = false;
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, MsgSaveBoxImportClear, MsgSaveBoxImportClearNo);
            if (dr == DialogResult.Cancel)
                return false;

            clearAll = dr == DialogResult.Yes;
            noSetb = GetPKMSetOverride();
            return true;
        }
        private bool? GetPKMSetOverride()
        {
            var yn = ModifyPKM ? MsgYes : MsgNo;
            DialogResult noSet = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                MsgSaveBoxImportModifyIntro,
                MsgSaveBoxImportModifyYes + Environment.NewLine +
                MsgSaveBoxImportModifyNo + Environment.NewLine +
                string.Format(MsgSaveBoxImportModifyCurrent, yn));
            return noSet == DialogResult.Yes ? true : (noSet == DialogResult.No ? (bool?)false : null);
        }
        private static bool IsFolderPath(out string path)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog() == DialogResult.OK;
            path = fbd.SelectedPath;
            return result;
        }

        public bool ExportSaveFile()
        {
            ValidateChildren();
            return WinFormsUtil.SaveSAVDialog(SAV, SAV.CurrentBox);
        }
        public bool ExportBackup()
        {
            if (!SAV.Exportable)
                return false;
            SaveFileDialog sfd = new SaveFileDialog
                { FileName = Util.CleanFileName(SAV.BAKName) };
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            string path = sfd.FileName;
            File.WriteAllBytes(path, SAV.BAK);
            WinFormsUtil.Alert(MsgSaveBackup, path);

            return true;
        }
        public bool IsPCBoxBin(int length) => PKX.IsPKM(length / SAV.SlotCount) || PKX.IsPKM(length / SAV.BoxSlotCount);
        public bool OpenPCBoxBin(byte[] input, out string c)
        {
            if (SAV.PCBinary.Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(0, SAV.BoxCount - 1))
                { c = MsgSaveBoxImportPCFailBattle; return false; }
                if (!SAV.SetPCBinary(input))
                { c = string.Format(MsgSaveCurrentGeneration, SAV.Generation); return false; }

                c = MsgSaveBoxImportPCBinary;
            }
            else if (SAV.GetBoxBinary(Box.CurrentBox).Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(Box.CurrentBox, Box.CurrentBox))
                { c = MsgSaveBoxImportBoxFailBattle; return false; }
                if (!SAV.SetBoxBinary(input, Box.CurrentBox))
                { c = string.Format(MsgSaveCurrentGeneration, SAV.Generation); return false; }

                c = MsgSaveBoxImportBoxBinary;
            }
            else
            {
                c = string.Format(MsgSaveCurrentGeneration, SAV.Generation);
                return false;
            }
            SetPKMBoxes();
            UpdateBoxViewers();
            return true;
        }
        public bool OpenBattleVideo(BattleVideo b, out string c)
        {
            if (b == null || SAV.Generation != b.Generation)
            {
                c = MsgSaveBoxImportVideoFailGeneration;
                return false;
            }

            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
                string.Format(MsgSaveBoxImportVideo, Box.CurrentBoxName), MsgSaveBoxImportOverwrite);
            if (prompt != DialogResult.Yes)
            {
                c = string.Empty;
                return false;
            }

            bool? noSetb = GetPKMSetOverride();
            PKM[] data = b.BattlePKMs;
            int offset = SAV.GetBoxOffset(Box.CurrentBox);
            int slotSkipped = 0;
            for (int i = 0; i < 24; i++)
            {
                if (SAV.IsSlotLocked(Box.CurrentBox, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(data[i], offset + i * SAV.SIZE_STORED, noSetb);
            }

            SetPKMBoxes();
            UpdateBoxViewers();

            c = slotSkipped > 0 ? string.Format(MsgSaveBoxImportSkippedLocked, slotSkipped) : MsgSaveBoxImportVideoSuccess;

            return true;
        }
        public bool DumpBoxes(out string result, string path = null, bool separate = false)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            Directory.CreateDirectory(path);

            SAV.DumpBoxes(path, out result, separate);
            return true;
        }
        public bool DumpBox(out string result, string path = null)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            Directory.CreateDirectory(path);

            SAV.DumpBox(path, out result, Box.CurrentBox);
            return true;
        }
        public bool LoadBoxes(out string result, string path = null)
        {
            result = string.Empty;
            if (!SAV.HasBox)
                return false;

            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            if (!Directory.Exists(path))
                return false;

            if (!GetBulkImportSettings(out bool clearAll, out bool? noSetb))
                return false;

            SAV.LoadBoxes(path, out result, Box.CurrentBox, clearAll, noSetb);
            SetPKMBoxes();
            UpdateBoxViewers();
            return true;
        }

        public bool ToggleInterface()
        {
            FieldsLoaded = false;

            ToggleViewReset();
            ToggleViewSubEditors(SAV);

            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= ToggleViewBox(SAV);
            int BoxTab = tabBoxMulti.TabPages.IndexOf(Tab_Box);
            WindowTranslationRequired |= ToggleViewParty(SAV, BoxTab);
            int PartyTab = tabBoxMulti.TabPages.IndexOf(Tab_PartyBattle);
            WindowTranslationRequired |= ToggleViewDaycare(SAV, BoxTab, PartyTab);
            SetPKMBoxes();   // Reload all of the PKX Windows

            ToggleViewMisc(SAV);

            FieldsLoaded = true;
            return WindowTranslationRequired;
        }
        private void ToggleViewReset()
        {
            // Close subforms that are save dependent
            foreach (var z in M.Boxes.Skip(1).ToArray())
                z.FindForm()?.Close();

            UndoStack.Clear();
            RedoStack.Clear();
            Box.M = M;
            Box.ResetBoxNames();   // Display the Box Names
            M.SetColor(-1, -1, null);
        }
        private bool ToggleViewBox(SaveFile sav)
        {
            if (!sav.HasBox)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Box))
                    tabBoxMulti.TabPages.Remove(Tab_Box);
                B_SaveBoxBin.Enabled = false;
                return false;
            }

            B_SaveBoxBin.Enabled = true;
            int startBox = !sav.Exportable ? 0 : sav.CurrentBox; // FF if BattleBox
            if (startBox > sav.BoxCount - 1) { tabBoxMulti.SelectedIndex = 1; Box.CurrentBox = 0; }
            else { tabBoxMulti.SelectedIndex = 0; Box.CurrentBox = startBox; }

            if (tabBoxMulti.TabPages.Contains(Tab_Box))
                return false;
            tabBoxMulti.TabPages.Insert(0, Tab_Box);
            return true;
        }
        private bool ToggleViewParty(SaveFile sav, int BoxTab)
        {
            if (!sav.HasParty)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_PartyBattle))
                    tabBoxMulti.TabPages.Remove(Tab_PartyBattle);
                return false;
            }

            PB_Locked.Visible = sav.HasBattleBox && sav.BattleBoxLocked;
            if (tabBoxMulti.TabPages.Contains(Tab_PartyBattle))
                return false;

            int index = BoxTab;
            if (index < 0)
                index = -1;
            tabBoxMulti.TabPages.Insert(index + 1, Tab_PartyBattle);
            return true;
        }
        private bool ToggleViewDaycare(SaveFile sav, int BoxTab, int PartyTab)
        {
            if (!sav.HasDaycare)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Other))
                    tabBoxMulti.TabPages.Remove(Tab_Other);
                return false;
            }

            SlotPictureBoxes[43].Visible = sav.Generation >= 2; // Second daycare slot
            if (tabBoxMulti.TabPages.Contains(Tab_Other))
                return false;

            int index = PartyTab;
            if (index < 0)
                index = BoxTab;
            if (index < 0)
                index = -1;
            tabBoxMulti.TabPages.Insert(index + 1, Tab_Other);
            return true;
        }
        private void ToggleViewSubEditors(SaveFile sav)
        {
            if (!sav.Exportable || sav is BulkStorage)
            {
                GB_SAVtools.Visible = false;
                B_JPEG.Visible = false;
                SL_Extra.HideAllSlots();
                return;
            }

            {
                PAN_BattleBox.Visible = L_BattleBox.Visible = L_ReadOnlyPBB.Visible = sav.HasBattleBox;
                GB_Daycare.Visible = sav.HasDaycare;
                B_OpenSecretBase.Enabled = sav.HasSecretBase;
                B_OpenPokepuffs.Enabled = sav.HasPuff;
                B_OpenPokeBeans.Enabled = sav.Generation == 7;
                B_CellsStickers.Enabled = sav.Generation == 7;
                B_OUTPasserby.Enabled = sav.HasPSS;
                B_OpenBoxLayout.Enabled = sav.HasNamableBoxes;
                B_OpenWondercards.Enabled = sav.HasWondercards;
                B_OpenSuperTraining.Enabled = sav.HasSuperTrain;
                B_OpenHallofFame.Enabled = sav.HasHoF;
                B_OpenOPowers.Enabled = sav.HasOPower;
                B_OpenPokedex.Enabled = sav.HasPokeDex;
                B_OpenBerryField.Enabled = sav.HasBerryField && sav.XY;
                B_OpenFriendSafari.Enabled = sav.XY;
                B_OpenPokeblocks.Enabled = sav.HasPokeBlock;
                B_JPEG.Visible = sav.HasJPEG;
                B_OpenEventFlags.Enabled = sav.HasEvents;
                B_OpenLinkInfo.Enabled = sav.HasLink;
                B_CGearSkin.Enabled = sav.Generation == 5;

                B_OpenTrainerInfo.Enabled = B_OpenItemPouch.Enabled = sav.HasParty; // Box RS
                B_OpenMiscEditor.Enabled = sav is SAV3 || sav is SAV4 || sav is SAV5;
                B_Roamer.Enabled = sav is SAV3;

                B_OpenHoneyTreeEditor.Enabled = sav.DP || sav.Pt;
                B_OpenApricorn.Enabled = sav.HGSS;
                B_OpenRTCEditor.Enabled = sav.RS || sav.E || sav.Generation == 2;
                B_OpenUGSEditor.Enabled = sav.DP || sav.Pt;
                B_FestivalPlaza.Enabled = sav.Generation == 7;
                B_MailBox.Enabled = sav is SAV2 || sav is SAV3 || sav is SAV4 || sav is SAV5;

                var slots = SL_Extra.Initialize(sav.GetExtraSlots(HaX), InitializeDragDrop);
                Box.SlotPictureBoxes.AddRange(slots);
            }
            GB_SAVtools.Visible = sav.Exportable && FLP_SAVtools.Controls.Cast<Control>().Any(c => c.Enabled);
            foreach (Control c in FLP_SAVtools.Controls.Cast<Control>())
                c.Visible = c.Enabled;
        }
        private void ToggleViewMisc(SaveFile sav)
        {
            // Generational Interface
            TB_Secure1.Visible = TB_Secure2.Visible = L_Secure1.Visible = L_Secure2.Visible = sav.Exportable && sav.Generation >= 6;
            TB_GameSync.Visible = L_GameSync.Visible = sav.Exportable && sav.Generation >= 6;
            B_VerifyCHK.Enabled = SAV.Exportable;

            if (sav.Version == GameVersion.BATREV)
            {
                L_SaveSlot.Visible = CB_SaveSlot.Visible = true;
                CB_SaveSlot.DisplayMember = nameof(ComboItem.Text); CB_SaveSlot.ValueMember = nameof(ComboItem.Value);
                CB_SaveSlot.DataSource = new BindingSource(((SAV4BR)sav).SaveSlots.Select(i => new ComboItem
                {
                    Text = ((SAV4BR)sav).SaveNames[i],
                    Value = i
                }).ToList(), null);
                CB_SaveSlot.SelectedValue = ((SAV4BR)sav).CurrentSlot;
            }
            else
                L_SaveSlot.Visible = CB_SaveSlot.Visible = false;

            switch (sav.Generation)
            {
                case 6:
                    TB_GameSync.Enabled = sav.GameSyncID != null;
                    TB_GameSync.MaxLength = sav.GameSyncIDSize;
                    TB_GameSync.Text = (sav.GameSyncID ?? 0.ToString()).PadLeft(sav.GameSyncIDSize, '0');
                    TB_Secure1.Text = sav.Secure1?.ToString("X16");
                    TB_Secure2.Text = sav.Secure2?.ToString("X16");
                    break;
                case 7:
                    TB_GameSync.Enabled = sav.GameSyncID != null;
                    TB_GameSync.MaxLength = sav.GameSyncIDSize;
                    TB_GameSync.Text = (sav.GameSyncID ?? 0.ToString()).PadLeft(sav.GameSyncIDSize, '0');
                    TB_Secure1.Text = sav.Secure1?.ToString("X16");
                    TB_Secure2.Text = sav.Secure2?.ToString("X16");
                    break;
            }
        }

        // DragDrop
        private void BoxSlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (M == null || M.DragActive)
                return;

            // Abort if there is no Pokemon in the given slot.
            PictureBox pb = (PictureBox)sender;
            if (pb.Image == null)
                return;
            int slot = GetSlot(pb);
            int box = slot >= (int)SlotIndex.Party ? -1 : Box.CurrentBox;
            if (SAV.IsSlotLocked(box, slot))
                return;

            bool encrypt = ModifierKeys == Keys.Control;
            M.HandleMovePKM(pb, slot, box, encrypt);
        }
        private void BoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            if (M == null)
                return;

            PictureBox pb = (PictureBox)sender;
            int slot = GetSlot(pb);
            int box = slot >= (int)SlotIndex.Party ? -1 : Box.CurrentBox;
            if (SAV.IsSlotLocked(box, slot) || slot >= (int)SlotIndex.BattleBox)
            {
                SystemSounds.Asterisk.Play();
                e.Effect = DragDropEffects.Copy;
                M.DragInfo.Reset();
                return;
            }

            bool overwrite = ModifierKeys == Keys.Alt;
            bool clone = ModifierKeys == Keys.Control;
            M.DragInfo.Destination.Parent = FindForm();
            M.DragInfo.Destination.Slot = GetSlot(sender);
            M.DragInfo.Destination.Box = M.DragInfo.Destination.IsParty ? -1 : Box.CurrentBox;
            M.HandleDropPKM(sender, e, overwrite, clone);
        }
        private void MultiDragOver(object sender, DragEventArgs e)
        {
            // iterate over all tabs to see if a tab switch should occur when drag/dropping
            Point pt = tabBoxMulti.PointToClient(new Point(e.X, e.Y));
            for (int i = 0; i < tabBoxMulti.TabCount; i++)
            {
                if (tabBoxMulti.SelectedIndex == i || !tabBoxMulti.GetTabRect(i).Contains(pt))
                    continue;
                tabBoxMulti.SelectedIndex = i;
                return;
            }
        }
        private void ClickShowdownExportParty(object sender, EventArgs e)
        {
            try
            {
                var str = string.Join(Environment.NewLine + Environment.NewLine, SAV.PartyData.Select(pk => pk.ShowdownText));
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
            }
            catch { }
            WinFormsUtil.Alert(MsgSimulatorExportParty);
        }
        private void ClickShowdownExportBattleBox(object sender, EventArgs e)
        {
            try
            {
                var str = string.Join(Environment.NewLine + Environment.NewLine, SAV.BattleBoxData.Select(pk => pk.ShowdownText));
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
            }
            catch { }
            WinFormsUtil.Alert(MsgSimulatorExportBattleBox);
        }

        private void B_OpenUGSEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Version)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                    new SAV_Underground(SAV).ShowDialog(); break;
            }
        }
        private void B_FestivalPlaza_Click(object sender, EventArgs e)
        {
            if (SAV.Generation == 7)
                new SAV_FestivalPlaza(SAV).ShowDialog();
        }
        private void B_MailBox_Click(object sender, EventArgs e)
        {
            new SAV_MailBox(SAV).ShowDialog();
            ResetParty();
        }

        private enum SlotIndex
        {
            Box = 0,        // -> 29 [30]
            Party = 30,     // -> 35 [6]
            BattleBox = 36, // -> 41 [6]
            Daycare = 42,
            GTS = 44,
            Fused = 45,
            SUBE = 46,
        }
    }
}
