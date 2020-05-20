using System;

namespace Paq1.Core
{
    public class Paq1
    {
        private const int shift = 31; // shift from first to last position in 32 bit integer
        private const uint msb = 1u << 31;

        private uint lo = 0;
        private uint hi = 0xffffffff;

        private BitFile sourceFile;
        private BitFile targetFile;
        private Predictor predictor;

        public long Compress(string sourcePath, string targetPath)
        {
            predictor = new Predictor();

            using (sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
                uint bit;
                while (!sourceFile.IsEof)
                {
                    bit = sourceFile.Read();
                    double p0 = predictor.Predict();
                    // todo: remove casting
                    Encode((int)bit, p0);
                    predictor.Update((int)bit);
                }

                while (lo > 0)
                {
                    var writeBit = (lo & msb) >> shift;
                    targetFile.Write(writeBit);
                    lo <<= 1;
                }

                return sourceFile.Length; // todo: encode length to header
            }
        }

        private void Encode(int bit, double p0)
        {
            checked // check for int overflow
            {
                uint mid = Convert.ToUInt32(lo + p0 * (hi - lo));

                if (bit == 1)
                    lo = mid + 1;
                else
                    hi = mid;

                while ((lo & msb) == (hi & msb))
                {
                    var writeBit = (lo & msb) >> shift; // & msb is probably useless here
                    targetFile.Write(writeBit);

                    lo = lo << 1;         // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1
                }
            }
        }

        private uint previousBytes = 0;
        public void Decompress(string sourcePath, string targetPath, long originalSize)
        {
            predictor = new Predictor();
            using (sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
                for (int i = 0; i < 32; i++) // initialize with first 4 bytes
                {
                    previousBytes = (previousBytes << 1) | sourceFile.Read();
                }

                while (targetFile.Length < originalSize)
                {
                    double p0 = predictor.Predict();
                    var decompressedBit = Decode(p0);
                    predictor.Update((int)decompressedBit);
                }
            }
        }

        private uint Decode(double p0)
        {
            checked // check for int overflow
            {
                var rangeDiff = hi - lo;
                uint mid = Convert.ToUInt32(lo + p0 * (rangeDiff));

                uint decompressedBit;
                if (previousBytes <= mid)
                {
                    hi = mid;
                    decompressedBit = 0;
                }
                else
                {
                    lo = mid + 1;
                    decompressedBit = 1;
                }
                targetFile.Write(decompressedBit);

                while ((lo & msb) == (hi & msb))
                {
                    lo = lo << 1;       // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1

                    previousBytes = (previousBytes << 1) | sourceFile.Read();
                }

                return decompressedBit;
            }
        }
    }
}
