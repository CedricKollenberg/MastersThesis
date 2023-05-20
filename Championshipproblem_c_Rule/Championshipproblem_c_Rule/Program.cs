using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Championshipproblem_c_Rule
{
    class Program
    {
        static string data = @"D:\Documents\Studium\Hochschule Fulda\Master\Masterarbeit\Implementierungen\Data\";
        static int c = 2;

        static void Main(string[] args)
        {
            SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 10, 1, null, 25);
        }

        public static void AbortTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;Mode;Iterations;Runtime");

            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int i = 0; i <= 2; i++)
                {
                    SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 6, i, csvData, 25);
                    SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 6, i, csvData, 20);
                }
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_AbortTest.csv", csvData.ToString());
        }

        public static void SimpleRuleTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;SimpleRule;Iterations;Runtime");

            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int rule = 7; rule <= 9; rule++)
                {
                    SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 5, 1, csvData, 25, false, int.MaxValue, 0, rule);
                    SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 5, 1, csvData, 20, false, int.MaxValue, 0, rule);
                }

                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 5, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 5, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_SimpleRuleTest.csv", csvData.ToString());
        }

        public static void RuleTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;Rule;Abort 1;Abort 5;Abort 6;Aborted Teams;Iterations;Runtime");

            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int rule = 1; rule <= 6; rule++)
                {
                    SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 4, 1, csvData, 25, false, int.MaxValue, rule);
                    SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 4, 1, csvData, 20, false, int.MaxValue, rule);
                }

                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 4, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 4, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_RuleTest.csv", csvData.ToString());
        }

        public static void VariableDepthTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;Level;Iterations;Runtime");

            for (int j = 0; j < 20; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int level = 1; level <= 50; level++)
                {
                    for (int k = 1; k <= 2; k++)
                    {
                        SolveChampionshipProblems(k, 0, 0.1, 0.0, 0.0, true, 1000, 3, 1, csvData, 25, true, level);
                        SolveChampionshipProblems(0, k, 0.0, 0.15, 0.175, true, 1000, 3, 1, csvData, 20, true, level);
                    }
                }

                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000, 3, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000, 3, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_VariableDepthTest.csv", csvData.ToString());
        }

        public static void ComplexityTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Limit;Iterations;Runtime");

            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine("Iteration " + i);

                SolveChampionshipProblems(0, 0, 0, 0, 0, true, 1000, 2, 1, csvData);

                SolveChampionshipProblems(1, 0, 0.1, 0, 0, true, 1000, 2, 1, csvData, 0);
                SolveChampionshipProblems(1, 0, 0.1, 0, 0, true, 1000, 2, 1, csvData, 25);

                for (double kappa = 0.15; kappa <= 0.25; kappa += 0.025)
                {
                    for (double zeta = 0.15; zeta <= 0.25; zeta += 0.025)
                    {
                        SolveChampionshipProblems(0, 1, 0, kappa, zeta, true, 1000, 2, 1, csvData, 0);
                        SolveChampionshipProblems(0, 1, 0, kappa, zeta, true, 1000, 2, 1, csvData, 20);
                    }
                }
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_ComplexityTest.csv", csvData.ToString());
        }

        public static void MultiIterationTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Iterations;Runtime");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Iteration: " + i);

                for (int T1 = 0; T1 <= 1; T1++)
                {
                    for (int T2 = 0; T2 <= 1; T2++)
                    {
                        for (double epsilon = 0.1; epsilon <= 0.5; epsilon += 0.1)
                        {
                            for (double kappa = 0.0; kappa <= 0.15; kappa += 0.05)
                            {
                                for (double zeta = 0.0; zeta <= 0.15; zeta += 0.05)
                                {
                                    SolveChampionshipProblems(T1, T2, epsilon, kappa, zeta, true, 1000, 1, 1, csvData);
                                }
                            }
                        }
                    }
                }
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_MultiIterationTest.csv", csvData.ToString());
        }

        public static void CompleteTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Order;Iterations;Runtime");

            for (int T1 = 0; T1 <= 3; T1++)
            {
                for (int T2 = 0; T2 <= 3; T2++)
                {
                    for (double epsilon = 0.1; epsilon <= 1.0; epsilon += 0.1)
                    {
                        for (double kappa = 0.0; kappa <= 0.25; kappa += 0.05)
                        {
                            for (double zeta = 0.0; zeta <= 0.25; zeta += 0.05)
                            {
                                for (int order = 0; order <= 1; order++)
                                {
                                    SolveChampionshipProblems(T1, T2, epsilon, kappa, zeta, Convert.ToBoolean(order), 1000, 0, 1, csvData);
                                }
                            }
                        }
                    }
                }
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_CompleteTest.csv", csvData.ToString());
        }

        public static void TestCriticalProblems()
        {
            int i = 0;
            int maxIterations = 1_000;
            int[] gamesPerGameday = new int[4] { 9, 9, 9, 9 };
            int[] gamedays = new int[4] { 20, 22, 23, 23 };
            string[] champions = new string[4] { "SC Tasmania 1900 Berlin (-1973)", "Blau-Weiß 90 Berlin (-1992)", "Hertha BSC", "SC Freiburg" };

            foreach (string csv in Directory.GetFiles(data + "Critical\\", "*_*.csv"))
            {
                string season = Path.GetFileNameWithoutExtension(csv);

                var csvData = new StringBuilder();
                csvData.AppendLine("Season:" + season + ";Gameday:" + gamedays[i] + ";Champion:" + champions[i]);
                csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Order;Iterations;Runtime;Distance to Champion");

                for (int T1 = 0; T1 < 3; T1++)
                {
                    for (int T2 = 0; T2 < 4; T2++)
                    {
                        for (double epsilon = 0.05; epsilon <= 1.0; epsilon += 0.05)
                        {
                            for (double kappa = 0.0; kappa <= 1.0; kappa += 0.05)
                            {
                                for (double zeta = 0.0; zeta <= 1.5; zeta += 0.05)
                                {
                                    for (int order = 0; order < 2; order++)
                                    {
                                        bool b = Convert.ToBoolean(order);

                                        Stopwatch s = new Stopwatch();

                                        s.Start();

                                        ChampionshipProblem p = new ChampionshipProblem(csv, gamesPerGameday[i], c, gamedays[i], champions[i], T1, T2, epsilon, kappa, zeta, b);
                                        p.setMaxIterations(maxIterations);

                                        p.applyRules();
                                        int counter = p.applyHeuristics(1, 0);

                                        s.Stop();

                                        int diff = 0;

                                        foreach (Team k in p.getTeams().Values)
                                        {
                                            diff -= k.getDiffPoints();
                                        }

                                        if (counter < 0)
                                        {
                                            counter *= -1;
                                        }

                                        csvData.AppendLine(T1 + ";" + T2 + ";" + epsilon + ";" + kappa + ";" + zeta + ";" + b + ";" + counter + ";" + s.Elapsed + ";" + diff);

                                        Console.WriteLine(T1 + ";" + T2 + ";" + epsilon + ";" + kappa + ";" + zeta + ";" + b);
                                    }
                                }
                            }
                        }
                    }
                }

                File.WriteAllText(data + "\\Results\\RuntimeResult_" + season + ".csv", csvData.ToString());

                i++;
            }
        }

        public static void SolveChampionshipProblems(int T1, int T2, double epsilon, double kappa, double zeta, bool order, int maxIterations, int mode, int heuristicMode = 1, StringBuilder csvData = null, int limit = 0, bool depthLevel = false, int level = int.MaxValue, int rule = 0, int simpleRule = 0)
        {
            int iterations = 0;
            int aborted = 0;
            Dictionary<int, int> aborts = new();
            aborts.Add(1, 0);
            aborts.Add(5, 0);
            aborts.Add(6, 0);

            if (mode.Equals(10) || mode.Equals(100))
            {
                csvData = new();
            }

            Stopwatch s = new Stopwatch();

            s.Start();

            foreach (string csv in Directory.GetFiles(data, "*_*.csv"))
            {
                string season = Path.GetFileNameWithoutExtension(csv);

                if (mode.Equals(10))
                {
                    csvData.AppendLine(season);
                    csvData.AppendLine("Gameday;Possible champions;Timeouts;Heuristic aborts;Avg distance to champion");
                }

                string[] lines = File.ReadAllLines(csv);

                List<string> teams = new();
                List<string> abortedTeams = new();

                int i = 1;

                while (true)
                {
                    string line = lines[i];

                    string[] entries = line.Split(",");

                    if (!Convert.ToInt32(entries[0]).Equals(1))
                    {
                        break;
                    }

                    teams.Add(entries[2]);
                    teams.Add(entries[4]);

                    i++;
                }

                int gamesPerGameday = i - 1;
                int numberGamedays = gamesPerGameday * 4 - 2;

                for (i = numberGamedays / 2 + 1; i < numberGamedays; i++)
                {
                    int counter = 0;
                    double overallDiff = 0;
                    int timeout = 0;
                    int heuristicsNotPossible = 0;

                    for (int j = 0; j < gamesPerGameday * 2; j++)
                    {
                        if (abortedTeams.Contains(teams[j]))
                        {
                            aborted++;
                            continue;
                        }

                        ChampionshipProblem p = new ChampionshipProblem(csv, gamesPerGameday, c, i, teams[j], T1, T2, epsilon, kappa, zeta, order);
                        p.setMaxIterations(maxIterations);

                        if (mode.Equals(100))
                        {
                            int result = p.bruteForce();

                            if (result >= 0)
                            {
                                iterations += result;
                            }
                            else if (result.Equals(-maxIterations))
                            {
                                iterations += maxIterations;
                            }
                            else
                            {
                                abortedTeams.Add(teams[j]);
                                iterations -= result;
                            }
                        }
                        else if (p.applyRules(rule, aborts))
                        {
                            if (p.getOverallSurplus() < limit)
                            {
                                p.removeDepths();
                            }

                            if (depthLevel)
                            {
                                p.setVariableDepthLevel(level);
                            }

                            int result = p.applyHeuristics(heuristicMode, 0, simpleRule);

                            if (result >= 0)
                            {
                                int diff = 0;

                                foreach (Team k in p.getTeams().Values)
                                {
                                    diff -= k.getDiffPoints();
                                }

                                counter++;
                                overallDiff += diff;

                                iterations += result;
                            }
                            else if (result.Equals(-maxIterations))
                            {
                                timeout++;
                                iterations += maxIterations;
                            }
                            else
                            {
                                abortedTeams.Add(teams[j]);
                                heuristicsNotPossible++;
                                iterations -= result;
                            }
                        }
                        else
                        {
                            abortedTeams.Add(teams[j]);
                        }
                    }

                    if (mode.Equals(10))
                    {
                        csvData.AppendLine(i + ";" + counter + ";" + timeout + ";" + heuristicsNotPossible + ";" + (double)overallDiff / counter);
                    }
                }

                if (mode.Equals(10))
                {
                    csvData.AppendLine();
                }
            }

            s.Stop();

            string configuration = T1 + "_" + T2 + "_" + epsilon + "_" + kappa + "_" + zeta + "_" + order + "_" + limit + "_" + level + "_" + rule + "_" + simpleRule;

            Console.WriteLine(configuration);

            switch (mode)
            {
                case 0: // Complete Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + epsilon + ";" + kappa + ";" + zeta + ";" + order + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 1: // Multi Iteration Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + epsilon + ";" + kappa + ";" + zeta + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 2: // Complexity Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + epsilon + ";" + kappa + ";" + zeta + ";" + limit + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 3: // Variable Depth Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + level + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 4: // Rule Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + rule + ";" + aborts[1] + ";" + aborts[5] + ";" + aborts[6] + ";" + aborted + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 5: // Simple Rule Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + simpleRule + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 6: // Abort Test
                    csvData.AppendLine(T1 + ";" + T2 + ";" + heuristicMode + ";" + iterations + ";" + s.Elapsed);

                    break;
                case 10: // Data
                    csvData.AppendLine("Iterations;Runtime");
                    csvData.AppendLine(iterations + ";" + s.Elapsed);
                    File.WriteAllText(data + "Results\\ChampionshipProblemResult_" + configuration + ".csv", csvData.ToString());

                    break;
                case 100: // Brute Force
                    csvData.AppendLine("Iterations;Runtime");
                    csvData.AppendLine(iterations + ";" + s.Elapsed);
                    File.WriteAllText(data + "Results\\BruteForce.csv", csvData.ToString());
                    break;
                default:
                    break;
            }
        }
    }
}