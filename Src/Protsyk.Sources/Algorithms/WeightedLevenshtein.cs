using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Toolkit.Levenshtein
{
    class Program
    {

        static double WeightedLevenshtein(string b1, string b2)
        {
            b1 = b1.ToUpper();
            b2 = b2.ToUpper();

            double[,] matrix = new double[b1.Length + 1, b2.Length + 1];

            for (int i = 1; i <= b1.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int i = 1; i <= b2.Length; i++)
            {
                matrix[0, i] = i;
            }

            for (int i = 1; i <= b1.Length; i++)
            {
                for (int j = 1; j <= b2.Length; j++)
                {
                    double distance_replace = matrix[(i - 1), (j - 1)];
                    if (b1[i - 1] != b2[j - 1])
                    {
                        // Cost of replace
                        distance_replace += Math.Abs((float)(b1[i - 1]) - b2[j - 1]) / ('Z' - 'A');
                    }

                    // Cost of remove = 1 
                    double distance_remove = matrix[(i - 1), j] + 1;
                    // Cost of add = 1
                    double distance_add = matrix[i, (j - 1)] + 1;

                    matrix[i, j] = Math.Min(distance_replace,
                                        Math.Min(distance_add, distance_remove));
                }
            }

            return matrix[b1.Length, b2.Length];
        }

        static void Test(string[] args)
        {
            double w1 = WeightedLevenshtein("THEATRE", "TNEATRE");
            double w2 = WeightedLevenshtein("THEATRE", "TOEATRE");

            Console.WriteLine("Distance between THEATRE and TNEATRE is: " + w1);
            Console.WriteLine("Distance between THEATRE and TOEATRE is: " + w2);
        }
    }
}
