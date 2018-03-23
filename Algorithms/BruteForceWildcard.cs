using System;

namespace Protsyk.Algorithms
{
    public static class BruteForceWildcard
    {
        public static bool Match(string pattern, string word)
        {
            return MatchRecursive(pattern, 0, word, 0);
        }

        private static bool MatchRecursive(string p, int startP, string a, int startA)
        {
            var ai = startA;
            for (int i = startP; i < p.Length; ++i)
            {
                if (ai >= a.Length)
                {
                    // Word matched, skip trailing stars
                    if (p[i] != '*')
                    {
                        return false;
                    }
                }
                else
                {
                    if (p[i] == a[ai] || p[i] == '?')
                    {
                        ai++;
                    }
                    else if (p[i] == '*')
                    {
                        for (int j = ai; j < a.Length; ++j)
                        {
                            if (MatchRecursive(p, i + 1, a, j))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
