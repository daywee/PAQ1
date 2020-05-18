using System.Collections.Generic;

namespace Paq1.Core.Models
{
    public class PartialNonStationaryPpmModel : IModel
    {
        public int Order { get; }
        private readonly int[] _context;
        private readonly Dictionary<int[], int[]> _counters = new Dictionary<int[], int[]>(new ContextComparer());

        public PartialNonStationaryPpmModel(int order)
        {
            Order = order;
            _context = new int[order + 1];
            _context[order] = 1;
        }

        public (int, int) Predict(int n0, int n1)
        {
            if (!_counters.ContainsKey(_context))
            {
                _counters[_context.Clone() as int[]] = new int[2];
            }

            var c = _counters[_context];
            return (c[0], c[1]);
        }

        public void Update(int bit)
        {
            var a = bit;
            var b = 1 - a;

            var el = _counters[_context];
            if (el[a] < 255) el[a]++;
            if (el[b] > 0)
            {
                el[b] = el[b] / 2 + 1;
            }

            _context[Order] = (_context[Order] << 1) | bit;
            if (_context[Order] >= 256)
            {
                _context[Order] = _context[Order] & 0xff;
                for (int i = 0; i < Order; i++)
                {
                    _context[i] = _context[i + 1];
                }

                _context[Order] = 1;
            }
        }
    }
}
