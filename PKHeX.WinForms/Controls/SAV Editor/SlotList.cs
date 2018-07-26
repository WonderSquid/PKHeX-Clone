﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class SlotList : UserControl, ISlotViewer<PictureBox>
    {
        private static readonly string[] names = Enum.GetNames(typeof(StorageSlotType));
        private readonly LabelType[] Labels = new LabelType[names.Length];
        private readonly List<PictureBox> slots = new List<PictureBox>();
        private List<StorageSlotOffset> SlotOffsets = new List<StorageSlotOffset>();
        public int SlotCount { get; private set; }
        public SlotChangeManager M { get; set; }

        public SlotList()
        {
            InitializeComponent();
            AddLabels();
        }

        /// <summary>
        /// Initializes the extra slot viewers with a list of offsets and sets up event handling.
        /// </summary>
        /// <param name="list">Extra slots to show</param>
        /// <param name="enableDragDropContext">Events to set up</param>
        /// <remarks>Uses an object pool for viewers (only generates as needed)</remarks>
        public void Initialize(List<StorageSlotOffset> list, Action<Control> enableDragDropContext)
        {
            SlotOffsets = list;
            LoadSlots(list.Count, enableDragDropContext);
        }

        /// <summary>
        /// Hides all slots from the <see cref="SlotList"/>.
        /// </summary>
        public void HideAllSlots() => LoadSlots(0, null);

        public SlotChange GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            return new SlotChange
            {
                Slot = GetSlot(view),
                Box = ViewIndex,
                Offset = GetSlotOffset(slot),
                Type = StorageSlotType.Misc,
                IsPartyFormat = GetSlotIsParty(slot),
                Editable = false,
                Parent = FindForm(),
            };
        }
        public IList<PictureBox> SlotPictureBoxes => slots;
        public int GetSlot(PictureBox sender) => slots.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);
        public int GetSlotOffset(int slot) => SlotOffsets[slot].Offset;
        public bool GetSlotIsParty(int slot) => false;
        public int ViewIndex { get; set; } = -1;

        private IEnumerable<PictureBox> LoadSlots(int after, Action<Control> enableDragDropContext)
        {
            var generated = new List<PictureBox>();
            int before = SlotCount;
            SlotCount = after;
            int diff = after - before;
            if (diff > 0)
            {
                AddSlots(diff);
                for (int i = before; i < after; i++)
                {
                    var slot = slots[i];
                    enableDragDropContext(slot);
                    FLP_Slots.Controls.Add(slot);
                    FLP_Slots.SetFlowBreak(slot, true);
                    generated.Add(slot);
                }
            }
            else
            {
                for (int i = before - 1; i >= after; i--)
                    FLP_Slots.Controls.Remove(slots[i]);
            }
            SetLabelVisibility();
            return generated;
        }

        private void AddSlots(int count)
        {
            for (int i = 0; i < count; i++)
                slots.Add(GetPictureBox(i));
        }
        private const int PadPixels = 2;
        private const int SlotWidth = 40;
        private const int SlotHeight = 30;
        private static PictureBox GetPictureBox(int index)
        {
            return new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = SlotWidth + 2,
                Height = SlotHeight + 2,
                AllowDrop = true,
                Margin = new Padding(PadPixels),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Name = $"bpkm{index}",
            };
        }

        private class LabelType : Label
        {
            public StorageSlotType Type;
        }
        private void AddLabels()
        {
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                Enum.TryParse<StorageSlotType>(name, out var val);
                var label = new LabelType
                {
                    Name = $"L_{name}",
                    Text = name,
                    Type = val,
                    AutoSize = true,
                    Visible = false,
                };
                Labels[i] = label;
                FLP_Slots.Controls.Add(label);
                FLP_Slots.SetFlowBreak(label, true);
            }
        }
        private void SetLabelVisibility()
        {
            foreach (var l in Labels)
            {
                int index = SlotOffsets.FindIndex(z => z.Type == l.Type);
                if (index < 0)
                {
                    l.Visible = false;
                    continue;
                }
                int pos = FLP_Slots.Controls.IndexOf(slots[index]);
                if (pos > FLP_Slots.Controls.IndexOf(l))
                    pos--;
                FLP_Slots.Controls.SetChildIndex(l, pos);
                l.Visible = true;
            }
        }
    }
}
