﻿using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public static class MysteryUtil
    {
        /// <summary>
        /// Gets a description of the <see cref="MysteryGift"/> using the current default string data.
        /// </summary>
        /// <param name="gift">Gift data to parse</param>
        /// <returns>List of lines</returns>
        public static IEnumerable<string> GetDescription(this MysteryGift gift) => gift.GetDescription(GameInfo.Strings);

        /// <summary>
        /// Gets a description of the <see cref="MysteryGift"/> using provided string data.
        /// </summary>
        /// <param name="gift">Gift data to parse</param>
        /// <param name="strings">String data to use</param>
        /// <returns>List of lines</returns>
        public static IEnumerable<string> GetDescription(this MysteryGift gift, IBasicStrings strings)
        {
            if (gift.Empty)
                return new[] { MsgMysteryGiftSlotEmpty };

            var result = new List<string> { gift.CardHeader };
            if (gift.IsItem)
            {
                AddLinesItem(gift, strings, result);
            }
            else if (gift.IsPokémon)
            {
                try
                {
                    AddLinesPKM(gift, strings, result);
                }
                catch { result.Add(MsgMysteryGiftParseFail); }
            }
            else if (gift.IsBP)
            {
                result.Add($"BP: {gift.BP}");
            }
            else if (gift.IsBean)
            {
                result.Add($"Bean ID: {gift.Bean}");
                result.Add($"Quantity: {gift.Quantity}");
            }
            else { result.Add(MsgMysteryGiftParseTypeUnknown); }
            if (gift is WC7 w7)
            {
                result.Add($"Repeatable: {w7.GiftRepeatable}");
                result.Add($"Collected: {w7.GiftUsed}");
                result.Add($"Once Per Day: {w7.GiftOncePerDay}");
            }
            return result;
        }

        private static void AddLinesItem(MysteryGift gift, IBasicStrings strings, ICollection<string> result)
        {
            result.Add($"Item: {strings.Item[gift.ItemID]} (Quantity: {gift.Quantity})");
            if (gift is WC7 wc7)
            {
                var ind = 1;
                while (wc7.GetItem(ind) != 0)
                {
                    result.Add($"Item: {strings.Item[wc7.GetItem(ind)]} (Quantity: {wc7.GetQuantity(ind)})");
                    ind++;
                }
            }
        }

        private static void AddLinesPKM(MysteryGift gift, IBasicStrings strings, ICollection<string> result)
        {
            int TID7() => (int)((uint)(gift.TID | (gift.SID << 16)) % 1000000);
            int SID7() => (int)((uint)(gift.TID | (gift.SID << 16)) / 1000000);
            var id = gift.Format < 7 ? $"{gift.TID:D5}/{gift.SID:D5}" : $"[{SID7():D4}]{TID7():D6}";

            var first =
                $"{strings.Species[gift.Species]} @ {strings.Item[gift.HeldItem]}  --- "
                + (gift.IsEgg ? strings.EggName : $"{gift.OT_Name} - {id}");
            result.Add(first);
            result.Add(string.Join(" / ", gift.Moves.Select(z => strings.Move[z])));

            if (gift is WC7 wc7)
            {
                var addItem = wc7.AdditionalItem;
                if (addItem != 0)
                    result.Add($"+ {strings.Item[addItem]}");
            }
        }

        public static bool IsCardCompatible(this MysteryGift g, SaveFile SAV, out string message)
        {
            if (g.Format != SAV.Generation)
            {
                message = MsgMysteryGiftSlotSpecialReject;
                return false;
            }

            if (g is WC6 && g.CardID == 2048 && g.ItemID == 726) // Eon Ticket (OR/AS)
            {
                if (!SAV.ORAS)
                {
                    message = MsgMysteryGiftSlotSpecialReject;
                    return false;
                }

                // Set the special recieved data
                BitConverter.GetBytes(WC6.EonTicketConst).CopyTo(SAV.Data, ((SAV6)SAV).EonTicket);
            }

            message = null;
            return true;
        }
    }
}
