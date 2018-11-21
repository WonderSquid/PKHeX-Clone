﻿using FluentAssertions;
using PKHeX.Core;
using System.IO;
using Xunit;

namespace PKHeX.Tests.Saves
{
    public class SMTests
    {
        private SAV7 GetSave()
        {
            var folder = GetRepoPath();
            var path = Path.Combine(folder, "TestData", "SM Project 802.main");
            return new SAV7(File.ReadAllBytes(path));
        }

        private string GetRepoPath()
        {
            var folder = Directory.GetCurrentDirectory();
            while (!folder.EndsWith(nameof(Tests)))
                folder = Directory.GetParent(folder).FullName;
            return folder;
        }

        [Fact]
        public void ChecksumsValid()
        {
            GetSave().ChecksumsValid.Should().BeTrue();
        }

        [Fact]
        public void ChecksumsUpdate()
        {
            var save = GetSave();
            var originalChecksumInfo = save.ChecksumInfo;
            var newSave = new SAV7(save.Write(false, false));

            save.ChecksumInfo.Should().BeEquivalentTo(originalChecksumInfo, "because the checksum should have been modified");
            save.ChecksumsValid.Should().BeTrue("because the checksum should be valid after write");
            newSave.ChecksumsValid.Should().BeTrue("because the checksums should be valid after reopening the save");
            newSave.ChecksumInfo.Should().BeEquivalentTo(save.ChecksumInfo, "because the checksums should be the same since write and open");
        }
    }
}
