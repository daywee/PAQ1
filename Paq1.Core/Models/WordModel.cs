using System.Collections.Generic;

namespace Paq1.Core.Models
{
    public class WordModel : IModel
    {
        private uint word0; // hash of word
        private uint word1; // hash of previous word

        private int word0Length; // word length (weight), max length == 8
        private int word1Length; // word length of previous word

        private readonly Dictionary<uint, int[]> counter0 = new Dictionary<uint, int[]>();
        private readonly Dictionary<uint, int[]> counter1 = new Dictionary<uint, int[]>();

        private uint context0;
        private uint context1;
        private uint partialByte = 1; // 0-7 bits of current character
        private uint partialByteHash = 1;
        private uint previousByte;

        public (int, int) Predict(int n0, int n1)
        {
            if (!counter0.ContainsKey(context0))
            {
                counter0[context0] = new int[2];
            }

            if (!counter1.ContainsKey(context1))
            {
                counter1[context1] = new int[2];
            }

            var c = counter0[context0];
            n0 += c[0] * word0Length;
            n1 += c[1] * word0Length;

            c = counter1[context1];
            n0 += c[0] * word1Length;
            n1 += c[1] * word1Length;

            return (n0, n1);
        }

        public void Update(int bit)
        {
            UpdateCounter(counter0[context0], bit);
            UpdateCounter(counter1[context1], bit);

            //partialByte += partialByte + (uint)bit;
            partialByte = (partialByte << 1) | (uint)bit;
            partialByteHash = (partialByteHash << 1) | (uint)bit;
            if (partialByteHash >= 59)
                partialByteHash -= 59;

            if (partialByte >= 256)
            {
                char c = (char)(partialByte & 0xff);
                previousByte = partialByte;
                partialByte = 1;
                partialByteHash = 1;

                if (char.IsLetter(c)) // add char to word
                {
                    c = char.ToLower(c);
                    word0 = (word0 + c + 1) * 234577751 * 16;
                    word0Length++;
                    if (word0Length > 8)
                        word0Length = 8;
                }
                else if (word0 > 0) // finish current word
                {
                    word1Length = word0Length;
                    word0Length = 0;

                    word1 = word0;
                    word0 = 0;
                }
            }

            uint h = word0 * 123456791 + previousByte * 345689647 + partialByteHash + (partialByte << 24);
            context0 = h;
            context1 = h + word1;
        }

        private void UpdateCounter(int[] counter, int bit)
        {
            var a = bit;
            var b = 1 - a;

            if (counter[a] < 255) counter[a]++;
            if (counter[b] > 0)
            {
                counter[b] = counter[b] / 2 + 1;
            }
        }
    }
}
