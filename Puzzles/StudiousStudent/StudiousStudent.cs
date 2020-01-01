//You've been given a list of words to study and memorize. 
//Being a diligent student of language and the arts, you've 
//decided to not study them at all and instead make up pointless 
//games based on them. One game you've come up with is to see how you 
//can concatenate the words to generate the lexicographically lowest 
//possible string.

//Input As input for playing this game you will receive a 
// file containing an integer N, the number of word sets you need to 
//play your game against. This will be followed by N word sets, each starting 
//with an integer M, the number of words in the set, followed by M words. 
//All tokens in the input will be separated by some whitespace and, aside from 
//N and M, will consist entirely of lowercase letters.

//Output Your submission should contain the lexicographically shortest strings 
//for each corresponding word set, one per line and in order.

//Constraints 1 <= N <= 100
// 1 <= M <= 9
// 1 <= all word lengths <= 10
//Example input
//6 facebook hacker cup for studious students
//5 k duz q rc lvraw
//5 mybea zdr yubx xe dyroiy
//5 jibw ji jp bw jibw
//5 uiuy hopji li j dcyi

//Example output.5
//cupfacebookforhackerstudentsstudious
//duzklvrawqrc
//dyroiymybeaxeyubxzdr
//bwjibwjibwjijp
//dcyihopjijliuiu


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Protsyk.Puzzles {
    class Program {
        static int N;

        static void Test(string[] args) {
            string s = File.ReadAllText(args[0]);
            string[] splits = s.Split(new char[] { '\n', '\r' },
                              StringSplitOptions.RemoveEmptyEntries);

            N = int.Parse(splits[0]);
            for (int i = 1; i < N + 1; i++) {
                string[] words = splits[i]
                                         .Split(' ')
                                         .Skip(1)
                                         .ToArray();

                Console.WriteLine(Solve(words));
            }
        }

        private static string Solve(string[] words) {
            var prefix = string.Empty;
            var sorted = new List<String>(words);

            while (true) {
                sorted = sorted.OrderBy(x => x).ToList();
                if (sorted.Count == 0) break;

                string pr = sorted.First();
                string sf = sorted.Skip(1)
                                .Where(a => a.StartsWith(pr))
                                .Select(s => s.Substring(pr.Length))
                                .Where(s => !string.IsNullOrEmpty(s))
                                .OrderBy(x => x + pr)
                                .FirstOrDefault();

                if (string.Compare(pr + pr, pr + sf) < 0)
                    sf = null;

                prefix += pr + sf;
                sorted.Remove(pr + sf);
            }

            return prefix;
        }
    }
}