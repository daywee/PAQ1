using System.Collections.Generic;

namespace Paq1.Core.Models
{
    public class ContextComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
        public int GetHashCode(int[] a)
        {
            int hash = 17;
            for (int i = 0; i < a.Length; i++)
            {
                unchecked
                {
                    hash = hash * 23 + a[i];
                }
            }
            return hash;
        }
    }
}
