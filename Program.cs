using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.Sources
{
    class Program
    {
        public static void Main(String[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintConsole(ConsoleColor.Green, "Algorithms, Data Structures, Puzzles (c) Petro Protsyk 2001-2020");
        }

        private static void PrintConsole(ConsoleColor color, string text)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }

    }
}
