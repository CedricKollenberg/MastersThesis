using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Championshipproblem_c_Rule
{
    // class to execute the calculation of championship problems
    class ChampionshipProblem
    {
        private Dictionary<string,Team> teams; // save all relevant teams
        private Team x; // team to become champion
        private int maxWins; // number of maximum possible wins
        private int level = 0; // level in the search tree, a level describes a winner and loser
        private int c; // number of point per win
        private List<string> surplusTeams; // list of surplus teams
        private int T1; // depth 1
        private int T2; // depth 2
        private double epsilon; // epsilon factor
        private double kappa; // kappa factor
        private double zeta; // zeta factor
        private bool order; // order of the execution of the surplus teams, true: complete reduction of the set surplus team
        private bool depthLevel; // use of depth level
        private bool abortRule = false; // usage of the abort rule, relevant for heuristic mode 1
        private int maxIterations = int.MaxValue; // number of maximum iterations
        private int[] T1Levels; // array of depths for the levels for T1
        private int[] T2Levels; // array of depths for the levels for T2

        // set depths to 0
        public void removeDepths()
        {
            T1 = 0;
            T2 = 0;
        }

        // set variable depths, depends on the current level
        public void setVariableDepthLevel(int l)
        {
            this.depthLevel = true;

            // initialize array size with the maximum possible number of levels
            T1Levels = new int[maxWins];
            T2Levels = new int[maxWins];

            // decrement starting depth with each level step l
            for (int i = 0; i < maxWins; i++)
            {
                T1Levels[i] = Math.Max(T1 - (i / l),0);
            }
            for (int i = 0; i < maxWins; i++)
            {
                T2Levels[i] = Math.Max(T2 - (i / l), 0);
            }
        }

        // set maximum number of iterations
        public void setMaxIterations(int maxIterations)
        {
            this.maxIterations = maxIterations;
        }

        // get list of teams
        public Dictionary<string, Team> getTeams()
        {
            return this.teams;
        }

        // constructor for creating a championship problem based on the all given parameters
        public ChampionshipProblem(Dictionary<string,Team> teams, Team x, int c, int T1, int T2, double epsilon, double kappa, double zeta, bool order, bool depthLevel)
        {
            this.teams = teams;
            this.x = x;
            this.c = c;
            this.surplusTeams = new ();
            this.T1 = T1;
            this.T2 = T2;
            this.epsilon = epsilon;
            this.kappa = kappa;
            this.zeta = zeta;
            this.order = order;
            
        }

        // constructor for creating a championship problem based on the data file
        public ChampionshipProblem(string csv, int gamesPerGameday, int c, int gameday, string teamX, int T1, int T2, double epsilon, double kappa, double zeta, bool order, bool depthLevel = false)
        {
            string[] lines = File.ReadAllLines(csv); // read all lines

            int consideredGames = gameday * gamesPerGameday; // calculate number of games to be considered for the current table

            teams = new(); // initilize teams list

            // initialize heuristic parameters
            this.T1 = T1;
            this.T2 = T2;
            this.epsilon = epsilon;
            this.kappa = kappa;
            this.zeta = zeta;
            this.order = order;

            // check all games already played and enter results
            for (int i = 1; i <= consideredGames; i++)
            {
                string line = lines[i];

                string[] entries = line.Split(",");

                string homeTeam = entries[2];
                string awayTeam = entries[4];
                string result = entries[3];

                if (i <= gamesPerGameday) // add teams to the list
                {
                    teams.Add(homeTeam, new Team(homeTeam));
                    teams.Add(awayTeam, new Team(awayTeam));
                }

                Team h = teams[homeTeam];
                Team a = teams[awayTeam];

                int homegoals = Convert.ToInt32(result.Split("-")[0]);
                int awaygoals = Convert.ToInt32(result.Split("-")[1]);

                // handle result
                if (homegoals > awaygoals) // home team wins
                {
                    h.addPoints(c);
                }
                else if (awaygoals > homegoals) // away team wins
                {
                    a.addPoints(c);
                }
                else // draw
                {
                    h.addPoints(1);
                    a.addPoints(1);
                }
            }

            // remaining games
            for (int i = consideredGames + 1; i < lines.Length; i++)
            {
                string line = lines[i];

                string[] entries = line.Split(",");

                string homeTeam = entries[2];
                string awayTeam = entries[4];

                Team h = teams[homeTeam];
                Team a = teams[awayTeam];

                if (h.getName().Equals(teamX)) // add win for the team x
                {
                    h.addPoints(c);
                }
                else if (a.getName().Equals(teamX)) // add win for the team x
                {
                    a.addPoints(c);
                }
                else // add opponent to other team's opponent list
                {
                    h.addOpponent(a.getName());
                    a.addOpponent(h.getName());
                }
            }

            x = teams[teamX];
            teams.Remove(x.getName()); // remove team x, since it is the team to become champion and must not be considered
            this.c = c;
            surplusTeams = new(); // initialize surplus team list
        }

        // print results of the games based on the heuristic's calculation
        public void printGameResults()
        {
            foreach (Team k in teams.Values)
            {
                Console.WriteLine("\nWins of Team " + k.getName());

                foreach (string s in k.getWins()) // print all wins of team k
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\nDraws of Team " + k.getName());

                foreach (string s in k.getDraws()) // print all draws of team k
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\nLosses of Team " + k.getName());

                foreach (string s in k.getLosses()) // print all losses of team k
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\n");
            }
        }

        // print resulting table
        public void printTable()
        {
            List<KeyValuePair<string, Team>> sortedTeams = teams.ToList(); // create list of key value pairs for soritng

            // sort list based on their points
            sortedTeams.Sort(
                delegate (KeyValuePair<string, Team> pair1,
                KeyValuePair<string, Team> pair2)
                {
                    return (pair2.Value.getPoints() - pair1.Value.getPoints());
                }
            );
            
            Console.WriteLine("Team " + x.getName() + "\t" + x.getPoints()); // print champion

            // print all other teams and their points
            foreach ((string s, Team k) in sortedTeams)
            {
                Console.WriteLine("Team " + k.getName() + "\t" + k.getPoints());
            }
        }

        // print whole result
        public void printResult()
        {
            printGameResults();

            printTable();
        }

        // internal class for storing games, needed for brute force method
        public class Game
        {
            string TeamA;
            string TeamB;
            int mode; // result of the game, 1 = win for home team, 0 = draw, -1 = win for away team

            public Game(string teamA, string teamB)
            {
                TeamA = teamA;
                TeamB = teamB;
                mode = 1;
            }

            public string getTeamA()
            {
                return TeamA;
            }

            public string getTeamB()
            {
                return TeamB;
            }

            public int getMode()
            {
                return mode;
            }

            public void setMode(int m)
            {
                mode = m;
            }

            public void decrementMode()
            {
                mode--;
            }
        }

        // brute force method
        public int bruteForce()
        {
            int counter = 0; // counter for number of iterations
            List<Game> games = new(); // list of games to be considered

            // set oppenets for each team
            foreach (Team t in teams.Values)
            {
                foreach (string s in t.getOpponents())
                {
                    games.Add(new Game(t.getName(), s));
                    t.removeOpponent(t.getName());
                }
            }

            // execute calculation
            for (int i = 0; i < games.Count; i++)
            {
                // abort when reaching maximum number of iterations
                if (counter.Equals(maxIterations))
                {
                    Console.WriteLine("max Iterations");
                    return -maxIterations;
                }

                counter++;

                Game g = games[i]; // get game

                switch (g.getMode()) // check mode of the game
                {
                    case 1: // home team wins
                        teams[g.getTeamA()].addPoints(c);
                        break;
                    case 0: // draw
                        teams[g.getTeamA()].addPoints(-(c-1));
                        teams[g.getTeamB()].addPoints(1);
                        break;
                    case -1: // away team wins
                        teams[g.getTeamA()].addPoints(-1);
                        teams[g.getTeamB()].addPoints(c-1);
                        break;
                    case -2: // reset current game and consider the previous game
                        if (i.Equals(0))
                        {
                            return -counter;
                        }
                        g.setMode(1);
                        i -= 2;
                        teams[g.getTeamB()].addPoints(-c);
                        continue;
                }

                g.decrementMode(); // set next game result

                if (i.Equals(games.Count - 1)) // if all games are set
                {
                    if (getDistance() > 0) // if it is no solution, continue
                    {
                        i--;
                    }
                    else // solution found
                    {
                        break;
                    }
                }
            }

            return counter;
        }

        // apply reduction rules
        public bool applyRules(int rule = 0, Dictionary<int,int> aborts = null)
        {
            // initiliaze abort list in case it is null
            if (aborts is null)
            {
                aborts = new();
                aborts.Add(1, 0);
                aborts.Add(5, 0);
                aborts.Add(6, 0);
            }

            if (rule != 1)
            {
                // apply rule 1
                foreach (Team y in teams.Values)
                {
                    if (y.getPoints() > x.getPoints()) // abort if a team has already more points than x
                    {
                        aborts[1]++;
                        //Console.WriteLine("Championship problem is not possible.\nReason: Rule 1\nDetails: Team " + y.getName() + " has already " + y.getPoints() + " points.\nTeam " + x.getName() + " has just " + x.getPoints() + " points.");
                        return false;
                    }
                }
            }

            if (rule != 2)
            {
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

                    int possiblePoints = y.getPoints() + y.getOpponents().Count * c; // calculate maximum possible number of points

                    if (possiblePoints <= x.getPoints()) // if it is smaller than x's points, team should win all games
                    {
                        // set win against each opponent
                        for (int i = 0; i < y.getOpponents().Count;)
                        {
                            string t = y.getOpponents()[i];
                            y.addWin(t, c + 1);
                            Team k = teams[t];
                            k.addLoss(y.getName());
                            k.addPoints(1);
                            consideredTeams.Enqueue(k); // add opponent to the queue
                        }
                    }
                }
            }

            if (rule != 3)
            {
                // apply rule 3
                foreach (Team y in teams.Values)
                {
                    if (y.getPoints().Equals(x.getPoints())) // if a team has exactly the same number of points as x, this team should lose all of their games
                    {
                        // set loss against each opponent
                        for (int i = 0; i < y.getOpponents().Count;)
                        {
                            string t = y.getOpponents()[i];
                            y.addLoss(t);
                            y.addPoints(1);
                            Team k = teams[t];
                            k.addWin(y.getName(), c + 1);
                        }
                    }
                }
            }

            if (rule != 4)
            {
                // apply rule 4
                foreach (Team y in teams.Values)
                {
                    int possiblePoints = y.getPoints() + y.getOpponents().Count; // calculate number of points if team draws all games

                    if (y.getPoints() + c > x.getPoints() && possiblePoints <= x.getPoints()) // if this is smaller than x's points and the team cannot win another game
                    {
                        // set all games as draw
                        for (int i = 0; i < y.getOpponents().Count;)
                        {
                            string t = y.getOpponents()[i];
                            y.addDraw(t);
                            Team k = teams[t];
                            k.addDraw(y.getName());
                        }
                    }
                }
            }

            if (rule != 5)
            {
                // apply rule 5 -> same as rule 1
                foreach (Team y in teams.Values)
                {
                    if (y.getPoints() > x.getPoints())
                    {
                        aborts[5]++;
                        //Console.WriteLine("Championship problem is not possible.\nReason: Rule 5\nDetails: Team " + y.getName() + " has already " + y.getPoints() + " points.\nTeam " + x.getName() + " has just " + x.getPoints() + " points.");
                        return false;
                    }
                }
            }

            // set overall surplus and receptiveness
            int overallSurplus = 0;
            int overallReceptiveness = 0;

            if (rule != 6)
            {
                // apply rule 6
                int overallCompleteReceptiveness = 0; // set overall complete receptiveness

                foreach (Team y in teams.Values) // check each team
                {
                    y.addPoints(y.getOpponents().Count); // set each game provisional as draw
                    y.setDiffPoints(y.getPoints() - x.getPoints()); // set differential points

                    if (y.getDiffPoints() > 0) // surplus team
                    {
                        surplusTeams.Add(y.getName());
                        overallSurplus += y.getDiffPoints(); // add surplus
                    }
                    else // deficit team
                    {
                        overallCompleteReceptiveness += (-y.getDiffPoints() / (c - 1)); // add multiples of c-1 to the complete receptiveness
                        overallReceptiveness += -y.getDiffPoints(); // add receptiveness
                    }
                }

                if (overallSurplus > overallCompleteReceptiveness) // abort if surplus is greater than complete receptiveness
                {
                    aborts[6]++;
                    //Console.WriteLine("Championship problem is not possible.\nReason: Rule 6\nDetails: Overall surplus of " + overallSurplus + " is bigger than the overall complete receptiveness " + overallCompleteReceptiveness + ".");
                    return false;
                }
            }
            else // the same behaviour without the complete receptiveness
            {
                maxWins = 0;

                foreach (Team y in teams.Values)
                {
                    y.addPoints(y.getOpponents().Count);
                    y.setDiffPoints(y.getPoints() - x.getPoints());

                    if (y.getDiffPoints() > 0)
                    {
                        surplusTeams.Add(y.getName());
                        overallSurplus += y.getDiffPoints();
                    }
                    else
                    {
                        overallReceptiveness += -y.getDiffPoints();
                    }
                }
            }

            maxWins = overallReceptiveness - overallSurplus * (c-2); // set maximum number of wins, based on receptivenss and surplus

            return true;
        }

        // apply simple exit rules for checking the status of the heuristic
        public bool applySimpleExitRules(Team a, int rule)
        {
            if (rule != 7)
            {
                // apply rule 7
                foreach (Team y in teams.Values)
                {
                    if (y.getDiffPoints() > y.getOpponents().Count) // abort if suplus is greater than the number of opponents
                    {
                        return false;
                    }
                }
            }

            if (rule != 8)
            {
                // appl rule 8
                if ((maxWins - level) < getOverallSurplus()) // abort if current surplus is greater than the left levels
                {
                    return false;
                }
            }

            if (rule != 9)
            {
                // apply rule 9
                foreach (string opp in a.getOpponents()) // check for each opponent if they can still accept a win
                {
                    Team y = teams[opp];

                    if (y.getPoints() - y.getOpponents().Count + c <= x.getPoints())
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        // calculate overall surplus
        public int getOverallSurplus()
        {
            int overallSurplus = 0;

            foreach (Team k in teams.Values)
            {
                if (k.getDiffPoints() > 0)
                {
                    overallSurplus += k.getDiffPoints(); // add surplus of each surplus team
                }
            }

            return overallSurplus;
        }

        // calculate surplus based on their current points, similar to getOverallSurplus, used for brute force method
        public int getDistance()
        {
            int overallSurplus = 0;

            foreach (Team k in teams.Values)
            {
                int diff = k.getPoints() - x.getPoints();

                if (diff > 0)
                {
                    overallSurplus += diff;
                }
            }

            return overallSurplus;
        }

        // apply heuristics, calculate if there is a solution for the problem
        // mode possibilities: 0 = normal mode, checks every possibility, 1 = exit method 1, aborts based on tree branch, 2 = exit method 2, aborts based on levels
        public int applyHeuristics(int mode, int counter, int rule = 0)
        {
            Stack<Team> sTeams = new(); // stack to save all surplus teams
            Stack<Team> winners = new(); // stack to save all winners
            List<Team> abortTeams = new(); // list of abort teams, used for mode 1
            Dictionary<string, HashSet<Team>[]> examinedTeams = new(); // dictionary for examined teams, used for mode 0
            Dictionary<string, int> levelAbort = new(); // dictionary for level aborts, used for mode 2
            bool lastLevelChecked = false; // states if last level was checked

            if (surplusTeams.Count > 0) // only apply if there is a surplus team
            {
                // initiliaze abort data structures
                switch (mode)
                {
                    case 0:
                        foreach (string s in teams.Keys)
                        {
                            examinedTeams.Add(s, new HashSet<Team>[maxWins]); // since maxWins is the highest possible number of levels, it could be used as an fixed maximum for the array size

                            for (int i = 0; i < maxWins; i++)
                            {
                                examinedTeams[s][i] = new();
                            }
                        }

                        //setMaxIterations(int.MaxValue);
                        break;
                    case 2:
                        foreach (string s in teams.Keys)
                        {
                            levelAbort.Add(s, maxWins); // set abort level to the maximum
                        }
                        break;
                }                

                setNextSurplusTeam(sTeams); // set first surplus team

                while (true) // heuristic loop
                {
                    Team k = sTeams.Peek(); // get first element of the stack

                    if (lastLevelChecked) // reset last game if last level was checked
                    {
                        level--;
                        recover(k);
                    }

                    Team surplusOpponent = getSurplusOpponent(k, mode, abortTeams, examinedTeams, levelAbort); // get next opponent for the surplus team

                    counter++;

                    // abort if maximum number of iterations is reached
                    if (counter.Equals(this.maxIterations))
                    {
                        //Console.WriteLine("Championship problem takes too long.");

                        return -maxIterations;
                    }

                    if (surplusOpponent is null || !applySimpleExitRules(k,rule)) // recover if there is no available surplus team opponent or the simple exit rules have found a abort condition
                    {
                        recover(k);

                        if (sTeams.Count == 0) // end of checking, no solution found
                        {
                            switch (mode)
                            {
                                case 0:
                                    return -counter;
                                case 1:
                                    if (abortRule) // if abort rule of mode 1 applies, which indicates that the solution does not have to be correct, a detailed check with mode 0 will be executed
                                    {
                                        abortRule = false;

                                        //Console.WriteLine("Check in detail");

                                        return applyHeuristics(--mode, counter);
                                    }
                                    else // otherwise there is no solution
                                    {
                                        //Console.WriteLine("Championship problem is not possible.\nReason: Heuristics\nDetails: Every possible game result was checked.");

                                        return -counter;
                                    }
                                case 2:
                                    return applyHeuristics(--mode, counter); // apply heuristics with mode 1
                            }
                        }
                    }
                    else
                    {
                        // set game
                        k.addLoss(surplusOpponent.getName());
                        surplusOpponent.addWin(k.getName(),c);
                        winners.Push(surplusOpponent);

                        level++;

                        lastLevelChecked = (maxWins == level);

                        if (order && k.getDiffPoints() > 0) // if the current surplus team has still a surplus and the order is active
                        {
                            sTeams.Push(k);
                        }
                        else if (!setNextSurplusTeam(sTeams)) // get next surplus team
                        {
                            break; // return found solution if there is no next surplus team
                        }
                    }
                }  
            }

            // set all remaining games to a draw
            foreach (Team k in teams.Values)
            {
                for (int i = 0; i < k.getOpponents().Count;)
                {
                    string t = k.getOpponents()[i];
                    k.addPoints(-1);
                    k.addDraw(t);
                    Team l = teams[t];
                    l.addPoints(-1);
                    l.addDraw(k.getName());
                }
            }

            //printResult();

            return counter;

            // internal method to recover previous situation, reset current game
            void recover(Team k)
            {
                sTeams.Pop(); // remove first surplus team

                lastLevelChecked = false;

                switch (mode)
                {
                    case 0:
                        examinedTeams[k.getName()][level] = new(); // delete examined teams for the current surplus team and level

                        break;
                    case 1:
                        for (int i = 0; i < abortTeams.Count; i++)
                        {
                            Team t = abortTeams[i];

                            if (t.getLevel() >= level) // check if saved level is greater than the current level
                            {
                                abortTeams.Remove(t); // remove saved team
                                i--;
                            }
                        }

                        break;
                    case 2:
                        break;
                    default:
                        break;
                }

                if (winners.Count > 0) // if there are saved winners
                {
                    Team loser = sTeams.Peek();
                    Team winner = winners.Pop(); // remove winner

                    // reset game
                    loser.removeLoss(winner.getName());
                    winner.removeWin(loser.getName(), c);

                    level--;

                    switch (mode)
                    {
                        case 0: // add examined team
                            examinedTeams[loser.getName()][level].Add(winner);

                            break;
                        case 1: // add winner to abort list
                            winner.setLevel(level);
                            winner.setExitOpponent(loser);
                            abortTeams.Add(winner);

                            break;
                        case 2: // add winner with current level to abort level dictionary
                            int l = levelAbort[winner.getName()];

                            if (level < l)
                            {
                                levelAbort[winner.getName()] = level;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // set next surplus team
        private bool setNextSurplusTeam(Stack<Team> sTeams)
        {
            if (depthLevel)
            {
                T1 = T1Levels[level]; // set depth based on level
            }

            Team y = getSurplusTeam(); // get surplus team

            if (y is null)
            {
                return false;
            }

            sTeams.Push(y); // add it to the surplus stack

            return true;
        }

        // get surplus team
        private Team getSurplusTeam()
        {
            double max = double.MinValue; // initialize max value of evaluation function
            Team maxTeam = null;

            // fill surplus team list
            surplusTeams = new();
            foreach (Team k in teams.Values)
            {
                if (k.getDiffPoints() > 0)
                {
                    surplusTeams.Add(k.getName());
                }
            }

            // calculate function value for each team
            foreach (string s in surplusTeams)
            {
                Team a = teams[s];
                double surplus = S(a, a, T1);

                if (surplus > max) // set team with highest value
                {
                    max = surplus;
                    maxTeam = a;
                }
            }

            return maxTeam;
        }

        // function to evaluate surplus teams
        private double S(Team a, Team b, int t)
        {
            double s = (double)a.getDiffPoints() / a.getOpponents().Count; // base value

            if (t > 0) // consider depths
            {
                foreach (string opp in a.getOpponents()) // get value for each opponent
                {
                    Team k = teams[opp];

                    if (k.Equals(b)) // prevent circles
                    {
                        continue;
                    }

                    s += epsilon * S(k, a, t - 1); // add opponents value, recursively
                }
            }

            return s; 
        }

        // get opponent for surplus team
        private Team getSurplusOpponent(Team k, int mode, List<Team> abortTeams, Dictionary<string,HashSet<Team>[]> examinedTeams, Dictionary<string,int> levelAbort)
        {
            if (depthLevel)
            {
                T2 = T2Levels[level]; // set depth based on level
            }

            double max = double.MinValue; // initialize max value of evaluation function
            Team maxTeam = null;

            // consider all opponents of surplus team
            foreach (string s in k.getOpponents())
            {
                Team a = teams[s];

                switch (mode) // abort if mode conditions are fulfilled
                {
                    case 0:
                        if (examinedTeams[k.getName()][level].Contains(a)) // check if team was already examined
                        {
                            continue;
                        }
                        break;
                    case 1:
                        if (abortTeams.Contains(a)) // check if team was already rejected in the tree branch
                        {
                            if (!a.getExitOpponent().Equals(k)) // check exit opponent
                            {
                                abortRule = true; // if it was not the same, assessment might be wrong
                            }

                            continue;
                        }

                        break;
                    case 2:
                        if (levelAbort[k.getName()] < level) // check abort based on current level
                        {
                            continue;
                        }

                        break;
                    default:
                        break;
                }

                double receptiveness = A(a, k, T2, mode, abortTeams); // calculate receptiveness

                if (receptiveness > max) // set team with max value
                {
                    max = receptiveness;
                    maxTeam = a;
                }
            }

            return maxTeam;
        }

        // base value of receptiveness
        private double A0(Team a)
        {
            return Math.Max(-(double)a.getDiffPoints(),0);
        }

        // recursive function for receptiveness 
        private double A(Team a, Team b, int t, int mode, List<Team> abortTeams)
        {
            switch (mode)
            {
                case 0:
                    break;
                case 1:
                    if (abortTeams.Contains(a)) // check if team could be used at all
                    {
                        return 0;
                    }

                    break;
                default:
                    break;
            }

            double s;

            if (t == T2)
            {
                s = -a.getDiffPoints(); // diff points for the first level, punish bad receptiveness
            }
            else
            {
                s = A0(a); // base value
            }

            if (t > 0) // consider depths
            {
                foreach (string opp in a.getOpponents()) // get value for each opponent
                        {
                    Team k = teams[opp];

                    if (k.Equals(b)) // prevent circles
                    {
                        continue;
                    }

                    s += zeta * 1.0/(c-1) * (A(k, a, t - 1, mode, abortTeams)) + Math.Floor(A0(k) / (c - 1)) * kappa;  // add receptiveness, recursively
                }
            }

            return s;
        }
    }
}