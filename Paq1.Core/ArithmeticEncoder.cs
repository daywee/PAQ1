using System;

namespace Paq1.Core
{
    public class ArithmeticEncoder
    {
        private readonly BitFile _targetFile;
        private const int shift = 31;       // shift from first to last position in 32 bit integer
        private const uint msb = 1u << 31;  // position of MSB in 32 bit integer

        private uint lo = 0;
        private uint hi = 0xffffffff;

        public ArithmeticEncoder(BitFile targetFile)
        {
            _targetFile = targetFile;
        }

        public void Encode(int bit, double p0)
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
                    _targetFile.Write(writeBit);

                    lo = lo << 1;         // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1
                }
            }
        }

        public void EncodeRemainingBits()
        {
            while (lo > 0)
            {
                var writeBit = (lo & msb) >> shift;
                _targetFile.Write(writeBit);
                lo <<= 1;
            }
        }
    }
}
