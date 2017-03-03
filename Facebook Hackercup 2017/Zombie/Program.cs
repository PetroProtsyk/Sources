using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Zombie
{
	class Program
	{
        public static Dictionary<Tuple<int, int, int>, double> ch = new Dictionary<Tuple<int, int, int>, double>();

        public static double CastBest(int X, int Y, int H)
        {
            if (X * Y < H)
            {
                return 0.0;
            }
            else if (X == 0)
            {
                if (H > 0) return 0.0;
                else return 1.0;
            }
            else if (H == 1)
            {
                if (Y > 0) return 1.0;
                else return 0.0;
            }
            else if (X == 1)
            {
                if (H > Y)
                {
                    return 0.0;
                }
                else
                {
                    return Math.Min(1.0, (Y - H + 1.0) / Y);
                }
            }
            else
            {
                double r = 0.0;

                if (ch.TryGetValue(Tuple.Create(X, Y, H), out r))
                {
                    return r;
                }

                {
                    double c = 0.0;
                    for (int i = 1; i <= Y; ++i)
                    {
                        c += CastBest(X - 1, Y, H - i);
                    }
                    r = c / (double)Y;
                }

                ch.Add(Tuple.Create(X, Y, H), r);

                return r;
            }
        }

		public static void Main(string[] args)
		{
			//Console.SetIn(new StreamReader("fighting_the_zombie_example_input.txt"));
			int T = int.Parse(Console.ReadLine());
					
			var regExp = new Regex("^(?<X>\\d+)d(?<Y>\\d+)(?<Z>[+-]\\d+)?$");
			
				
			for (int i=0; i<T; ++i)
			{
				var tok = Console.ReadLine().Split(' ').Select(t=>int.Parse(t)).ToArray();
				int H = tok[0];
				int S = tok[1];


                var toksp = Console.ReadLine().Split(' ').Select(t =>
                {
                    var m = regExp.Match(t); return new
                    {
                        X = int.Parse(m.Groups["X"].Value),
                        Y = int.Parse(m.Groups["Y"].Value),
                        Z = !string.IsNullOrEmpty(m.Groups["Z"].Value) ? int.Parse(m.Groups["Z"].Value) : 0
                    };
                }).ToArray();

                double result = 0.0;
                foreach (var spell in toksp)
                {
                    result  = Math.Max(result, CastBest(spell.X, spell.Y, H - spell.Z));
                    if (result > 1.0 - 0.0000001) break;
                }
                Console.Write(String.Format("Case #{0}: {1:N6}", i+1, result));
				Console.Write('\n');
			}
		}
	}
}