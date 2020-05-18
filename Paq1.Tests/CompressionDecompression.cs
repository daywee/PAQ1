using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paq1.Tests
{
    [TestClass]
    public class CompressionDecompression
    {
        private const string RootFolder = "../../TestData/";

        [TestMethod]
        public void Decompression()
        {
            var source = ReadBits(RootFolder + "paq1description.txt");

            var paq = new Core.Paq1();
            var compressed = paq.Compress(source);

            var dpaq = new Core.Paq1();
            var decompressed = dpaq.Decompress(compressed, source.Count);

            for (int i = 0; i < Math.Min(source.Count, decompressed.Count); i++)
            {
                if (source[i] != decompressed[i])
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void DecompressionOfLongText()
        {
            var source = ReadBits(RootFolder + "lipsum.txt");

            var paq = new Core.Paq1();
            var compressed = paq.Compress(source);

            var dpaq = new Core.Paq1();
            var decompressed = dpaq.Decompress(compressed, source.Count);

            for (int i = 0; i < Math.Min(source.Count, decompressed.Count); i++)
            {
                if (source[i] != decompressed[i])
                {
                    Assert.Fail();
                }
            }
        }

        private List<int> ReadBits(string path)
        {
            var bits = new List<int>();
            var bytes = File.ReadAllBytes(path);

            foreach (var b in bytes)
            {
                for (var i = 7; i >= 0; i--)
                {
                    var bit = (b >> i) & 1;
                    bits.Add(bit);
                }
            }

            return bits;
        }
    }
}
