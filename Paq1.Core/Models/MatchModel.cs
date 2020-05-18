using System;

namespace Paq1.Core.Models
{
    /// <summary>
    /// A MatchModel looks for a match of length n = 8 bytes or more between
    /// the current context and previous input and guesses that the next bit
    /// will be the same with weight 3n^2.  Matches are found in a 4MB rotating
    /// buffer using a 1M hash table of pointers.
    /// </summary>
    public class MatchModel : IModel
    {
        private byte[] _buffer = new byte[Convert.ToInt32(Math.Pow(2, 22))]; // input buffer, wraps at end
        private uint[] _ptr = new uint[Convert.ToInt32(Math.Pow(2, 20))]; // hash table of pointers // originally uint24
        private int pos; // element of buf where next bit will be stored
        private int bpos; // number of bits stored at _buffer[pos]
        private int begin; // points to first matching byte (does not wrap)
        private int end; // points to last matching byte + 1, 0 if no match
        private uint hash; // hash of current context up to pos-1

        public (int, int) Predict(int n0, int n1)
        {
            if (end > 0)
            {
                int wt = end - begin;
                if (wt > 1000)
                    wt = 3000000; // 3n^2; n=1000
                else
                    wt = 3 * wt * wt;

                if (((_buffer[end] >> (7 - bpos)) & 1) == 1)
                    n1 += wt;
                else
                    n0 += wt;
            }

            return (n0, n1);
        }

        public void Update(int bit)
        {
            _buffer[pos] = (byte)((_buffer[pos] << 1) | bit); // store bit
            bpos++;

            if ((end > 0) && (_buffer[end] >> (8 - bpos)) != _buffer[pos]) // does it match?
            {
                // no
                begin = 0;
                end = 0;
            }

            if (bpos == 8) // new byte
            {
                bpos = 0;
                hash = hash * (16 * 123456791) + _buffer[pos] + 1;
                pos++;

                if (pos == _buffer.Length)
                    pos = 0;

                if (end > 0)
                    end++;
                else
                {
                    // if no match, search for one
                    uint h = ((hash ^ (hash >> 16)) & ((uint)_ptr.Length - 1));
                    end = (int)_ptr[Convert.ToInt32(h)];

                    if (end > 0)
                    {
                        begin = end;
                        int p = pos;
                        while (begin > 0 && p > 0 && begin != (p + 1) && _buffer[begin - 1] == _buffer[p - 1])
                        {
                            begin--;
                            p--;
                        }
                    }

                    if (end == begin) // no match found
                    {
                        begin = 0;
                        end = 0;
                    }

                    _ptr[Convert.ToInt32(h)] = (uint)pos;
                }
            }
        }
    }
}
