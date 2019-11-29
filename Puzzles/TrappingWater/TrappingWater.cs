using System;
using System.Collections.Generic;

namespace trap
{
    class Program
    {
        public static int SolveWithStack(int[] w)
        {
            int t = 0;
            var s = new Stack<(int, int, int h)>();
            for (int i = 0; i < w.Length; i++)
            {
                while (s.Count > 0 && s.Peek().h < w[i])
                {
                    (int js, int je, int jh) = s.Pop();
                    if (s.Count == 0)
                    {
                        break;
                    }

                    (int ks, int ke, int kh) = s.Pop();
                    if (kh < w[i])
                    {
                        t += (je - js + 1) * (kh - jh);
                        s.Push((ks, je, kh));
                    }
                    else if (kh > w[i])
                    {
                        t += (je - js + 1) * (w[i] - jh);
                        s.Push((ks, ke, kh));
                        s.Push((js, je, w[i]));
                    }
                    else
                    {
                        t += (je - js + 1) * (w[i] - jh);
                        s.Push((ks, je, kh));
                    }
                }

                if (s.Count == 0 || s.Peek().h > w[i])
                {
                    s.Push((i, i, w[i]));
                }
                else if (s.Peek().h == w[i])
                {
                    (int si, _, _) = s.Pop();
                    s.Push((si, i, w[i]));
                }
            }
            return t;
        }

        public static int SolveWithTrick(int[] w)
        {
            if (w.Length < 3)
            {
                return 0;
            }

            int maxI = 0;
            for (int i=0; i<w.Length; ++i)
            {
                if (w[maxI] < w[i]) maxI = i;
            }
            
            int water = 0;
            int maxL = w[0];
            for (int i=1; i<maxI; ++i)
            {
                if (maxL>w[i]) water += maxL - w[i];
                if (maxL<w[i]) maxL = w[i];
            }

            int maxR = w[w.Length-1];
            for (int i=w.Length-2; i>maxI; --i)
            {
                if (maxR>w[i]) water += maxR - w[i];
                if (maxR<w[i]) maxR = w[i];
            }
            
            return water;
        }

        static void Main(string[] args)
        {
            //trapping water
            {
                // Answer 6
                int[] w = new int[] { 3, 1, 2, 3, 1, 2, 4 };

                // Answer 22
                //int[] w = new int[] { 10, 4, 1, 1, 2, 5, 7 };

                
                Console.WriteLine("You can trap " + SolveWithStack(w));
                Console.WriteLine("You can trap " + SolveWithTrick(w));
            }
        }
    }
}
