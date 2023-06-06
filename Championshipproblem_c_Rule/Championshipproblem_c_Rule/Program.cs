using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Championshipproblem_c_Rule
{
    class Program
    {
        // program variables
        static string data = @"..\..\..\..\..\Data\";
        static int c = 3;

        static void Main(string[] args)
        {
            SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 10, 1, null, 25);
        }

        // test abort methods
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

        // test efficiency of simple rules
        public static void SimpleRuleTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;SimpleRule;Iterations;Runtime");

            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int rule = 7; rule <= 9; rule++) // vary simple rule
                {
                    SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 5, 1, csvData, 25, false, int.MaxValue, 0, rule);
                    SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 5, 1, csvData, 20, false, int.MaxValue, 0, rule);
                }

                // compare to standard configuration
                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 5, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 5, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_SimpleRuleTest.csv", csvData.ToString());
        }

        // test efficiency of rules
        public static void RuleTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;Rule;Abort 1;Abort 5;Abort 6;Aborted Teams;Iterations;Runtime");

            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int rule = 1; rule <= 6; rule++) // vary rule
                {
                    SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 4, 1, csvData, 25, false, int.MaxValue, rule);
                    SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 4, 1, csvData, 20, false, int.MaxValue, rule);
                }

                // compare to standard configuration
                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000000, 4, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000000, 4, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_RuleTest.csv", csvData.ToString());
        }

        // test variable depth
        // decrement depth if level is reached
        public static void VariableDepthTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;Level;Iterations;Runtime");

            for (int j = 0; j < 20; j++)
            {
                Console.WriteLine("Iteration " + j);

                for (int level = 1; level <= 50; level++) // vary level
                {
                    for (int k = 1; k <= 2; k++) // vary depth
                    {
                        SolveChampionshipProblems(k, 0, 0.1, 0.0, 0.0, true, 1000, 3, 1, csvData, 25, true, level);
                        SolveChampionshipProblems(0, k, 0.0, 0.15, 0.175, true, 1000, 3, 1, csvData, 20, true, level);
                    }
                }

                // compare to standard configuration
                SolveChampionshipProblems(1, 0, 0.1, 0.0, 0.0, true, 1000, 3, 1, csvData, 25);
                SolveChampionshipProblems(0, 1, 0.0, 0.15, 0.175, true, 1000, 3, 1, csvData, 20);
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_VariableDepthTest.csv", csvData.ToString());
        }

        // test complexity of problems
        // if complexity is below the limit, reset depth
        // otherwise keep configuration
        public static void ComplexityTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Limit;Iterations;Runtime");

            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine("Iteration " + i);

                // compare to configuration without depth
                SolveChampionshipProblems(0, 0, 0, 0, 0, true, 1000, 2, 1, csvData);

                SolveChampionshipProblems(1, 0, 0.1, 0, 0, true, 1000, 2, 1, csvData, 0); // no limit
                SolveChampionshipProblems(1, 0, 0.1, 0, 0, true, 1000, 2, 1, csvData, 25); // limit of 25

                // additional variation of kappa and zeta
                for (double kappa = 0.15; kappa <= 0.25; kappa += 0.025)
                {
                    for (double zeta = 0.15; zeta <= 0.25; zeta += 0.025)
                    {
                        SolveChampionshipProblems(0, 1, 0, kappa, zeta, true, 1000, 2, 1, csvData, 0); // no limit
                        SolveChampionshipProblems(0, 1, 0, kappa, zeta, true, 1000, 2, 1, csvData, 20); // limit of 20
                    }
                }
            }

            File.WriteAllText(data + "\\Results\\RuntimeResult_ComplexityTest.csv", csvData.ToString());
        }

        // test parameters in mulitple iterations
        public static void MultiIterationTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Iterations;Runtime");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Iteration: " + i);

                for (int T1 = 0; T1 <= 1; T1++) // vary depth 1
                {
                    for (int T2 = 0; T2 <= 1; T2++) // vary depth 2
                    {
                        for (double epsilon = 0.1; epsilon <= 0.5; epsilon += 0.1) // vary epsilon
                        {
                            for (double kappa = 0.0; kappa <= 0.15; kappa += 0.05) // vary kappa
                            {
                                for (double zeta = 0.0; zeta <= 0.15; zeta += 0.05) // vary zeta
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

        // test all cases
        // width variation of paramters
        public static void CompleteTest()
        {
            var csvData = new StringBuilder();

            csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Order;Iterations;Runtime");

            for (int T1 = 0; T1 <= 3; T1++) // vary depth 1
            {
                for (int T2 = 0; T2 <= 3; T2++) // vary depth 2
                {
                    for (double epsilon = 0.1; epsilon <= 1.0; epsilon += 0.1) // vary epsilon
                    {
                        for (double kappa = 0.0; kappa <= 0.25; kappa += 0.05) // vary kappa
                        {
                            for (double zeta = 0.0; zeta <= 0.25; zeta += 0.05) // vary zeta
                            {
                                for (int order = 0; order <= 1; order++) // vary order
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

        // test critical problems
        // extreme width variation of parameters
        public static void TestCriticalProblems()
        {
            // define situations of critical problems
            int i = 0;
            int maxIterations = 1_000;
            int[] gamesPerGameday = new int[4] { 9, 9, 9, 9 };
            int[] gamedays = new int[4] { 20, 22, 23, 23 };
            string[] champions = new string[4] { "SC Tasmania 1900 Berlin (-1973)", "Blau-Weiß 90 Berlin (-1992)", "Hertha BSC", "SC Freiburg" };

            foreach (string csv in Directory.GetFiles(data + "Critical\\", "*_*.csv")) // get all critical problems
            {
                string season = Path.GetFileNameWithoutExtension(csv);

                var csvData = new StringBuilder();
                csvData.AppendLine("Season:" + season + ";Gameday:" + gamedays[i] + ";Champion:" + champions[i]);
                csvData.AppendLine("T1;T2;epsilon;kappa;zeta;Order;Iterations;Runtime;Distance to Champion");

                for (int T1 = 0; T1 < 3; T1++) // vary depth 1
                {
                    for (int T2 = 0; T2 < 4; T2++) // vary depth 2
                    {
                        for (double epsilon = 0.05; epsilon <= 1.0; epsilon += 0.05) // vary epsilon
                        {
                            for (double kappa = 0.0; kappa <= 1.0; kappa += 0.05) // vary kappa
                            {
                                for (double zeta = 0.0; zeta <= 1.5; zeta += 0.05) // vary zeta
                                {
                                    for (int order = 0; order < 2; order++) // vary order
                                    {
                                        bool b = Convert.ToBoolean(order);

                                        Stopwatch s = new Stopwatch();

                                        s.Start(); // start test runtime

                                        ChampionshipProblem p = new ChampionshipProblem(csv, gamesPerGameday[i], c, gamedays[i], champions[i], T1, T2, epsilon, kappa, zeta, b); // create championship problem
                                        p.setMaxIterations(maxIterations); // set maximum of iterations

                                        p.applyRules(); // apply rules to reduce problem size
                                        int counter = p.applyHeuristics(1, 0); // apply heursitic with exit method 1

                                        s.Stop(); // stop test runtime

                                        // sum up distance of all teams to champion
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

        //calculate all championship problems
        public static void SolveChampionshipProblems(int T1, int T2, double epsilon, double kappa, double zeta, bool order, int maxIterations, int mode, int heuristicMode = 1, StringBuilder csvData = null, int limit = 0, bool depthLevel = false, int level = int.MaxValue, int rule = 0, int simpleRule = 0)
        {
            // initialize attributes
            int iterations = 0; // counter for iterations
            int aborted = 0; // counter for aborted teams
            Dictionary<int, int> aborts = new(); // counter of rule specific aborts
            aborts.Add(1, 0);
            aborts.Add(5, 0);
            aborts.Add(6, 0);

            if (mode.Equals(10) || mode.Equals(100))
            {
                csvData = new();
            }

            Stopwatch s = new Stopwatch();

            s.Start(); // start runtime measurement

            foreach (string csv in Directory.GetFiles(data, "*_*.csv")) // get all data files
            {
                string season = Path.GetFileNameWithoutExtension(csv); // get season name

                if (mode.Equals(10))
                {
                    csvData.AppendLine(season);
                    csvData.AppendLine("Gameday;Possible champions;Timeouts;Heuristic aborts;Avg distance to champion");
                }

                string[] lines = File.ReadAllLines(csv); // get all lines

                // initialize list of teams and aborted teams
                List<string> teams = new();
                List<string> abortedTeams = new();

                int i = 1;

                // get all teams of the season
                while (true)
                {
                    string line = lines[i];

                    string[] entries = line.Split(",");

                    if (!Convert.ToInt32(entries[0]).Equals(1)) // break if all teams are collected
                    {
                        break;
                    }

                    teams.Add(entries[2]);
                    teams.Add(entries[4]);

                    i++;
                }

                // set number of gamedays and games per gameday
                int gamesPerGameday = i - 1;
                int numberGamedays = gamesPerGameday * 4 - 2;

                for (i = numberGamedays / 2 + 1; i < numberGamedays; i++) // only the second half of the season is relevant
                {
                    // initialize counters
                    int counter = 0;
                    double overallDiff = 0;
                    int timeout = 0;
                    int heuristicsNotPossible = 0;

                    for (int j = 0; j < gamesPerGameday * 2; j++) // execute for every team
                    {
                        if (abortedTeams.Contains(teams[j])) // abort if team did not lead to a solution on an earlier gameday
                        {
                            aborted++;
                            continue;
                        }

                        ChampionshipProblem p = new ChampionshipProblem(csv, gamesPerGameday, c, i, teams[j], T1, T2, epsilon, kappa, zeta, order); // create championship problem object
                        p.setMaxIterations(maxIterations); // set max iterations

                        if (mode.Equals(100)) // brute force mode
                        {
                            int result = p.bruteForce(); // execute brute force

                            if (result >= 0) // solution
                            {
                                iterations += result;
                            }
                            else if (result.Equals(-maxIterations)) // timeout
                            {
                                iterations += maxIterations;
                            }
                            else // no solution
                            {
                                abortedTeams.Add(teams[j]);
                                iterations -= result;
                            }
                        }
                        else if (p.applyRules(rule, aborts)) // apply redcution rules
                        {
                            if (p.getOverallSurplus() < limit) // check complexity
                            {
                                p.removeDepths(); // set depths to 0
                            }

                            if (depthLevel)
                            {
                                p.setVariableDepthLevel(level); // set variable depth levels
                            }

                            int result = p.applyHeuristics(heuristicMode, 0, simpleRule); // apply heuristics

                            if (result >= 0) // solution
                            {
                                int diff = 0;

                                // get distance to the champion
                                foreach (Team k in p.getTeams().Values)
                                {
                                    diff -= k.getDiffPoints();
                                }

                                counter++;
                                overallDiff += diff;

                                iterations += result;
                            }
                            else if (result.Equals(-maxIterations)) // timeout
                            {
                                timeout++;
                                iterations += maxIterations;
                            }
                            else // no solution
                            {
                                abortedTeams.Add(teams[j]);
                                heuristicsNotPossible++;
                                iterations -= result;
                            }
                        }
                        else // abort because of rules
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

            s.Stop(); // stop runtime measurement

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