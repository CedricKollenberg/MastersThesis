// author: Cedric Kollenberg
//         Hochschule Fulda, University of Applied Sciences
//         Department of Applied Computer Science
//
// for detailed explanations, check out my master's thesis

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
        // program variables
        static string data = @"..\..\..\..\..\Data\";
        static int s = 2;

        static void Main(string[] args)
        {
            int position = 1; // position of the championship problem to be checked

            StringBuilder csvData = new();

            csvData.AppendLine("Algorithm;Data Structure;Runtime;Iterations;AddNode;RemoveNode;AddEdge;RemoveEdge;GetEdges;GetNeighbours;GetPredecessors;SetCapacity;AddCapacity;GetCapacity;GetSize");

            for (int k = 0; k < 10; k++) // 10 iterations
            {
                Console.WriteLine("Iteration " + k);

                for (int i = 0; i < 4; i++) // iterate algorithm
                {
                    for (int j = 0; j < 2; j++) // iterate data structure
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

            // set algorithm name
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

            // set data structure name
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

            // initialize array to count graph-specific operations
            int[] counts = new int[11];
            for (int l = 0; l < 11; l++)
            {
                counts[l] = 0;
            }

            int iterations = 0; // number of iterations

            Console.WriteLine(modeName + "_" + dataStructureName);

            Stopwatch timer = new Stopwatch();

            timer.Start(); // start runtime measurement

            foreach (string csv in Directory.GetFiles(data, "*_*.csv")) // get all data files
            {
                string season = Path.GetFileNameWithoutExtension(csv); // get season name

                Console.WriteLine(season);

                string[] lines = File.ReadAllLines(csv); // get all lines

                // initialize list of teams and aborted teams
                List<string> teamNames = new();
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

                    teamNames.Add(entries[2]);
                    teamNames.Add(entries[4]);

                    i++;
                }

                // set number of gamedays and games per gameday
                int gamesPerGameday = i - 1;
                int numberGamedays = gamesPerGameday * 4 - 2;

                for (i = numberGamedays / 2 + 1; i < numberGamedays; i++) // only the second half of the season is relevant
                {
                    int counter = 0;

                    foreach (string k in teamNames) // execute for every team
                    {
                        Dictionary<string, Team> teams = new(); // initialize dictionary to store teams

                        if (abortedTeams.Contains(k)) // abort if team did not lead to a solution on an earlier gameday
                        {
                            continue;
                        }

                        int consideredGames = i * gamesPerGameday; // calculate number of games to be considered for the current table 

                        // check all games already played and enter results
                        for (int j = 1; j <= consideredGames; j++)
                        {
                            string line = lines[j];

                            string[] entries = line.Split(",");

                            string homeTeam = entries[2];
                            string awayTeam = entries[4];
                            string result = entries[3];

                            if (j <= gamesPerGameday) // add teams to the dictionary
                            {
                                teams.Add(homeTeam, new Team(homeTeam, 0));
                                teams.Add(awayTeam, new Team(awayTeam, 0));
                            }

                            Team h = teams[homeTeam];
                            Team a = teams[awayTeam];

                            int homegoals = Convert.ToInt32(result.Split("-")[0]);
                            int awaygoals = Convert.ToInt32(result.Split("-")[1]);

                            // handle result
                            if (homegoals > awaygoals) // home team wins
                            {
                                h.addPoints(s);
                            }
                            else if (awaygoals > homegoals) // away team wins
                            {
                                a.addPoints(s);
                            }
                            else // draw
                            {
                                h.addPoints(1);
                                a.addPoints(1);
                            }
                        }

                        // set max points
                        foreach (Team t in teams.Values)
                        {
                            t.setMaxPoints(t.getPoints());
                        }

                        int xCounter = 0; // counter for team x

                        // remaining games
                        for (int m = consideredGames + 1; m < lines.Length; m++)
                        {
                            string line = lines[m];

                            string[] entries = line.Split(",");

                            string homeTeam = entries[2];
                            string awayTeam = entries[4];

                            Team h = teams[homeTeam];
                            Team a = teams[awayTeam];

                            if (h.getName().Equals(k)) // add win for the team x
                            {
                                h.addPoints(s);
                                h.addMaxPoints(s);
                                xCounter++;
                            }
                            else if (a.getName().Equals(k)) // add win for the team x
                            {
                                a.addPoints(s);
                                a.addMaxPoints(s);
                                xCounter++;
                            }
                            else // add opponent to other team's opponent list
                            {
                                h.addOpponent(a.getName());
                                a.addOpponent(h.getName());

                                h.addMaxPoints(s);
                                a.addMaxPoints(s);
                            }
                        }

                        Team x = teams[k];
                        teams.Remove(k); // remove team x, since it is the team to become champion and must not be considered

                        if (!applyRules(teams,x)) // apply reduction rules
                        {
                            abortedTeams.Add(x.getName()); // add team x to the aborted list
                            continue;
                        }

                        // set number of matches to be considered for the network
                        int matchNumber = 0;

                        foreach (Team t in teams.Values)
                        {
                            matchNumber += t.getOpponents().Count;
                        }

                        if (matchNumber == 0)
                        {
                            counter++;
                            continue;
                        }

                        matchNumber /= 2;

                        string[,] matches = new string[matchNumber, 2]; // aray for matches

                        int matchCounter = 0;

                        // add matches to array
                        foreach (Team t in teams.Values)
                        {
                            for (int p= 0; p < t.getOpponents().Count;)
                            {
                                string opp = t.getOpponents()[p];
                                matches[matchCounter, 0] = t.getName();
                                matches[matchCounter, 1] = opp;
                                t.removeOpponent(opp);
                                teams[opp].removeOpponent(t.getName());
                                matchCounter++;
                            }
                        }

                        // create list of key value pairs
                        List<KeyValuePair<string, Team>> sortedTeams = teams.ToList();

                        // sort list accroding to max points of teams
                        sortedTeams.Sort(
                            delegate (KeyValuePair<string, Team> pair1,
                            KeyValuePair<string, Team> pair2)
                            {
                                return (pair2.Value.getMaxPoints() - pair1.Value.getMaxPoints());
                            }
                        );

                        int higherTeams = 0; // initialize number of teams ranked higher than x

                        int nodeNumber = matches.GetLength(0) + teams.Count + 2; // set number of nodes

                        Graph graph = null; // create graph

                        switch (dataStructure)
                        {
                            case 0: // adjacency matrix

                                List<List<float>> matrix = new(); // initialize matrix

                                for (int n = 0; n < nodeNumber; n++) //iterate through each node
                                {
                                    matrix.Add(new());
                                    matrix[n].Add(0); // 0 for each column, since there is no edge into the source

                                    for (int j = 1; j < nodeNumber; j++)
                                    {
                                        if (n == 0) // source node
                                        {
                                            if (j <= matches.GetLength(0)) // edge to each team node
                                            {
                                                matrix[n].Add(s);
                                            }
                                            else
                                            {
                                                matrix[n].Add(0);
                                            }
                                        }
                                        else if (n <= matches.GetLength(0)) // game nodes
                                        {
                                            // team names
                                            string t1 = matches[n - 1, 0];
                                            string t2 = matches[n - 1, 1];

                                            // set node for corresponding team indices
                                            if (sortedTeams.IndexOf(new(t1, teams[t1])) + matches.GetLength(0) + 1 == j || sortedTeams.IndexOf(new(t2, teams[t2])) + matches.GetLength(0) + 1 == j)
                                            {
                                                matrix[n].Add(s);
                                            }
                                            else
                                            {
                                                matrix[n].Add(0);
                                            }
                                        }
                                        else if (n == nodeNumber - 1) // sink node
                                        {
                                            matrix[n].Add(0); // no outgoing edges
                                        }
                                        else // edges from the teams to the sink
                                        {
                                            if (j < nodeNumber - 1)
                                            {
                                                matrix[n].Add(0);
                                            }
                                            else
                                            {
                                                int index = n - (matches.GetLength(0) + 1); // get index of team
                                                int diff = x.getPoints() - sortedTeams[index].Value.getPoints(); // calculate edge weight

                                                if (diff < 0 || index < position - 1) // if the difference is negative or the team is allowed to be higher ranked than x
                                                {
                                                    matrix[n].Add(float.MaxValue); // set infinity edge
                                                    higherTeams++;
                                                }
                                                else
                                                {
                                                    matrix[n].Add(diff); // set diff edge
                                                }
                                            }
                                        }
                                    }
                                }

                                graph = new AdjacencyMatrix(matrix, counts); // initialize graph

                                break;
                            case 1:

                                List<List<Edge>> adjacencyList = new(); // initialize list

                                for (int n = 0; n < nodeNumber; n++) //iterate through each node
                                {
                                    adjacencyList.Add(new()); // add neighbour list
                                }
                                for (int n = 0; n < nodeNumber; n++) // iterate through each node
                                {
                                    if (n.Equals(0)) // source node
                                    {
                                        for (int j = 1; j <= matches.GetLength(0); j++) // add edges to all games
                                        {
                                            adjacencyList[0].Add(new Edge(0, j, s));
                                        }
                                    }
                                    else if (n <= matches.GetLength(0)) // game nodes
                                    {
                                        // get team names
                                        string t1 = matches[n - 1, 0];
                                        string t2 = matches[n - 1, 1];

                                        // add edges to teams
                                        adjacencyList[n].Add(new Edge(n, sortedTeams.IndexOf(new(t1, teams[t1])) + matches.GetLength(0) + 1, s));
                                        adjacencyList[n].Add(new Edge(n, sortedTeams.IndexOf(new(t2, teams[t2])) + matches.GetLength(0) + 1, s));
                                    }
                                    else if (n < nodeNumber - 1) // team nodes
                                    {
                                        int index = n - (matches.GetLength(0) + 1); // get team index
                                        int diff = x.getPoints() - sortedTeams[index].Value.getPoints(); // // calculate edge weight

                                        if (diff < 0 || index < position - 1) // if the difference is negative or the team is allowed to be higher ranked than x
                                        {
                                            adjacencyList[n].Add(new Edge(n, nodeNumber - 1, float.MaxValue)); // set infinity edge
                                            higherTeams++;
                                        }
                                        else if (diff > 0)
                                        {
                                            adjacencyList[n].Add(new Edge(n, nodeNumber - 1, diff)); // set diff edge
                                        }
                                    }
                                }

                                graph = new AdjacencyList(adjacencyList, counts); // initialize graph

                                break;
                        }

                        if (higherTeams > position - 1) // check if there are more teams positioned higher than allowed
                        {
                            abortedTeams.Add(x.getName()); // abort
                            continue;
                        }

                        float flow;

                        // execute max flow network algorithm
                        switch (algorithmMode)
                        {
                            case 0:
                                flow = MaxFlow.EdmondsKarp(graph, ref iterations);
                                break;
                            case 1:
                                flow = MaxFlow.Dinic(graph, dataStructure, ref iterations);
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

                        counts = graph.getCounts(); // get number of graph-specific operations

                        if (flow == matches.GetLength(0) * s) // check if there is a solution
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

            timer.Stop(); // stop runtime measrement

            csvData.AppendLine(modeName + ";" + dataStructureName + ";" + timer.Elapsed + ";" + iterations + ";" + counts[0] + ";" + counts[1] + ";" + counts[2] + ";" + counts[3] + ";" + counts[4] + ";" + counts[5] + ";" + counts[6] + ";" + counts[7] + ";" + counts[8] + ";" + counts[9] + ";" + counts[10]); ;
        }

        // apply reduction rules
        public static bool applyRules(Dictionary<string,Team> teams, Team x)
        {
            // apply rule 1
            foreach (Team y in teams.Values)
            {
                if (y.getPoints() > x.getPoints())
                {
                    //Console.WriteLine("Championship problem is not possible.\nReason: Rule 1\nDetails: Team " + y.getName() + " has already " + y.getPoints() + " points.\nTeam " + x.getName() + " has just " + x.getPoints() + " points.");
                    return false;
                }
            }

            // apply rule 2
            Queue<Team> consideredTeams = new Queue<Team>(); // initiliaze queue for all teams to be considered

            // fill queue with teams from team list
            foreach (Team t in teams.Values)
            {
                consideredTeams.Enqueue(t);
            }

            // as long as there are teams in the queue
            while (consideredTeams.Count > 0)
            {
                Team y = consideredTeams.Dequeue();

                int possiblePoints = y.getPoints() + y.getOpponents().Count * s; // calculate maximum possible number of points

                if (possiblePoints <= x.getPoints()) // if it is smaller than x's points, team should win all games
                {
                    for (int i = 0; i < y.getOpponents().Count;)
                    {
                        string t = y.getOpponents()[i];
                        y.addPoints(s);
                        Team k = teams[t];
                        consideredTeams.Enqueue(k); // add opponent to the queue
                        y.removeOpponent(t);
                        k.removeOpponent(y.getName());
                    }
                }
            }

            // apply rule 3
                foreach (Team y in teams.Values)
            {
                if (y.getPoints().Equals(x.getPoints())) // if a team has exactly the same number of points as x, this team should lose all of their games
                {
                    // set loss against each opponent
                    for (int i = 0; i < y.getOpponents().Count;)
                    {
                        string t = y.getOpponents()[i];
                        Team k = teams[t];
                        k.addPoints(s);
                        y.removeOpponent(t);
                        k.removeOpponent(y.getName());
                    }
                }
            }

            // apply rule 4
            foreach (Team y in teams.Values)
            {
                int possiblePoints = y.getPoints() + y.getOpponents().Count; // calculate number of points if team draws all games

                if (y.getPoints() + s > x.getPoints() && possiblePoints <= x.getPoints()) // if this is smaller than x's points and the team cannot win another game
                {
                    // set all games as draw
                    for (int i = 0; i < y.getOpponents().Count;)
                    {
                        string t = y.getOpponents()[i];
                        y.addPoints(1);
                        Team k = teams[t];
                        k.addPoints(1);
                        y.removeOpponent(t);
                        k.removeOpponent(y.getName());
                    }
                }
            }

            // apply rule 5 -> same as rule 1
            foreach (Team y in teams.Values)
            {
                if (y.getPoints() > x.getPoints())
                {
                    //Console.WriteLine("Championship problem is not possible.\nReason: Rule 5\nDetails: Team " + y.getName() + " has already " + y.getPoints() + " points.\nTeam " + x.getName() + " has just " + x.getPoints() + " points.");
                    return false;
                }
            }

            // set overall surplus and complete receptiveness
            int overallCompleteReceptiveness = 0;
            int overallSurplus = 0;

            // apply rule 6
            foreach (Team y in teams.Values) // check each team
            {
                int diff;

                if ((diff = y.getPoints() + y.getOpponents().Count - x.getPoints()) > 0) // calculate the surplus
                {
                    overallSurplus += diff; // surplus team
                }
                else
                {
                    overallCompleteReceptiveness -= diff; // deficit team
                }
            }

            if (overallSurplus > overallCompleteReceptiveness) // abort if surplus is greater than complete receptiveness
            {
                //Console.WriteLine("Championship problem is not possible.\nReason: Rule 6\nDetails: Overall surplus of " + overallSurplus + " is bigger than the overall complete receptiveness " + overallCompleteReceptiveness + ".");
                return false;
            }

            return true;
        }
    }
}