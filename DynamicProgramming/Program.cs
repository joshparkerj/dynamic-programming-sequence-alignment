using System;
using System.Linq;
using System.Collections.Generic;
// I ran this code with the following inputs:
//
// From p.139 of the book:
// AACAGTTACC
// TAAGGTCA
// Score: 7
//
// From p.149 of the book:
// CCGGGTTACCA
// GGAGTTCA
// Score: 8
//
// TATAT
// ATAT
// Score: 2
//
// ACTG
// TACT
// Score: 4
//
// TACT
// TACCT
// Score: 2
//
// ATTCATTAGATTCATCGGTGCGATGC
// ATATTAAATCAGGGTGCATC
// Score: 14
//
// AAAAAAAAAAAA
// AAAAAAAAAAAA
// Score: 0
//
// TACGTACGTACGT
// TACGTACGTAGA
// Score: 3
//
// ACACACTTACACAC
// AGACACTTAGACAC
// Score: 2
// 
// AACAGTTCACC
// TAAGGTCACC
// Score: 4
// 
// TAACAGTTCACC
// TAACGGTTCACC
// Score: 1
//
// GTCCAGTCGGGTG
// ACAGAACTTGTGC
// Score: 11

namespace DynamicProgramming
{
    class Program
    {

        static void Main(string[] args)
        {
            char[] seq1;
            char[] seq2;
            string qToQuit = "a";
            while (qToQuit != "q")
            {
                Console.WriteLine("please type in first sequence");
                seq1 = Console.ReadLine().ToCharArray();
                Console.WriteLine("please type in second sequence");
                seq2 = Console.ReadLine().ToCharArray();

                var sa = new SequenceAlignment(seq1, seq2);
                Console.WriteLine("Using the dynamic programming approach as described in the book, here is an optimal alignment:");
                Console.WriteLine($"Score: {sa.DP_Opt()}");
                Console.WriteLine(sa.OptimalAlignment());
                Console.WriteLine("Enter q to quit. Anything else to try again.");
                qToQuit = Console.ReadLine();
            }
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
            return DP()[0, 0];
        }
        
        public int[,] DP()
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
            return optValues;
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

        // This method goes on to actually find an optimal alignment as described in the book.
        public string OptimalAlignment()
        {
            int[,] optValues = DP();
            int i = 0;
            int j = 0;
            int m = x.Length;
            int n = y.Length;

            string optX = "";
            string optY = "";

            int penalty;

            Stack<int[]> path = new Stack<int[]>();

            while (i < m || j < n)
            {
                path.Push(new int[] { i, j });
                if (i == m)
                {
                    j++;
                    continue;
                }
                if (j == n)
                {
                    i++;
                    continue;
                }
                if (x[i] == y[j])
                {
                    penalty = 0;
                }
                else
                {
                    penalty = 1;
                }
                if (optValues[i,j+1] == optValues[i,j] - 2)
                {
                    j++;
                }
                else if (optValues[i+1,j] == optValues[i,j] - 2)
                {
                    i++;
                }
                else if (optValues[i+1,j+1] == optValues[i,j] - penalty)
                {
                    i++;
                    j++;
                }
                else
                {
                    Console.WriteLine("there's a problem in the path-finding portion of the method.");
                    return "error";
                }
            }
            i = m;
            j = n;
            while (i > 0 || j > 0)
            {
                var nextStep = path.Pop();
                if (nextStep[0] == i-1 && nextStep[1] == j-1)
                {
                    i = nextStep[0];
                    j = nextStep[1];
                    optX = $"{x[i]}{optX}";
                    optY = $"{y[j]}{optY}";
                }
                else if (nextStep[0] == i && nextStep[1] == j-1)
                {
                    i = nextStep[0];
                    j = nextStep[1];
                    optX = $"-{optX}";
                    optY = $"{y[j]}{optY}";
                }
                else if (nextStep[0] == i-1 && nextStep[1] == j)
                {
                    i = nextStep[0];
                    j = nextStep[1];
                    optX = $"{x[i]}{optX}";
                    optY = $"-{optY}";
                }
                else
                {
                    Console.WriteLine("there's a problem in the retrieval portion of the method");
                    return "error";
                }
            }
            return $"{optX}\n{optY}";
        }
    }
}
