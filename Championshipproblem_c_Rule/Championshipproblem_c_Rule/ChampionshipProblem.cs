using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Championshipproblem_c_Rule
{
    class ChampionshipProblem
    {
        private Dictionary<string,Team> teams;
        private Team x;
        private int maxWins;
        private int level = 0;
        private int c;
        private List<string> surplusTeams;
        private int T1;
        private int T2;
        private double epsilon;
        private double kappa;
        private double zeta;
        private bool order;
        private bool depthLevel;
        private bool abortRule = false;
        private int maxIterations = int.MaxValue;
        private int[] T1Levels;
        private int[] T2Levels;

        public void removeDepths()
        {
            T1 = 0;
            T2 = 0;
        }

        public void setVariableDepthLevel(int l)
        {
            this.depthLevel = true;

            T1Levels = new int[maxWins];
            T2Levels = new int[maxWins];

            for (int i = 0; i < maxWins; i++)
            {
                T1Levels[i] = Math.Max(T1 - (i / l),0);
            }

            for (int i = 0; i < maxWins; i++)
            {
                T2Levels[i] = Math.Max(T2 - (i / l), 0);
            }
        }

        public void setMaxIterations(int maxIterations)
        {
            this.maxIterations = maxIterations;
        }

        public Dictionary<string, Team> getTeams()
        {
            return this.teams;
        }

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

        public ChampionshipProblem(string csv, int gamesPerGameday, int c, int gameday, string teamX, int T1, int T2, double epsilon, double kappa, double zeta, bool order, bool depthLevel = false)
        {
            string[] lines = File.ReadAllLines(csv);

            int consideredGames = gameday * gamesPerGameday;

            teams = new();
            this.T1 = T1;
            this.T2 = T2;
            this.epsilon = epsilon;
            this.kappa = kappa;
            this.zeta = zeta;
            this.order = order;

            // results
            for (int i = 1; i <= consideredGames; i++)
            {
                string line = lines[i];

                string[] entries = line.Split(",");

                string homeTeam = entries[2];
                string awayTeam = entries[4];
                string result = entries[3];

                if (i <= gamesPerGameday)
                {
                    teams.Add(homeTeam, new Team(homeTeam));
                    teams.Add(awayTeam, new Team(awayTeam));
                }

                Team h = teams[homeTeam];
                Team a = teams[awayTeam];

                int homegoals = Convert.ToInt32(result.Split("-")[0]);
                int awaygoals = Convert.ToInt32(result.Split("-")[1]);

                if (homegoals > awaygoals)
                {
                    h.addPoints(c);
                }
                else if (awaygoals > homegoals)
                {
                    a.addPoints(c);
                }
                else
                {
                    h.addPoints(1);
                    a.addPoints(1);
                }
            }

            // left games
            for (int i = consideredGames + 1; i < lines.Length; i++)
            {
                string line = lines[i];

                string[] entries = line.Split(",");

                string homeTeam = entries[2];
                string awayTeam = entries[4];

                Team h = teams[homeTeam];
                Team a = teams[awayTeam];

                if (h.getName().Equals(teamX))
                {
                    h.addPoints(c);
                }
                else if (a.getName().Equals(teamX))
                {
                    a.addPoints(c);
                }
                else
                {
                    h.addOpponent(a.getName());
                    a.addOpponent(h.getName());
                }
            }

            x = teams[teamX];
            teams.Remove(x.getName());
            this.c = c;
            surplusTeams = new();
        }

        public void printGameResults()
        {
            foreach (Team k in teams.Values)
            {
                Console.WriteLine("\nWins of Team " + k.getName());

                foreach (string s in k.getWins())
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\nDraws of Team " + k.getName());

                foreach (string s in k.getDraws())
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\nLosses of Team " + k.getName());

                foreach (string s in k.getLosses())
                {
                    Console.WriteLine(s);
                }

                Console.WriteLine("\n");
            }
        }

        public void printTable()
        {
            List<KeyValuePair<string, Team>> sortedTeams = teams.ToList();

            sortedTeams.Sort(
                delegate (KeyValuePair<string, Team> pair1,
                KeyValuePair<string, Team> pair2)
                {
                    return (pair2.Value.getPoints() - pair1.Value.getPoints());
                }
            );

            Console.WriteLine("Team " + x.getName() + "\t" + x.getPoints());

            foreach ((string s, Team k) in sortedTeams)
            {
                Console.WriteLine("Team " + k.getName() + "\t" + k.getPoints());
            }
        }

        public void printResult()
        {
            printGameResults();

            printTable();
        }

        public class Game
        {
            string TeamA;
            string TeamB;
            int mode;

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

        public int bruteForce()
        {
            int counter = 0;
            List<Game> games = new();

            foreach (Team t in teams.Values)
            {
                foreach (string s in t.getOpponents())
                {
                    games.Add(new Game(t.getName(), s));
                    t.removeOpponent(t.getName());
                }
            }

            for (int i = 0; i < games.Count; i++)
            {
                if (counter.Equals(maxIterations))
                {
                    return -maxIterations;
                }

                counter++;

                Game g = games[i];

                switch (g.getMode())
                {
                    case 1:
                        // home team wins
                        teams[g.getTeamA()].addPoints(c);
                        break;
                    case 0:
                        // draw
                        teams[g.getTeamA()].addPoints(-(c-1));
                        teams[g.getTeamB()].addPoints(1);
                        break;
                    case -1:
                        //away team wins
                        teams[g.getTeamA()].addPoints(-1);
                        teams[g.getTeamB()].addPoints(c-1);
                        break;
                    case -2:
                        if (i.Equals(0))
                        {
                            return -counter;
                        }
                        g.setMode(1);
                        i -= 2;
                        teams[g.getTeamB()].addPoints(-c);
                        continue;
                }

                g.decrementMode();

                if (i.Equals(games.Count - 1))
                {
                    if (getDistance() > 0)
                    {
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return counter;
        }

        public bool applyRules(int rule = 0, Dictionary<int,int> aborts = null)
        {
            if (aborts is null)
            {
                aborts = new();
                aborts.Add(1, 0);
                aborts.Add(5, 0);
                aborts.Add(6, 0);
            }

            if (rule != 1)
            {
                // Rule 1
                foreach (Team y in teams.Values)
                {
                    if (y.getPoints() > x.getPoints())
                    {
                        aborts[1]++;
                        //Console.WriteLine("Championship problem is not possible.\nReason: Rule 1\nDetails: Team " + y.getName() + " has already " + y.getPoints() + " points.\nTeam " + x.getName() + " has just " + x.getPoints() + " points.");
                        return false;
                    }
                }
            }

            if (rule != 2)
            {
                // Rule 2
                Queue<Team> consideredTeams = new Queue<Team>();

                foreach (Team t in teams.Values)
                {
                    consideredTeams.Enqueue(t);
                }

                while (consideredTeams.Count > 0)
                {
                    Team y = consideredTeams.Dequeue();

                    int possiblePoints = y.getPoints() + y.getOpponents().Count * c;

                    if (possiblePoints <= x.getPoints())
                    {
                        for (int i = 0; i < y.getOpponents().Count;)
                        {
                            string t = y.getOpponents()[i];
                            y.addWin(t, c + 1);
                            Team k = teams[t];
                            k.addLoss(y.getName());
                            k.addPoints(1);
                            consideredTeams.Enqueue(k);
                        }
                    }
                }
            }

            if (rule != 3)
            {
                // Rule 3
                foreach (Team y in teams.Values)
                {
                    if (y.getPoints().Equals(x.getPoints()))
                    {
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
                // Rule 4
                foreach (Team y in teams.Values)
                {
                    int possiblePoints = y.getPoints() + y.getOpponents().Count;

                    if (y.getPoints() + c > x.getPoints() && possiblePoints <= x.getPoints())
                    {
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
                // Rule 5
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

            int overallSurplus = 0;
            int overallReceptiveness = 0;

            if (rule != 6)
            {
                // Rule 6
                int overallCompleteReceptiveness = 0;

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
                        overallCompleteReceptiveness += (-y.getDiffPoints() / (c - 1));
                        overallReceptiveness += -y.getDiffPoints();
                    }
                }

                if (overallSurplus > overallCompleteReceptiveness)
                {
                    aborts[6]++;
                    //Console.WriteLine("Championship problem is not possible.\nReason: Rule 6\nDetails: Overall surplus of " + overallSurplus + " is bigger than the overall complete receptiveness " + overallCompleteReceptiveness + ".");
                    return false;
                }
            }
            else
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

            maxWins = overallReceptiveness - overallSurplus * (c-2);

            return true;
        }

        public bool applySimpleExitRules(Team a, int rule)
        {
            if (rule != 7)
            {
                // Rule 7
                foreach (Team y in teams.Values)
                {
                    if (y.getDiffPoints() > y.getOpponents().Count)
                    {
                        return false;
                    }
                }
            }

            if (rule != 8)
            {
                // Rule 8
                if ((maxWins - level) < getOverallSurplus())
                {
                    return false;
                }
            }

            if (rule != 9)
            {
                // Rule 9
                foreach (string opp in a.getOpponents())
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

        public int getOverallSurplus()
        {
            int overallSurplus = 0;

            foreach (Team k in teams.Values)
            {
                if (k.getDiffPoints() > 0)
                {
                    overallSurplus += k.getDiffPoints();
                }
            }

            return overallSurplus;
        }

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

        public int applyHeuristics(int mode, int counter, int rule = 0)
        {
            if (surplusTeams.Count > 0)
            {
                Stack<Team> sTeams = new();
                Stack<Team> winners = new();
                List<Team> abortTeams = new();
                Dictionary<string, HashSet<Team>[]> examinedTeams = new();
                Dictionary<string, int> levelAbort = new();

                switch (mode)
                {
                    case 0:
                        foreach (string s in teams.Keys)
                        {
                            examinedTeams.Add(s, new HashSet<Team>[maxWins]);

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
                            levelAbort.Add(s, maxWins);
                        }
                        break;
                }

                bool lastLevelChecked = false;

                setNextSurplusTeam(sTeams);

                while (true)
                {
                    Team k = sTeams.Peek();

                    Team surplusOpponent = getSurplusOpponent(k, mode, abortTeams, examinedTeams, levelAbort);

                    counter++;

                    if (counter.Equals(this.maxIterations))
                    {
                        //Console.WriteLine("Championship problem takes too long.");

                        return -maxIterations;
                    }

                    if (surplusOpponent is null || !applySimpleExitRules(k,rule) || lastLevelChecked)
                    {
                        sTeams.Pop();

                        lastLevelChecked = false;

                        switch (mode)
                        {
                            case 0:
                                examinedTeams[k.getName()][level] = new();

                                break;
                            case 1:
                                for (int i = 0; i < abortTeams.Count; i++)
                                {
                                    Team t = abortTeams[i];

                                    if (t.getLevel() >= level)
                                    {
                                        abortTeams.Remove(t);
                                        i--;
                                    }
                                }

                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }

                        if (winners.Count > 0)
                        {
                            Team loser = sTeams.Peek();
                            Team winner = winners.Pop();

                            loser.removeLoss(winner.getName());
                            winner.removeWin(loser.getName(), c);

                            level--;

                            switch (mode)
                            {
                                case 0:
                                    examinedTeams[loser.getName()][level].Add(winner);

                                    break;
                                case 1:
                                    winner.setLevel(level);
                                    winner.setExitOpponent(loser);
                                    abortTeams.Add(winner);

                                    break;
                                case 2:
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

                        if (sTeams.Count == 0)
                        {
                            switch (mode)
                            {
                                case 0:
                                    return -counter;
                                case 1:
                                    if (abortRule)
                                    {
                                        abortRule = false;

                                        //Console.WriteLine("Check in detail");

                                        return applyHeuristics(--mode, counter);
                                    }
                                    else
                                    {
                                        //Console.WriteLine("Championship problem is not possible.\nReason: Heuristics\nDetails: Every possible game result was checked.");

                                        return -counter;
                                    }
                                case 2:
                                    return applyHeuristics(--mode, counter);
                            }                   
                        }
                    }
                    else
                    {
                        k.addLoss(surplusOpponent.getName());
                        surplusOpponent.addWin(k.getName(),c);
                        winners.Push(surplusOpponent);

                        level++;

                        lastLevelChecked = (maxWins == level);

                        if (order && k.getDiffPoints() > 0)
                        {
                            sTeams.Push(k);
                        }
                        else if (!setNextSurplusTeam(sTeams))
                        {
                            break;
                        }
                    }
                }  
            }

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
        }

        private bool setNextSurplusTeam(Stack<Team> sTeams)
        {
            if (depthLevel)
            {
                T1 = T1Levels[level];
            }

            Team y = getSurplusTeam();

            if (y is null)
            {
                return false;
            }

            sTeams.Push(y);

            return true;
        }

        private Team getSurplusTeam()
        {
            double max = double.MinValue;
            Team maxTeam = null;

            surplusTeams = new();
            foreach (Team k in teams.Values)
            {
                if (k.getDiffPoints() > 0)
                {
                    surplusTeams.Add(k.getName());
                }
            }

            foreach (string s in surplusTeams)
            {
                Team a = teams[s];
                double surplus = S(a, a, T1);

                if (surplus > max)
                {
                    max = surplus;
                    maxTeam = a;
                }
            }

            return maxTeam;
        }

        private double S(Team a, Team b, int t)
        {
            double s = (double)a.getDiffPoints() / a.getOpponents().Count;

            if (t > 0)
            {
                foreach (string opp in a.getOpponents())
                {
                    Team k = teams[opp];

                    if (k.Equals(b))
                    {
                        continue;
                    }

                    s += epsilon * S(k, a, t - 1);
                }
            }

            return s; 
        }

        private Team getSurplusOpponent(Team k, int mode, List<Team> abortTeams, Dictionary<string,HashSet<Team>[]> examinedTeams, Dictionary<string,int> levelAbort)
        {
            if (depthLevel)
            {
                T2 = T2Levels[level];
            }

            double max = double.MinValue;
            Team maxTeam = null;

            foreach (string s in k.getOpponents())
            {
                Team a = teams[s];

                switch (mode)
                {
                    case 0:
                        if (examinedTeams[k.getName()][level].Contains(a))
                        {
                            continue;
                        }
                        break;
                    case 1:
                        if (abortTeams.Contains(a))
                        {
                            if (!a.getExitOpponent().Equals(k))
                            {
                                abortRule = true;
                            }

                            continue;
                        }

                        break;
                    case 2:
                        if (levelAbort[k.getName()] < level)
                        {
                            continue;
                        }

                        break;
                    default:
                        break;
                }

                double receptiveness = A(a, k, T2, mode, abortTeams);

                if (receptiveness > max)
                {
                    max = receptiveness;
                    maxTeam = a;
                }
            }

            return maxTeam;
        }

        private double A0(Team a)
        {
            return Math.Max(-(double)a.getDiffPoints(),0);
        }

        private double A(Team a, Team b, int t, int mode, List<Team> abortTeams)
        {
            switch (mode)
            {
                case 0:
                    break;
                case 1:
                    if (abortTeams.Contains(a))
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
                s = -a.getDiffPoints();
            }
            else
            {
                s = A0(a);
            }

            if (t > 0)
            {
                foreach (string opp in a.getOpponents())
                {
                    Team k = teams[opp];

                    if (k.Equals(b))
                    {
                        continue;
                    }

                    s += zeta * 1.0/(c-1) * (A(k, a, t - 1, mode, abortTeams)) + Math.Floor(A0(k) / (c - 1)) * kappa;
                }
            }

            return s;
        }
    }
}