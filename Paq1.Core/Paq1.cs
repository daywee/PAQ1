using System;
using System.Collections.Generic;

namespace Paq1.Core
{
    public class Paq1
    {
        private readonly List<uint> _compressedBits = new List<uint>();

        public List<uint> Compress(List<int> bits)
        {
            var predictor = new Predictor();

            int cnt = 0;

            foreach (var bit in bits)
            {
                double p0 = predictor.Predict();
                Encode(bit, p0);
                predictor.Update(bit);
            }

            while (lo > 0)
            {
                var writeBit = (lo & msb) >> shift;
                _compressedBits.Add(writeBit);
                lo <<= 1;
            }

            return _compressedBits;
        }

        private const int shift = 31; // shift from first to last position in 32 bit integer
        private const uint msb = 1u << 31;

        private uint lo = 0;
        private uint hi = 0xffffffff;


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
                    _compressedBits.Add(writeBit);

                    lo = lo << 1;         // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1
                }
            }
        }

        private uint prevBytes = 0;
        private List<uint> _decompressedBits = new List<uint>();

        public List<uint> Decompress(List<uint> bits, int originalSize)
        {
            var predictor = new Predictor();

            _nextBitEnumerator = bits.GetEnumerator();

            for (int i = 0; i < 32; i++) // initialize with first 4 bytes
            {
                prevBytes = (prevBytes << 1) | NextBit();
            }

            int cnt = 0;
            while (_decompressedBits.Count < originalSize)
            {
                double p0 = predictor.Predict();
                var decompressedBit = Decode(p0);
                predictor.Update((int)decompressedBit);
            }

            return _decompressedBits;
        }

        private IEnumerator<uint> _nextBitEnumerator;
        private uint NextBit()
        {
            if (!_nextBitEnumerator.MoveNext())
                return 0;

            return _nextBitEnumerator.Current;
        }

        private uint Decode(double p0)
        {
            checked // check for int overflow
            {
                var rangeDiff = hi - lo;
                uint mid = Convert.ToUInt32(lo + p0 * (rangeDiff));

                uint decompressedBit;
                if (prevBytes <= mid)
                {
                    hi = mid;
                    decompressedBit = 0;
                }
                else
                {
                    lo = mid + 1;
                    decompressedBit = 1;
                }
                _decompressedBits.Add(decompressedBit);

                while ((lo & msb) == (hi & msb))
                {
                    lo = lo << 1;       // shift one written bit
                    hi = (hi << 1) | 1;   // shift one written bit and set LSB to 1

                    prevBytes = (prevBytes << 1) | NextBit();
                }

                return decompressedBit;
            }
        }
    }
}
