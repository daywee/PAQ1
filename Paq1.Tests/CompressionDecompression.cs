using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Paq1.Tests
{
    [TestClass]
    public class CompressionDecompression
    {
        private const string RootFolder = "../../TestData/";

        [TestMethod]
        public void CanDecompressShortText()
        {
            string source = RootFolder + "paq1description.txt";
            string compressed = Path.GetTempFileName();
            string decompressed = Path.GetTempFileName();

            var paq = new Core.Paq1();
            var originalSize = paq.Compress(source, compressed);

            paq = new Core.Paq1();
            paq.Decompress(compressed, decompressed, originalSize);

            CompareFiles(source, decompressed);

            File.Delete(compressed);
            File.Delete(decompressed);
        }

        [TestMethod]
        public void CanDecompressText()
        {
            string source = RootFolder + "lipsum.txt";
            string compressed = Path.GetTempFileName();
            string decompressed = Path.GetTempFileName();

            var paq = new Core.Paq1();
            var originalSize = paq.Compress(source, compressed);

            paq = new Core.Paq1();
            paq.Decompress(compressed, decompressed, originalSize);

            CompareFiles(source, decompressed);

            File.Delete(compressed);
            File.Delete(decompressed);
        }

        [TestMethod]
        public void CanDecompressLongText()
        {
            string source = RootFolder + "lipsumLarge.txt";
            string compressed = Path.GetTempFileName();
            string decompressed = Path.GetTempFileName();

            var paq = new Core.Paq1();
            var originalSize = paq.Compress(source, compressed);

            paq = new Core.Paq1();
            paq.Decompress(compressed, decompressed, originalSize);

            CompareFiles(source, decompressed);

            File.Delete(compressed);
            File.Delete(decompressed);
        }

        [TestMethod]
        public void CanDecompressCsv()
        {
            string source = RootFolder + "data.csv";
            string compressed = Path.GetTempFileName();
            string decompressed = Path.GetTempFileName();

            var paq = new Core.Paq1();
            var originalSize = paq.Compress(source, compressed);

            paq = new Core.Paq1();
            paq.Decompress(compressed, decompressed, originalSize);

            CompareFiles(source, decompressed);

            File.Delete(compressed);
            File.Delete(decompressed);
        }

        private void CompareFiles(string path1, string path2)
        {
            using (var f1 = File.OpenRead(path1))
            using (var f2 = File.OpenRead(path2))
            {
                if (f1.Length != f2.Length)
                    Assert.Fail();

                for (int i = 0; i < f1.Length; i++)
                {
                    var b1 = f1.ReadByte();
                    var b2 = f2.ReadByte();
                    if (b1 != b2)
                        Assert.Fail();
                }
            }
        }
    }
}
