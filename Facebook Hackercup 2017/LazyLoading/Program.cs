using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Lazy_Loading
{
	class Program
	{
		public static void Main(string[] args)
		{
			//Console.SetIn(new StreamReader("lazy_loading_example_input.txt"));
			
			var T = int.Parse(Console.ReadLine());
			
			for (int i=0; i<T; ++i)
			{
				var N = int.Parse(Console.ReadLine());
				var W = new List<int>();
				for (int j=0; j<N; ++j)
				{
					var Wj = int.Parse(Console.ReadLine());
					W.Add(Wj);
				}
				
				W.Sort((a,b)=>b-a);
				int top = 0;
				int bottom = W.Count;
				int c = 0;
				int result = 0;
				while (top != bottom)
				{
					if ((1+c)*W[top] < 50)
					{
						c++;
						bottom--;
					}
					else
					{
						top++;
						result++;
						c = 0;
					}
				}
				
				
				Console.Write(String.Format("Case #{0}: {1}", i+1, result));
				Console.Write('\n');
			}
		}
	}
}