using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SCBlock"/> metadata for <see cref="GameVersion.PLA"/>
/// </summary>
public static class BlankBlocks8a
{
    /// <summary>
    /// Creates a blank <see cref="SCBlock"/> array for <see cref="GameVersion.PLA"/>
    /// </summary>
    public static SCBlock[] GetBlankBlocks() => SCBlockUtil.GetBlankBlockArray(DefaultChunkSizes);

    private static ReadOnlySpan<uint> DefaultChunkSizes =>
    [
        0x00EF4BAE, 0x00140, 0x017C3CBB, 0x00001, 0x02168706, 0x1E460, 0x022A2253, 0x00001,
        0x024C8CF3, 0x00004, 0x033D60DA, 0x00004, 0x0493AF74, 0x00004, 0x04AE4181, 0x00001,
        0x04EABECF, 0x00004, 0x050E9D63, 0x00004, 0x05E7EBEB, 0x02EE0, 0x062AF6C2, 0x00020,
        0x069EAD42, 0x00004, 0x070F34AD, 0x00020, 0x0821C372, 0x00004, 0x0A40A680, 0x00004,
        0x0A49F0EF, 0x01F40, 0x0B3C5217, 0x00004, 0x0BFDEBA1, 0x00004, 0x0E6246BE, 0x00004,
        0x0F3AD63C, 0x00004, 0x10A052BA, 0x00008, 0x10A0546D, 0x00008, 0x10D148DE, 0x00004,
        0x111933CD, 0x00004, 0x11B37EC9, 0x000C8, 0x13A0E18B, 0x00004, 0x17833C07, 0x00004,
        0x17EB7C4D, 0x00004, 0x180A4E9F, 0x00004, 0x19722C89, 0x00440, 0x1B1E3D8B, 0x00004,
        0x1B5E0528, 0x00001, 0x1B65ABFD, 0x00004, 0x1BF70FCB, 0x00004, 0x1D482A63, 0x00004,
        0x1E0F1BA3, 0x00190, 0x203A7F34, 0x00008, 0x2085565F, 0x00001, 0x20855812, 0x00001,
        0x208559C5, 0x00001, 0x208D511F, 0x00004, 0x2123FE4A, 0x00004, 0x2137FAFF, 0x00004,
        0x22540FF0, 0x00004, 0x22DEF108, 0x00004, 0x23AA6AE3, 0x00004, 0x24E0D195, 0x002F2,
        0x250F3C75, 0x00004, 0x267DD9DA, 0x00070, 0x2728E588, 0x00004, 0x27931A29, 0x00004,
        0x27986623, 0x00004, 0x279EA6CD, 0x00004, 0x27C9C8C2, 0x00100, 0x2846B7DB, 0x00004,
        0x2969F5EB, 0x00004, 0x296C9DB8, 0x00004, 0x2985FE5D, 0x008D4, 0x298D9297, 0x00004,
        0x29FB8D78, 0x00004, 0x2BBF9423, 0x00001, 0x2BBF9789, 0x00001, 0x2BBF9CA2, 0x00001,
        0x2BBF9E55, 0x00001, 0x2C0B9BF3, 0x00004, 0x2C24C5F2, 0x00020, 0x2DBE7204, 0x04B00,
        0x2EB1B190, 0x00020, 0x2F85E20D, 0x00004, 0x305FD79A, 0x00008, 0x30967638, 0x00001,
        0x3096799E, 0x00001, 0x30967B51, 0x00001, 0x30B884F9, 0x00004, 0x3279D927, 0x00004,
        0x35BDC76F, 0x00001, 0x35BDC922, 0x00001, 0x3745DA43, 0x00004, 0x37B18444, 0x00004,
        0x385F9860, 0x00004, 0x388E378D, 0x00004, 0x3AAF5E5E, 0x00004, 0x3ADB8A98, 0x000C8,
        0x3B4D705E, 0x00001, 0x3B956C1A, 0x00004, 0x3BFC2C3C, 0x000D0, 0x3C4AADD3, 0x00008,
        0x3EBEE1A7, 0x00004, 0x3F5225A0, 0x00004, 0x3F7CC8A4, 0x00004, 0x3F8120BA, 0x00002,
        0x402FAC1D, 0x00001, 0x4033C7DB, 0x00001, 0x40892A39, 0x00004, 0x40CC5A21, 0x00002,
        0x40E13871, 0x00004, 0x41309084, 0x00001, 0x416A4820, 0x00004, 0x416A49D3, 0x00004,
        0x416A4D39, 0x00004, 0x416A5252, 0x00004, 0x416A5405, 0x00004, 0x431018F0, 0x00004,
        0x43749288, 0x00010, 0x444D8A2C, 0x00001, 0x44552BFF, 0x00004, 0x451E1BAF, 0x00020,
        0x457495AE, 0x00010, 0x45851092, 0x00064, 0x4618E7E4, 0x00004, 0x46459F4E, 0x00004,
        0x46749741, 0x00010, 0x477498D4, 0x00010, 0x47E1CEAB, 0x54600, 0x481DB963, 0x00001,
        0x481DBCC9, 0x00001, 0x481DBE7C, 0x00001, 0x481DC02F, 0x00001, 0x481DCA61, 0x00001,
        0x4820CC20, 0x00001, 0x4820CDD3, 0x00001, 0x4820CF86, 0x00001, 0x4820D139, 0x00001,
        0x4820D2EC, 0x00001, 0x4820D49F, 0x00001, 0x4820D805, 0x00001, 0x4820E3EA, 0x00001,
        0x4820E59D, 0x00001, 0x4823EE28, 0x00001, 0x4823EFDB, 0x00001, 0x4823F18E, 0x00001,
        0x4823F341, 0x00001, 0x4823F4F4, 0x00001, 0x4823F6A7, 0x00001, 0x4826224C, 0x00001,
        0x48262918, 0x00001, 0x48262ACB, 0x00001, 0x48262C7E, 0x00001, 0x48262E31, 0x00001,
        0x48263197, 0x00001, 0x4826334A, 0x00001, 0x482634FD, 0x00001, 0x48749A67, 0x00010,
        0x48CE01F7, 0x000FC, 0x48DDB755, 0x00006, 0x4918E303, 0x00001, 0x49749BFA, 0x00010,
        0x4A6B888D, 0x00004, 0x4A749D8D, 0x00010, 0x4AA3F543, 0x00004, 0x4AA3F6F6, 0x00004,
        0x4AAF7FBE, 0x00004, 0x4BD70B32, 0x041A0, 0x4C5C85AB, 0x00004, 0x4CF1F5D3, 0x00004,
        0x4D7EADDD, 0x00004, 0x4DB28157, 0x00004, 0x4EB3ECBB, 0x00004, 0x4EE2B115, 0x00008,
        0x4FBDB5FF, 0x00008, 0x500164D0, 0x00004, 0x50016683, 0x00004, 0x500687A1, 0x00004,
        0x509A1AC8, 0x00004, 0x509A1C7B, 0x00004, 0x509A1FE1, 0x00004, 0x50FE632A, 0x00004,
        0x511622B3, 0x88040, 0x5297D400, 0x00001, 0x5297D766, 0x00001, 0x5297D919, 0x00001,
        0x5297DACC, 0x00001, 0x5297DC7F, 0x00001, 0x5297EBCA, 0x00001, 0x5297ED7D, 0x00001,
        0x529AE870, 0x00001, 0x529AEA23, 0x00001, 0x529AF608, 0x00001, 0x529AF7BB, 0x00001,
        0x529AF96E, 0x00001, 0x529AFB21, 0x00001, 0x529AFCD4, 0x00001, 0x529AFE87, 0x00001,
        0x529B003A, 0x00001, 0x529B01ED, 0x00001, 0x52A96788, 0x00001, 0x52A9693B, 0x00001,
        0x52A96AEE, 0x00001, 0x52A96CA1, 0x00001, 0x52A96E54, 0x00001, 0x52A97007, 0x00001,
        0x52A971BA, 0x00001, 0x52A9736D, 0x00001, 0x52AC71C6, 0x00001, 0x52AC7379, 0x00001,
        0x52AC7BF8, 0x00001, 0x52AC7DAB, 0x00001, 0x52AC7F5E, 0x00001, 0x52AC8111, 0x00001,
        0x52AC82C4, 0x00001, 0x52AC8477, 0x00001, 0x52AC862A, 0x00001, 0x52AC87DD, 0x00001,
        0x52AF8D02, 0x00001, 0x52AF8EB5, 0x00001, 0x52AF9068, 0x00001, 0x52AF921B, 0x00001,
        0x52AF93CE, 0x00001, 0x52AF9581, 0x00001, 0x52AF9734, 0x00001, 0x52AF98E7, 0x00001,
        0x52AF9C4D, 0x00001, 0x52B27C10, 0x00001, 0x52B27DC3, 0x00001, 0x52B27F76, 0x00001,
        0x52B28129, 0x00001, 0x52B282DC, 0x00001, 0x52B2848F, 0x00001, 0x52B28642, 0x00001,
        0x52B287F5, 0x00001, 0x52B289A8, 0x00001, 0x52B28B5B, 0x00001, 0x52B4B700, 0x00001,
        0x52B4B8B3, 0x00001, 0x52B4BA66, 0x00001, 0x52B4BDCC, 0x00001, 0x52B4BF7F, 0x00001,
        0x52B4C132, 0x00001, 0x52B4C2E5, 0x00001, 0x52B4CB64, 0x00001, 0x52B4CD17, 0x00001,
        0x52B7CB70, 0x00001, 0x52B7CD23, 0x00001, 0x52B7D089, 0x00001, 0x52B7D23C, 0x00001,
        0x52B7D5A2, 0x00001, 0x52B7D755, 0x00001, 0x52BAE346, 0x00001, 0x52BAE4F9, 0x00001,
        0x52BAED78, 0x00001, 0x52BAEF2B, 0x00001, 0x52BAF291, 0x00001, 0x52BAF444, 0x00001,
        0x52BAF5F7, 0x00001, 0x52BAF7AA, 0x00001, 0x52BAF95D, 0x00001, 0x52BDFB1C, 0x00001,
        0x52BDFCCF, 0x00001, 0x52BE039B, 0x00001, 0x52BE054E, 0x00001, 0x52BE0701, 0x00001,
        0x52BE08B4, 0x00001, 0x52BE0A67, 0x00001, 0x52BE0C1A, 0x00001, 0x53DB799F, 0x00004,
        0x5423DAA0, 0x00004, 0x549B6033, 0x03000, 0x54DAE9C5, 0x00004, 0x567F1330, 0x00001,
        0x567F14E3, 0x00001, 0x567F1696, 0x00001, 0x567F1849, 0x00001, 0x567F19FC, 0x00001,
        0x56823385, 0x00001, 0x56878ECA, 0x00001, 0x5687907D, 0x00001, 0x57A07D08, 0x00004,
        0x57B1D097, 0x00004, 0x5898095A, 0x00004, 0x58AB6233, 0x00064, 0x58DC8855, 0x00004,
        0x590CD38E, 0x00004, 0x5979158E, 0x00004, 0x5988DF78, 0x00004, 0x59A4D0C3, 0x00190,
        0x5A39A553, 0x00004, 0x5B1F53F3, 0x00004, 0x5C283C72, 0x00008, 0x5C283E25, 0x00008,
        0x5C283FD8, 0x00008, 0x5C28418B, 0x00008, 0x5C28433E, 0x00008, 0x5C2844F1, 0x00008,
        0x5C2846A4, 0x00008, 0x5C284857, 0x00008, 0x5C284BBD, 0x00008, 0x61A7A35B, 0x00004,
        0x62E91A65, 0x00004, 0x62F05895, 0x00004, 0x636A5ABD, 0x000C8, 0x64A1A5B0, 0x00004,
        0x64A1A763, 0x00004, 0x64A1AE2F, 0x00004, 0x64A1AFE2, 0x00004, 0x64A1B195, 0x00004,
        0x6506EE96, 0x06D60, 0x651D61A2, 0x004B0, 0x67692BB8, 0x00004, 0x67692D6B, 0x00004,
        0x67692F1E, 0x00004, 0x676935EA, 0x00004, 0x6769379D, 0x00004, 0x6960C6EF, 0x00004,
        0x6AFB0A16, 0x00004, 0x6B35BADB, 0x00060, 0x6B734EFD, 0x00004, 0x6C03D4A8, 0x00014,
        0x6C99F9A0, 0x00002, 0x6EB3E8A0, 0x00004, 0x6F36A3AC, 0x00004, 0x717DDAA3, 0x00008,
        0x71825204, 0x00001, 0x72391B04, 0x00004, 0x727AE2EE, 0x00004, 0x727AE4A1, 0x00004,
        0x727AE654, 0x00004, 0x727AE807, 0x00004, 0x727AE9BA, 0x00004, 0x74026290, 0x00004,
        0x744447B4, 0x00004, 0x75931048, 0x00004, 0x75931561, 0x00004, 0x75CE2CF6, 0x00004,
        0x7659EC88, 0x00004, 0x76AB1B01, 0x00004, 0x76ABB5CD, 0x00004, 0x77675FA0, 0x00320,
        0x7799EB86, 0x03980, 0x77B752BC, 0x00004, 0x77B75622, 0x00004, 0x77B757D5, 0x00004,
        0x78848293, 0x00001, 0x78848446, 0x00001, 0x788485F9, 0x00001, 0x78E0935E, 0x00004,
        0x79448B5D, 0x00004, 0x79C56A5C, 0x00004, 0x7A8530FD, 0x00004, 0x7ACB8CB5, 0x00004,
        0x7B8CCB0B, 0x00180, 0x7CA9D9FA, 0x00004, 0x7D249649, 0x00004, 0x7D87DC83, 0x00004,
        0x7E82513F, 0x00004, 0x8048A7DC, 0x00004, 0x8184EFB4, 0x00004, 0x81EC3A78, 0x00004,
        0x82AD5F84, 0x00004, 0x82D57F17, 0x000C8, 0x8507839C, 0x00004, 0x85166DE2, 0x00004,
        0x877CB98F, 0x009B0, 0x885E5F53, 0x00010, 0x89C1C8DE, 0x00004, 0x8A0E9425, 0x004B0,
        0x8B18A566, 0x00004, 0x8B18A719, 0x00004, 0x8B18A8CC, 0x00004, 0x8B18AA7F, 0x00004,
        0x8B18AC32, 0x00004, 0x8B18ADE5, 0x00004, 0x8B8FB439, 0x00004, 0x8BDFF0F3, 0x00040,
        0x8BEEF106, 0x00004, 0x8C46768E, 0x00004, 0x8C5F59E8, 0x00004, 0x8D781241, 0x00008,
        0x8E434F0D, 0x002D0, 0x8F0D8720, 0x00004, 0x8FC0A045, 0x00004, 0x92EB0306, 0x00004,
        0x92F697ED, 0x00004, 0x95013114, 0x00008, 0x96993D83, 0x00008, 0x96F6F453, 0x00004,
        0x9751BABE, 0x00400, 0x98785EE4, 0x00008, 0x98786097, 0x00008, 0x987863FD, 0x00008,
        0x99E1625E, 0x07EB0, 0x9AB5F3D9, 0x00001, 0x9B986D2E, 0x00004, 0x9C41123A, 0x00004,
        0x9C808BD3, 0x00004, 0x9D5D1CA5, 0x00004, 0x9E45BE99, 0x00004, 0x9E4635BB, 0x00004,
        0x9EC079DA, 0x00002, 0x9FE2790A, 0x00A8C, 0xA00A8ABB, 0x00008, 0xA2AA5B41, 0x00004,
        0xA373DA53, 0x01900, 0xA4317061, 0x00004, 0xA69E079B, 0x00004, 0xA94E1F5F, 0x00004,
        0xA9F1368B, 0x00004, 0xAB48C136, 0x00004, 0xAD319811, 0x00004, 0xAE7B3CA1, 0x00004,
        0xAE89206E, 0x00001, 0xAEE903A2, 0x00008, 0xB027F396, 0x00004, 0xB0B2A5AA, 0x00004,
        0xB1EE7CF5, 0x00004, 0xB568265C, 0x00004, 0xB79EF1FE, 0x00004, 0xB7AAB47E, 0x00004,
        0xB7DB15CC, 0x00004, 0xB8E961AD, 0x000FB, 0xB9075BC9, 0x00008, 0xB9252862, 0x00110,
        0xBA230941, 0x00004, 0xBB0B39CF, 0x00004, 0xBCC66014, 0x00004, 0xBCC72306, 0x00004,
        0xBCE74BFD, 0x00004, 0xBDDC386E, 0x00004, 0xC02AC847, 0x00004, 0xC25B0D5A, 0x00004,
        0xC2F1C1B9, 0x02580, 0xC4C6417F, 0x000C0, 0xC4FA7C8C, 0x00004, 0xC5277828, 0x00008,
        0xC5919BF6, 0x00004, 0xC5D7112B, 0x0DCA8, 0xC69F66B6, 0x00FA0, 0xC74A3FE7, 0x00010,
        0xC7652F1C, 0x00004, 0xC7F8E3BC, 0x00008, 0xC7F8E56F, 0x00008, 0xC7F8EA88, 0x00008,
        0xC7F8EC3B, 0x00008, 0xC7F8EDEE, 0x00008, 0xC7F8EFA1, 0x00008, 0xC7F8F154, 0x00008,
        0xC7F8F4BA, 0x00008, 0xC7F8F66D, 0x00008, 0xC9541EB3, 0x00002, 0xC9A81578, 0x00010,
        0xCC022CEC, 0x00008, 0xCC022E9F, 0x00008, 0xCC023052, 0x00008, 0xCD8ADF1D, 0x00004,
        0xCFC9AA03, 0x00004, 0xCFEB27B3, 0x009C4, 0xCFED4C69, 0x00004, 0xD0068A74, 0x00004,
        0xD03A595A, 0x009B0, 0xD06FD1EA, 0x00004, 0xD11C1F59, 0x00004, 0xD20DF4A0, 0x00004,
        0xD2E5D408, 0x00004, 0xD3C06782, 0x00008, 0xD48FDC48, 0x00004, 0xD6546A02, 0x00038,
        0xD6C95683, 0x00960, 0xD6F1B724, 0x00004, 0xD8048E00, 0x00004, 0xD94D71D7, 0x00004,
        0xDD548BB1, 0x00B58, 0xDED70F11, 0x00004, 0xDFA7E2D8, 0x00004, 0xE0FB1EE7, 0x00008,
        0xE1DF0672, 0x005B8, 0xE2798DDE, 0x00001, 0xE36D5700, 0x00064, 0xE450FEF7, 0x0001A,
        0xE4E75089, 0x00040, 0xE5169DA1, 0x00004, 0xE668F1A6, 0x00001, 0xE668F359, 0x00001,
        0xE668F50C, 0x00001, 0xE668F6BF, 0x00001, 0xE668FA25, 0x00001, 0xE72BDCEC, 0x00004,
        0xE733FE4D, 0x00004, 0xE7425CFF, 0x00004, 0xE793EEAE, 0x00004, 0xEA7C87F4, 0x00020,
        0xEB550C12, 0x00004, 0xEC13DFD7, 0x00004, 0xEE10F128, 0x00004, 0xEFB533F7, 0x00001,
        0xF25C070E, 0x00080, 0xF32218C2, 0x00004, 0xF3CF94FF, 0x00004, 0xF41CFF61, 0x00200,
        0xF45C3F59, 0x00004, 0xF4954B60, 0x00004, 0xF5D9F4A5, 0x00118, 0xF626721E, 0x00004,
        0xF62D79D3, 0x00004, 0xF8154AC9, 0x00004, 0xFB58B1A7, 0x00064, 0xFB5AE87D, 0x00004,
        0xFC374750, 0x00004, 0xFF400328, 0x00004, 0xFFCC05C2, 0x00001,
    ];
}
