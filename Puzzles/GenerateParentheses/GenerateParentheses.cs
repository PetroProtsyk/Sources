using System;

namespace GenerateParentheses
{
    /// <summary>
    /// Generate all valid (properly opened and closed) combinations of n pairs of parentheses.
    /// </summary>
    class Program
    {
        static void Solve(int stillToOpen, int currentlyOpened, int position, char[] result)
        {
            if (position == result.Length)
            {
                Console.WriteLine(new string(result));
                return;
            }

            if (stillToOpen > 0)
            {
                result[position] = '(';
                Solve(stillToOpen - 1, currentlyOpened + 1, position + 1, result);
            }

            if (currentlyOpened > 0)
            {
                result[position] = ')';
                Solve(stillToOpen, currentlyOpened - 1, position + 1, result);
            }
        }

        static void Solve(int n)
        {
            Solve(n, 0, 0, new char[2*n]);
        }

        static void Main(string[] args)
        {
            Solve(3);
        }
    }
}