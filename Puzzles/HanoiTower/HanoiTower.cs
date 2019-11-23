using System;
using System.Linq;
using System.Collections.Generic;

namespace HanoiTower
{
    class Program
     {
         static void Solve(int pegA, int pegB, char from, char to, char temp)
         {
            if (pegA == 0) return;

            // Move B to Temp
            Solve(pegB, 0, to, temp, from);
            // One peg from A to B
            Console.WriteLine($"Moving ring {pegB + 1} from {from} to {to}");
            // Move Temp to B
            Solve(pegB, 0, temp, to, from);

            // Solve smaller task
            Solve(pegA - 1, pegB + 1, from, to, temp);
        }

        static void Solve1(int n, char from, char to, char temp)
         {
            if (n == 1)
            {
                Console.WriteLine($"Moving ring 1 from {from} to {to}");
                return;
            } 

            Solve1(n-1, from, temp, to);
            Console.WriteLine($"Moving ring {n} from {from} to {to}");
            Solve1(n-1, temp, to, from);
        }

        static void Main(string[] args)
        {
            // Solve(3, 0, 'A', 'B', 'C');
            Solve1(3, 'A', 'B', 'C');
        }
    }
}
