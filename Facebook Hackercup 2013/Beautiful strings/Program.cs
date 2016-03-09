using System;
using System.Text;


namespace Protsyk.Puzzles.BeautifulStrings
{
    class Program
    {
        static void Main(String []args)
        {
            int n = int.Parse(Console.ReadLine());
            
            for (int i=0; i<n; i++)
            {
                string s = Console.ReadLine().ToLower();
                int [] count = new int[26];
                
                for (int j=0; j<s.Length; j++)
                {
                    int code = (int)s[j] - (int)('a');
                    if (code >= 0 && code < 26)
                      count[code]++;
                }
                
                Array.Sort(count);
                                
                int best = 0;            
                for (int j=0; j<count.Length; j++)
                {
                 best += count[count.Length - 1 - j] * (26 - j);
                }
                
                Console.WriteLine("Case #{0}: {1}", i+1, best);
            }
        }
    }
}