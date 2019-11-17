using System;
using System.Collections.Generic;

namespace trap
{
    class Program
    {
        static void Main(string[] args)
        {
            //trapping water
            {
                // Answer 6
                int[] w = new int[] { 3, 1, 2, 3, 1, 2, 4 };

                // Answer 22
                //int[] w = new int[] { 10, 4, 1, 1, 2, 5, 7 };

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
                Console.WriteLine("You can trap " + t);
            }
        }
    }
}
