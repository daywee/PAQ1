using System.Collections.Generic;
using System.Linq;

namespace Paq1.Core.Models
{
    public class CyclicModel : IModel
    {
        class Pattern
        {
            public uint LastMatchPosition { get; set; } // p
            public uint NumberOfMatches { get; set; } // n
            public uint Interval { get; set; } // r
        }

        private uint column; // column number, 0 to cycle-1
        private uint cycle; // number of columns
        private uint size; // number of bits before table expires (0 to 3*cycle)
        private uint last8bits = 1;
        private uint currentPosition;
        private uint context0;
        private uint context1;

        private Pattern[] patterns = Enumerable.Range(0, 256).Select(_ => new Pattern()).ToArray(); // patterns indexed by character
        private readonly Dictionary<uint, int[]> counter0 = new Dictionary<uint, int[]>();
        private readonly Dictionary<uint, int[]> counter1 = new Dictionary<uint, int[]>();

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

            if (cycle > 0)
            {
                int weights = 16;
                var c = counter0[context0];
                n0 += c[0] * weights;
                n1 += c[1] * weights;

                weights = 4;
                c = counter1[context1];
                n0 += c[0] * weights;
                n1 += c[1] * weights;
            }

            return (n0, n1);
        }

        public void Update(int bit)
        {
            if (++column >= cycle)
                column = 0;
            if (size > 0 && --size == 0)
                cycle = 0;

            UpdateCounter(counter0[context0], bit);
            UpdateCounter(counter1[context1], bit);

            last8bits = ((last8bits << 1) | (uint)bit) & 0xff;
            currentPosition++;

            var pattern = patterns[last8bits];
            if (pattern.LastMatchPosition + pattern.Interval == currentPosition)
            {
                pattern.NumberOfMatches++;
                if (pattern.NumberOfMatches > 3 && pattern.Interval > 8 && pattern.Interval * pattern.NumberOfMatches > size)
                {
                    size = pattern.Interval * pattern.NumberOfMatches;
                    if (cycle != pattern.Interval)
                    {
                        cycle = pattern.Interval;
                        column = currentPosition % cycle;
                    }
                }
            }
            else
            {
                pattern.NumberOfMatches = 1;
                pattern.Interval = currentPosition - pattern.LastMatchPosition;
            }

            pattern.LastMatchPosition = currentPosition;
            uint hash = column * 3 + last8bits * 876546821;
            context0 = hash;
            context1 = column & 2047;
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
