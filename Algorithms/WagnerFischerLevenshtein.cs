using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEA2016.FuzzySearch
{
	/// <summary>
	/// Calculate Levenshtein distance between two strings using Dynamic Programming approach
	/// </summary>
	public static class LevenshteinDynamicProgramming
	{
		/// <summary>
		/// Wagner–Fischer algorithm with full matrix
		/// Time complexity: O(|a|*|b|)
		/// Memory complexity: O(|a|*|b|)
		/// https://en.wikipedia.org/wiki/Wagner%E2%80%93Fischer_algorithm
		/// </summary>
		public static int Calculate(string a, string b)
		{
			return CreateMatrix(a, b)[a.Length, b.Length];
		}

		/// <summary>
		/// Wagner–Fischer algorithm that uses memory for two rows
		/// Time complexity: O(|a|*|b|)
		/// Memory complexity: O(max(|a|,|b|))
		/// https://en.wikipedia.org/wiki/Wagner%E2%80%93Fischer_algorithm
		/// </summary>
		public static int CalculateMemoryOptimized(string a, string b)
		{
			if (a.Length < b.Length) {
				var t = a;
				a = b;
				b = t;
			}

			var d = new int[b.Length + 1];
			var c = new int[b.Length + 1];
			
			// Initialization
			for (int i = 0; i <= b.Length; i++)
				d[i] = i;

			// Calculation
			for (int i = 0; i < a.Length; i++) {
				c[0] = i + 1;

				for (int j = 0; j < b.Length; j++) {
					if (a[i] == b[j]) {
						c[j + 1] = d[j];
					} else {
						c[j + 1] = 1 + Math.Min(d[j], Math.Min(d[j + 1], c[j]));
					}
				}
                
				var t = c;
				c = d;
				d = t;
			}

			return d[b.Length];
		}
        
		/// <summary>
		/// Print steps to transform a to b
		/// </summary>
		public static TraceLog Trace(string a, string b)
		{
			return new TraceLog(a, b, CreateMatrix(a, b));
		}

		private static int[,] CreateMatrix(string a, string b)
		{
			var d = new int[a.Length + 1, b.Length + 1];

			// Initialization
			for (int i = 0; i <= a.Length; i++)
				d[i, 0] = i;
			for (int i = 0; i <= b.Length; i++)
				d[0, i] = i;

			// Calculation
			for (int i = 0; i < a.Length; i++) {
				for (int j = 0; j < b.Length; j++) {
					if (a[i] == b[j]) {
						d[i + 1, j + 1] = d[i, j];
					} else {
						d[i + 1, j + 1] = 1 + Math.Min(d[i, j], Math.Min(d[i, j + 1], d[i + 1, j]));
					}
				}
			}
			
			return d;
		}
		
		public class TraceLog
		{
			private readonly int[,] d;
			private readonly string a;
			private readonly string b;

			internal TraceLog(string a, string b, int[,] d)
			{
				this.a = a;
				this.b = b;
				this.d = d;
			}

			public void PrintMatrix()
			{
				for (int i = 0; i <= a.Length; i++) {
					for (int j = 0; j <= b.Length; j++) {
						Console.Write(d[i, j] + " ");
					}
					Console.WriteLine();
				}
			}

			public void PrintTransformations()
			{
				// Trace back
				int k = a.Length;
				int m = b.Length;
				int p = b.Length;
				var path = new Stack<Action>();
				while ((k != 0) || (m != 0)) {
					int c = d[k, m];
					if ((k > 0) && (m > 0) && (d[k - 1, m - 1] < c)) {
						k = k - 1;
						m = m - 1;
						--p;
						// $"Sub {a[k]} with {b[m]} at position {p}"						
						path.Push(new Action(1, p, b[m]));
					} else if ((k > 0) && (d[k - 1, m] < c)) {
						k = k - 1;
						path.Push(new Action(2, p, '\0'));
						// $"Del {a[k]} at position {p}"
					} else if ((m > 0) && (d[k, m - 1] < c)) {
						m = m - 1;
						--p;
						// $"Ins {b[m]} at position {p}"
						path.Push(new Action(3, p, b[m]));
					} else if ((k > 0) && (m > 0) && (d[k - 1, m - 1] == c)) {
						k = k - 1;
						m = m - 1;
						--p;
					} else if ((k > 0) && (d[k - 1, m] == c)) {
						k = k - 1;
					} else if ((m > 0) && (d[k, m - 1] == c)) {
						m = m - 1;
					}
				}

				var tr = new List<char>(a);
				int counter = 0;
				Console.WriteLine(string.Format("{0}: {1}", counter, a));
				while (path.Count > 0) {
					var action = path.Pop();
					switch (action.type) {
						case 1:
							tr[action.position] = action.c;
							break;
						case 2:
							tr.RemoveAt(action.position);
							break;
						case 3:
							tr.Insert(action.position, action.c);
							break;
					}
					++counter;
					Console.Write(string.Format("{0}: ", counter));
					Console.WriteLine(new string(tr.ToArray()));
				}
			}
		}

		private struct Action
		{
			public readonly int type;
			public readonly int position;
			public readonly char c;

			public Action(int type, int position, char c)
			{
				this.type = type;
				this.position = position;
				this.c = c;
			}
		}

	}

    public static class Program
    {
    	public static void Main(string[] args)
        {
           var words = Console.ReadLine().Split(' ');

           Console.WriteLine($"Distance between words {words[0]} and {words[1]} using dynamic programming algorithm:");
           Console.WriteLine(LevenshteinDynamicProgramming.Calculate(words[0], words[1]));
        
           Console.WriteLine("Transformations:");
           var trace = LevenshteinDynamicProgramming.Trace(words[0], words[1]);
           trace.PrintTransformations();
        }
    }
}
