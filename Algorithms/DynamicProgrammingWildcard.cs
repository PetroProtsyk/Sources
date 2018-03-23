using System;

namespace Protsyk.Algorithms
{
    public class DynamicProgrammingWildcard
    {
        public static bool Match(string pattern, string word)
        {
            var m = new bool[pattern.Length + 1, word.Length + 1];
            m[0, 0] = true;
            for (int i = 0; i < pattern.Length; ++i)
            {
                if (pattern[i] == '*')
                {
                    m[i + 1, 0] = m[i, 0];
                }
            }

            for (int i = 0; i < pattern.Length; ++i)
            {
                for (int j = 0; j < word.Length; ++j)
                {
                    if (pattern[i] == '*')
                    {
                        m[i + 1, j + 1] = m[i, j] || m[i + 1, j] || m[i, j + 1];
                    }
                    else
                    {
                        m[i + 1, j + 1] = m[i, j] & (pattern[i] == word[j] || pattern[i] == '?');
                    }
                }
            }

            return m[pattern.Length, word.Length];
        }
    }
}
