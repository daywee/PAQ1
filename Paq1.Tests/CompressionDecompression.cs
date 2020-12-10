using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Paq1.Tests
{
    [TestClass]
    public class CompressionDecompression
    {
        private const string RootFolder = "../../TestData/";

        [DataTestMethod]
        [DataRow(RootFolder + "paq1description.txt")] // short text
        [DataRow(RootFolder + "lipsum.txt")] // medium text
        [DataRow(RootFolder + "lipsumLarge.txt")] // long text
        [DataRow(RootFolder + "data.csv")] // tabular data
        public void CanCompressAndDecompress(string dataPath)
        {
            var source = File.OpenRead(dataPath);
            var compressed = new MemoryStream();
            var decompressed = new MemoryStream();

            var paq = new Core.Paq1();
            paq.Compress(source, compressed);

            compressed.Position = 0;

            paq = new Core.Paq1();
            paq.Decompress(compressed, decompressed);

            source.Position = 0;
            decompressed.Position = 0;

            CompareStreams(source, decompressed);
        }

        private void CompareStreams(Stream first, Stream second)
        {
            if (first.Length != second.Length)
                Assert.Fail();

            for (int i = 0; i < first.Length; i++)
            {
                var b1 = first.ReadByte();
                var b2 = second.ReadByte();
                if (b1 != b2)
                    Assert.Fail();
            }

        }
    }
}
