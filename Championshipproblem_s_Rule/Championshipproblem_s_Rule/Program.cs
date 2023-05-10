using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Championshipproblem_s_Rule
{ 
    class Program
    {
        static string data = @"D:\Documents\Studium\Hochschule Fulda\Master\Masterarbeit\Implementierungen\Data";
        static int s = 2;

        static void Main(string[] args)
        {
            int position = 1;

            StringBuilder csvData = new();

            csvData.AppendLine("Algorithm;Data Structure;Runtime;Iterations;AddNode;RemoveNode;AddEdge;RemoveEdge;GetEdges;GetNeighbours;GetPredecessors;SetCapacity;AddCapacity;GetCapacity;GetSize");

            for (int k = 0; k < 10; k++)
            {
                Console.WriteLine("Iteration " + k);

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        SolveChampionshipproblems(position, i, j, csvData);
                    }
                }
            }

            File.WriteAllText(data + "Results\\ChampionshipProblemResult_s-Rule.csv", csvData.ToString());
        }

        public static void SolveChampionshipproblems(int position, int algorithmMode, int dataStructure, StringBuilder csvData)
        {
            string modeName = null;
            string dataStructureName = null;

            switch (algorithmMode)
            {
                case 0:
                    modeName = "Edmonds_Karp";
                    break;
                case 1:
                    modeName = "Dinic";
                    break;
                case 2:
                    modeName = "Push_Relabel";
                    break;
                case 3:
                    modeName = "Relabel_To_Front";
                    break;
                default:
                    return;
            }

            switch (dataStructure)
            {
                case 0:
                    dataStructureName = "AdjacencyMatrix";
                    break;
                case 1:
                    dataStructureName = "AdjacencyList";
                    break;
                default:
                    return;
            }

            int[] counts = new int[11];
            for (int l = 0; l < 11; l++)
            {
                counts[l] = 0;
            }

            int iterations = 0;

            Console.WriteLine(modeName + "_" + dataStructureName);

            Stopwatch timer = new Stopwatch();

            timer.Start();

            foreach (string csv in Directory.GetFiles(data, "*_*.csv"))
            {
                string season = Path.GetFileNameWithoutExtension(csv);

                Console.WriteLine(season);

                string[] lines = File.ReadAllLines(csv);

                List<string> teamNames = new();
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

                    teamNames.Add(entries[2]);
                    teamNames.Add(entries[4]);

                    i++;
                }

                int gamesPerGameday = i - 1;
                int numberGamedays = gamesPerGameday * 4 - 2;

                for (i = numberGamedays / 2 + 1; i < numberGamedays; i++)
                {
                    int counter = 0;

                    foreach (string k in teamNames)
                    {
                        Dictionary<string, Team> teams = new();

                        if (abortedTeams.Contains(k))
                        {
                            continue;
                        }

                        int consideredGames = i * gamesPerGameday;

                        // results
                        for (int j = 1; j <= consideredGames; j++)
                        {
                            string line = lines[j];

                            string[] entries = line.Split(",");

                            string homeTeam = entries[2];
                            string awayTeam = entries[4];
                            string result = entries[3];

                            if (j <= gamesPerGameday)
                            {
                                teams.Add(homeTeam, new Team(homeTeam,0));
                                teams.Add(awayTeam, new Team(awayTeam,0));
                            }

                            Team h = teams[homeTeam];
                            Team a = teams[awayTeam];

                            int homegoals = Convert.ToInt32(result.Split("-")[0]);
                            int awaygoals = Convert.ToInt32(result.Split("-")[1]);

                            if (homegoals > awaygoals)
                            {
                                h.addPoints(s);
                            }
                            else if (awaygoals > homegoals)
                            {
                                a.addPoints(s);
                            }
                            else
                            {
                                h.addPoints(1);
                                a.addPoints(1);
                            }
                        }

                        string[,] matches = new string[lines.Length - consideredGames - 1 - (numberGamedays - i),2];

                        foreach (Team t in teams.Values)
                        {
                            t.setMaxPoints(t.getPoints());
                        }

                        int xCounter = 0;

                        // left games
                        for (int m = consideredGames + 1; m < lines.Length; m++)
                        {
                            string line = lines[m];

                            string[] entries = line.Split(",");

                            string homeTeam = entries[2];
                            string awayTeam = entries[4];

                            Team h = teams[homeTeam];
                            Team a = teams[awayTeam];

                            if (h.getName().Equals(k))
                            {
                                h.addPoints(s);
                                h.addMaxPoints(s);
                                xCounter++;
                            }
                            else if (a.getName().Equals(k))
                            {
                                a.addPoints(s);
                                a.addMaxPoints(s);
                                xCounter++;
                            }
                            else
                            {
                                matches[m - consideredGames - 1 - xCounter, 0] = h.getName();
                                matches[m - consideredGames - 1 - xCounter, 1] = a.getName();

                                h.addMaxPoints(s);
                                a.addMaxPoints(s);
                            }
                        }

                        Team x = teams[k];
                        teams.Remove(k);

                        List<KeyValuePair<string, Team>> sortedTeams = teams.ToList();

                        sortedTeams.Sort(
                            delegate (KeyValuePair<string, Team> pair1,
                            KeyValuePair<string, Team> pair2)
                            {
                                return (pair2.Value.getMaxPoints() - pair1.Value.getMaxPoints());
                            }
                        );

                        int higherTeams = 0;

                        int nodeNumber = matches.GetLength(0) + teams.Count + 2;

                        Graph graph = null;

                        switch (dataStructure)
                        {
                            case 0:

                                List<List<float>> matrix = new();

                                for (int n = 0; n < nodeNumber; n++)
                                {
                                    matrix.Add(new());
                                    matrix[n].Add(0);

                                    for (int j = 1; j < nodeNumber; j++)
                                    {
                                        if (n == 0)
                                        {
                                            if (j <= matches.GetLength(0))
                                            {
                                                matrix[n].Add(s);
                                            }
                                            else
                                            {
                                                matrix[n].Add(0);
                                            }
                                        }
                                        else if (n <= matches.GetLength(0))
                                        {
                                            string t1 = matches[n - 1, 0];
                                            string t2 = matches[n - 1, 1];

                                            if (sortedTeams.IndexOf(new(t1, teams[t1])) + matches.GetLength(0) + 1 == j || sortedTeams.IndexOf(new(t2, teams[t2])) + matches.GetLength(0) + 1 == j)
                                            {
                                                matrix[n].Add(s);
                                            }
                                            else
                                            {
                                                matrix[n].Add(0);
                                            }
                                        }
                                        else if (n == nodeNumber - 1)
                                        {
                                            matrix[n].Add(0);
                                        }
                                        else
                                        {
                                            if (j < nodeNumber - 1)
                                            {
                                                matrix[n].Add(0);
                                            }
                                            else
                                            {
                                                int index = n - (matches.GetLength(0) + 1);
                                                int diff = x.getPoints() - sortedTeams[index].Value.getPoints();

                                                if (diff < 0 || index < position - 1)
                                                {
                                                    matrix[n].Add(float.MaxValue);
                                                    higherTeams++;
                                                }
                                                else
                                                {
                                                    matrix[n].Add(diff);
                                                }
                                            }
                                        }
                                    }
                                }

                                graph = new AdjacencyMatrix(matrix, counts);

                                break;
                            case 1:

                                List<List<Edge>> adjacencyList = new();

                                for (int n = 0; n < nodeNumber; n++)
                                {
                                    adjacencyList.Add(new());
                                }
                                for (int n = 0; n < nodeNumber; n++)
                                {
                                    if (n.Equals(0))
                                    {
                                        for (int j = 1; j <= matches.GetLength(0); j++)
                                        {
                                            adjacencyList[0].Add(new Edge(0,j,s));
                                        }
                                    }
                                    else if (n <= matches.GetLength(0))
                                    {
                                        string t1 = matches[n - 1, 0];
                                        string t2 = matches[n - 1, 1];

                                        adjacencyList[n].Add(new Edge(n, sortedTeams.IndexOf(new(t1, teams[t1])) + matches.GetLength(0) + 1, s));
                                        adjacencyList[n].Add(new Edge(n, sortedTeams.IndexOf(new(t2, teams[t2])) + matches.GetLength(0) + 1, s));
                                    }
                                    else if (n < nodeNumber - 1)
                                    {
                                        int index = n - (matches.GetLength(0) + 1);
                                        int diff = x.getPoints() - sortedTeams[index].Value.getPoints();

                                        if (diff < 0 || index < position - 1)
                                        {
                                            adjacencyList[n].Add(new Edge(n, nodeNumber - 1, float.MaxValue));
                                            higherTeams++;
                                        }
                                        else if (diff > 0)
                                        {
                                            adjacencyList[n].Add(new Edge(n, nodeNumber - 1, diff));
                                        }
                                    }
                                }

                                graph = new AdjacencyList(adjacencyList, counts);

                                break;
                        }

                        if (higherTeams > position - 1)
                        {
                            abortedTeams.Add(x.getName());
                            continue;
                        }

                        float flow;

                        switch (algorithmMode)
                        {
                            case 0:
                                flow = MaxFlow.EdmondsKarp(graph,ref iterations);
                                break;
                            case 1:
                                flow = MaxFlow.Dinic(graph,dataStructure, ref iterations);
                                break;
                            case 2:
                                flow = MaxFlow.PushRelabel(graph, 0, ref iterations);
                                break;
                            case 3:
                                flow = MaxFlow.PushRelabel(graph, 1, ref iterations);
                                break;
                            default:
                                return;
                        }

                        counts = graph.getCounts();

                        if (flow == matches.GetLength(0) * s)
                        {
                            counter++;
                        }
                        else
                        {
                            abortedTeams.Add(x.getName());
                        }
                    }
                }
            }

            timer.Stop();

            csvData.AppendLine(modeName + ";" + dataStructureName + ";" + timer.Elapsed + ";" + iterations + ";" + counts[0] + ";" + counts[1] + ";" + counts[2] + ";" + counts[3] + ";" + counts[4] + ";" + counts[5] + ";" + counts[6] + ";" + counts[7] + ";" + counts[8] + ";" + counts[9] + ";" + counts[10]); ;
        }
    }
}