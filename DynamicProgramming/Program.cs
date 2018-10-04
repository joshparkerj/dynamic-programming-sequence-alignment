using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
// I ran this code with the following inputs:
//
// From p.139 of the book:
// AACAGTTACC
// TAAGGTCA
// Score: 7. DAC time: 59 ms. DP time: 0 ms.
//
// From p.149 of the book:
// CCGGGTTACCA
// GGAGTTCA
// Score: 8
//
// Some that I invented:
// ACTG
// TACT
// Score: 4
//
// TACT
// TACCT
// Score: 2
//
// This one unfortunately wouldn't run using the recursive algorithm.
// It may be that the recursive algorithm is just too slow.
// 12 plus minutes of thinking about it was not enough...
// The dp algorithm handled it in 5 ms.
// ATTCATTAGATTCATCGGTGCGATGC
// ATATTAAATCAGGGTGCATC
// Score: 14 (according to dp algorithm)
//
// This one was almost unbearably slow with the recursive algorithm.
// However, I now suspect that part of the delay may have been due to the just-in-time compilation not kicking in yet.
// AAAAAAAAAAAA
// AAAAAAAAAAAA
// Score: 0
//
// Using a stopwatch, I found that the recursive algorithm took 26 seconds to think about this one.
// TACGTACGTACGT
// TACGTACGTAGA
// Score: 3
//
// ACACACTTACACAC
// AGACACTTAGACAC
// Score: 2. DAC time: 5.7 minutes. DP time: 0 ms.
// 
// AACAGTTCACC
// TAAGGTCACC
// Score: 4. DAC time: 800 ms. DP time: 0 ms.
// 
// TAACAGTTCACC
// TAACGGTTCACC
// Score: 1. DAC time: 10.8 seconds. DP time: 0 ms.
//
// GTCCAGTCGGGTG
// ACAGAACTTGTGC
// Score: 11. DAC time: 1 minute. DP time: 0 ms.

namespace DynamicProgramming
{
    class Program
    {

        static void Main(string[] args)
        {
            char[] seq1;
            char[] seq2;
            if (args.Length == 2)
            {
                seq1 = args[0].ToCharArray();
                seq2 = args[1].ToCharArray();
            }
            else
            {
                Console.WriteLine("please type in first sequence");
                seq1 = Console.ReadLine().ToCharArray();
                Console.WriteLine("please type in second sequence");
                seq2 = Console.ReadLine().ToCharArray();
            }
            var sa = new SequenceAlignment(seq1, seq2);
            /*
            Stopwatch stopwatch = new Stopwatch();

            int dac_score;
            Console.WriteLine("Using the recursive algorithm from the pseudocode in the book, the optimal score is:");
            stopwatch.Start();
            dac_score = sa.Opt(0, 0);
            stopwatch.Stop();
            Console.WriteLine(dac_score);
            Console.WriteLine($"The recursive code spent {stopwatch.ElapsedMilliseconds} ms thinking about it.");
            stopwatch.Reset();

            int dp_score;
            Console.WriteLine("Using the dynamic programming approach as described in the book, the optimal score is:");
            stopwatch.Start();
            dp_score = sa.DP_Opt();
            stopwatch.Stop();
            Console.WriteLine(dp_score);
            Console.WriteLine($"The dp code spent {stopwatch.ElapsedMilliseconds} ms thinking about it.");
            */
            Console.WriteLine("Using the dynamic programming approach as described in the book, here is an optimal alignment:");
            //Console.WriteLine(sa.OptimalAlignment());
            Console.WriteLine("press anything to quit");
            Console.ReadKey();
        }

    }
    class SequenceAlignment
    {
        private char[] x;
        private char[] y;

        public SequenceAlignment(char[] seq1, char[] seq2)
        {
            x = seq1;
            y = seq2;
        }

        // This is my implementation of the dynamic programming solution described on pp.142-144 of the textbook.
        // It returns the cost of an optimal alignment just like algorithm 3.12.
        // The book goes on to describe how to find an optimal alignment.
        // However, I wanted the two methods to return the same values.
        public int DP_Opt()
        {
            int m = x.Length;
            int n = y.Length;
            int[,] optValues = new int[m + 1, n + 1];
            int i = m;
            int j;
            int penalty;
            while (i >= 0)
            {
                j = n;
                while (j >= 0)
                {
                    if (i == m)
                    {
                        optValues[i, j] = 2 * (n - j);
                    }
                    else if (j == n)
                    {
                        optValues[i, j] = 2 * (m - i);
                    }
                    else
                    {
                        if (x[i] == y[j])
                        {
                            penalty = 0;
                        }
                        else
                        {
                            penalty = 1;
                        }
                        int[] neighbors = new int[] { penalty + optValues[i + 1, j + 1], 2 + optValues[i + 1, j], 2 + optValues[i, j + 1] };
                        optValues[i, j] = neighbors.Min();
                    }
                    j--;
                }
                i--;
            }
            return optValues[0, 0];
        }

        // This method comes from the pseudocode on p.142 of the textbook.
        // Algorithm 3.12 Sequence Alignment Using Divide-and-Conquer.
        // It is very inefficient, however only minor changes are needed to convert this to the efficient dynamic programming algorithm.
        // As the book explains, it only gives the cost of an optimal alignment; it does not produce one.
        public int Opt(int i, int j)
        {
            int m = x.Length;
            int n = y.Length;
            int penalty;
            if (i == m)
            {
                return 2 * (n - j);
            }
            else if (j == n)
            {
                return 2 * (m - i);
            }
            else
            {
                if (x[i] == y[j])
                {
                    penalty = 0;
                }
                else
                {
                    penalty = 1;
                }
                int[] answers = new int[] { penalty + Opt(i + 1, j + 1), 2 + Opt(i + 1, j), 2 + Opt(i, j + 1) };
                return answers.Min();
            }
        }

        // this method is based on DP_Opt.
        // It goes on to actually find an optimal alignment as described in the book.
        /* As it turns out, this whole method is fairly poorly thought out. 
         * I'll try again from DP_Opt using a different approach.
        public string OptimalAlignment()
        {
            int m = x.Length;
            int n = y.Length;
            int[,,] optValues = new int[m + 1, n + 1,3];
            int i = m;
            int j;
            int penalty;
            List<char> optX = new List<char>();
            List<char> optY = new List<char>();
            while (i >= 0)
            {
                j = n;
                while (j >= 0)
                {
                    if (i == m)
                    {
                        optValues[i, j,0] = 2 * (n - j);
                        optValues[i, j, 1] = i;
                        optValues[i, j, 2] = j + 1;
                    }
                    else if (j == n)
                    {
                        optValues[i, j,0] = 2 * (m - i);
                        optValues[i, j, 1] = i + 1;
                        optValues[i, j, 2] = j;
                    }
                    else
                    {
                        if (x[i] == y[j])
                        {
                            penalty = 0;
                        }
                        else
                        {
                            penalty = 1;
                        }
                        int[,] neighbors = new int[,] { { penalty + optValues[i + 1, j + 1, 0],i+1,j+1 }, { 2 + optValues[i + 1, j, 0],i+1,j }, { 2 + optValues[i, j + 1, 0],i,j+1 } };
                        if (neighbors[0,0] <= neighbors[1,0] && neighbors[0,0] <= neighbors[2,0])
                        {
                            optValues[i, j, 0] = neighbors[0, 0];
                            optValues[i, j, 1] = neighbors[0, 1];
                            optValues[i, j, 2] = neighbors[0, 2];
                        }
                        else if (neighbors[1,0] <= neighbors[0,0] && neighbors[1,0] <= neighbors[2,0])
                        {
                            optValues[i, j, 0] = neighbors[1, 0];
                            optValues[i, j, 1] = neighbors[1, 1];
                            optValues[i, j, 2] = neighbors[1, 2];
                        }
                        else if (neighbors[2,0] <= neighbors[0,0] && neighbors[2,0] <= neighbors[1,0])
                        {
                            optValues[i, j, 0] = neighbors[2, 0];
                            optValues[i, j, 1] = neighbors[2, 1];
                            optValues[i, j, 2] = neighbors[2, 2];
                        }
                        else
                        {
                            Console.WriteLine("There is some kind of problem in the optimizing portion of the OptimalAlignment method");
                            return "error";
                        }
                    }
                    j--;
                }
                i--;
            }
            i = 0;
            j = 0;
            while (i <= m && j <= n && !(i == m && j == n))
            {
                if (optValues[i,j,1] == i+1 && optValues[i,j,2] == j + 1)
                {
                    optX.Add(i < m ? x[i] : '-');
                    optY.Add(j < n ? y[j] : '-');
                }
                else if (optValues[i,j,1] == i+1 && optValues[i,j,2] == j)
                {
                    optX.Add(i < m ? x[i] : '-');
                    optY.Add('-');
                }
                else if (optValues[i,j,1] == i && optValues[i,j,2] ==j+1)
                {
                    optX.Add('-');
                    optY.Add(j < n ? y[j] : '-');
                }
                else
                {
                    Console.WriteLine("There is some kind of problem in the retrieval portion of the OptimalAlignment method");
                    return "error";
                }
                i = optValues[i, j, 1];
                j = optValues[i, j, 2];
            }
            return $"{new String(optX.ToArray())}\n{new String(optY.ToArray())}";
            
        }
        */
    }
}
