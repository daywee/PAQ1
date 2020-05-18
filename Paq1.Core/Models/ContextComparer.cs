using System.Collections.Generic;

namespace Paq1.Core.Models
{
    public class ContextComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            if (x == null || y == null)
                return false;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                    return false;
            }

            return true;
        }

        // based on https://stackoverflow.com/questions/14663168/an-integer-array-as-a-key-for-dictionary
        public int GetHashCode(int[] obj)
        {
            int hash = 17;
            foreach (var i in obj)
            {
                unchecked
                {
                    hash = hash * 23 + i;
                }
            }

            return hash;
        }
    }
}
