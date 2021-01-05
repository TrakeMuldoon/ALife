using System;
using System.Collections.Generic;
namespace QLearningTutorial
{
    class QLearningProgram
    {
        //https://www.freecodecamp.org/news/an-introduction-to-reinforcement-learning-4339519de419/
        //https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/august/test-run-introduction-to-q-learning-using-csharp#:~:text=%20Introduction%20to%20Q-Learning%20Using%20C%23%20%201,the%20mathematical%20Bellman%20equation%20and%20is...%20More%20
        static Random rnd = new Random(1);
        static void Main(string[] args)
        {
            Console.WriteLine("Begin Q-learning maze demo");
            Console.WriteLine("Setting up maze and rewards");
            int ns = 12;
            int[][] FT = CreateMaze(ns);
            double[][] R = CreateReward(ns);
            double[][] Q = CreateQuality(ns);
            Console.WriteLine("Analyzing maze using Q-learning");
            int goal = 11;
            double gamma = 0.5;
            double learnRate = 0.5;
            int maxEpochs = 1000;
            Train(FT, R, Q, goal, gamma, learnRate, maxEpochs);
            Console.WriteLine("Done. Q matrix: ");
            Print(Q);
            Console.WriteLine("Using Q to walk from cell 8 to 11");
            Walk(8, 11, Q);
            Console.WriteLine("End demo");
            Console.ReadLine();
        }
        static void Print(double[][] Q)
        {
            int ns = Q.Length;
            Console.WriteLine("[0] [1] . . [11]");
            for(int i = 0; i < ns; ++i)
            {
                for(int j = 0; j < ns; ++j)
                {
                    Console.Write(Q[i][j].ToString("F2") + " ");
                }
                Console.WriteLine();
            }
        }
        static int[][] CreateMaze(int ns)
        {
            int[][] FT = new int[ns][];
            for(int i = 0; i < ns; ++i) FT[i] = new int[ns];
            FT[0][1] = FT[0][4] = FT[1][0] = FT[1][5] = FT[2][3] = 1;
            FT[2][6] = FT[3][2] = FT[3][7] = FT[4][0] = FT[4][8] = 1;
            FT[5][1] = FT[5][6] = FT[5][9] = FT[6][2] = FT[6][5] = 1;
            FT[6][7] = FT[7][3] = FT[7][6] = FT[7][11] = FT[8][4] = 1;
            FT[8][9] = FT[9][5] = FT[9][8] = FT[9][10] = FT[10][9] = 1;
            FT[11][11] = 1;  // Goal
            return FT;
        }

        static double[][] CreateReward(int ns)
        {
            double[][] R = new double[ns][];
            for(int i = 0; i < ns; ++i) R[i] = new double[ns];
            R[0][1] = R[0][4] = R[1][0] = R[1][5] = R[2][3] = -0.1;
            R[2][6] = R[3][2] = R[3][7] = R[4][0] = R[4][8] = -0.1;
            R[5][1] = R[5][6] = R[5][9] = R[6][2] = R[6][5] = -0.1;
            R[6][7] = R[7][3] = R[7][6] = R[7][11] = R[8][4] = -0.1;
            R[8][9] = R[9][5] = R[9][8] = R[9][10] = R[10][9] = -0.1;
            R[7][11] = 10.0;  // Goal
            return R;
        }

        static double[][] CreateQuality(int ns)
        {
            double[][] Q = new double[ns][];
            for(int i = 0; i < ns; ++i)
                Q[i] = new double[ns];
            return Q;
        }

        static List<int> GetPossNextStates(int s,
          int[][] FT)
        {
            List<int> result = new List<int>();
            for(int j = 0; j < FT.Length; ++j)
                if(FT[s][j] == 1) result.Add(j);
            return result;
        }

        static int GetRandNextState(int s, int[][] FT)
        {
            List<int> possNextStates = GetPossNextStates(s, FT);
            int ct = possNextStates.Count;
            int idx = rnd.Next(0, ct);
            return possNextStates[idx];
        }

        static void Train(int[][] FT, double[][] R, double[][] Q,
          int goal, double gamma, double lrnRate,
          int maxEpochs)
        {
            for(int epoch = 0; epoch < maxEpochs; ++epoch)
            {
                int currState = rnd.Next(0, R.Length);
                while(true)
                {
                    int nextState = GetRandNextState(currState, FT);
                    List<int> possNextNextStates = GetPossNextStates(nextState, FT);
                    double maxQ = double.MinValue;
                    for(int j = 0; j < possNextNextStates.Count; ++j)
                    {
                        int nns = possNextNextStates[j];  // short alias
                        double q = Q[nextState][nns];
                        if(q > maxQ) maxQ = q;
                    }
                    Q[currState][nextState] =
                      ((1 - lrnRate) * Q[currState][nextState]) +
                      (lrnRate * (R[currState][nextState] + (gamma * maxQ)));
                    currState = nextState;
                    if(currState == goal) break;
                } // while
            } // for
        } // Train

        static void Walk(int start, int goal, double[][] Q)
        {
            int curr = start; int next;
            Console.Write(curr + "->");
            while(curr != goal)
            {
                next = ArgMax(Q[curr]);
                Console.Write(next + "->");
                curr = next;
            }
            Console.WriteLine("done");
        }

        static int ArgMax(double[] vector)
        {
            double maxVal = vector[0]; int idx = 0;
            for(int i = 0; i < vector.Length; ++i)
            {
                if(vector[i] > maxVal)
                {
                    maxVal = vector[i]; idx = i;
                }
            }
            return idx;
        }
    } // Program
} // ns
