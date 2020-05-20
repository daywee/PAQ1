using System;

namespace Paq1.Core
{
    public class ArithmeticDecoder
    {
        private readonly BitFile _sourceFile;
        private const uint msb = 1u << 31; // position of MSB in 32 bit integer

        private uint lo = 0;
        private uint hi = 0xffffffff;
        private uint previousBytes = 0;

        public ArithmeticDecoder(BitFile sourceFile)
        {
            _sourceFile = sourceFile;

            for (int i = 0; i < 32; i++) // initialize with first 4 bytes
            {
                previousBytes = (previousBytes << 1) | sourceFile.Read();
            }
        }

        public uint Decode(double p0)
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

                while ((lo & msb) == (hi & msb))
                {
                    lo = lo << 1;         // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1

                    previousBytes = (previousBytes << 1) | _sourceFile.Read();
                }

                return decompressedBit;
            }
        }
    }
}
