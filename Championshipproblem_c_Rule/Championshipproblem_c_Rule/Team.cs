using System.Collections.Generic;

namespace Championshipproblem_c_Rule
{
    // class to store information abput the teams
    class Team
    {
        private string name;
        private int points;
        private List<string> opponents;
        private int diffPoints;
        private List<string> wins;
        private List<string> draws;
        private List<string> losses;
        private int level;
        private Team exitOpponent;

        public Team (string name)
        {
            this.name = name;
            this.points = 0;
            this.opponents = new List<string>();
            this.level = int.MaxValue;
            initResults();
        }

        private void initResults()
        {
            this.wins = new List<string>();
            this.draws = new List<string>();
            this.losses = new List<string>();
        }

        public string getName()
        {
            return this.name;
        }

        public int getPoints()
        {
            return this.points;
        }

        public List<string> getOpponents()
        {
            return this.opponents;
        }

        public List<string> getWins()
        {
            return this.wins;
        }

        public List<string> getDraws()
        {
            return this.draws;
        }

        public List<string> getLosses()
        {
            return this.losses;
        }

        public void setPoints(int p)
        {
            this.points = p;
        }

        public void addPoints(int p)
        {
            this.points += p;
        }

        public int getDiffPoints()
        {
            return this.diffPoints;
        }

        public void setDiffPoints(int p)
        {
            this.diffPoints = p;
        }

        public void addDiffPoints(int p)
        {
            this.diffPoints += p;
        }

        public void setOppenents(List<string> opp)
        {
            this.opponents = opp;
        }

        public void addOpponent(string opp)
        {
            this.opponents.Add(opp);
        }

        public void removeOpponent(string opp)
        {
            this.opponents.Remove(opp);
        }

        public void addWin(string opp, int c)
        {
            this.wins.Add(opp);
            this.addPoints(c-1);
            this.addDiffPoints(c-1);
            this.removeOpponent(opp);
        }

        public void addDraw(string opp)
        {
            this.draws.Add(opp);
            this.addPoints(1);
            this.removeOpponent(opp);
        }

        public void addLoss(string opp)
        {
            this.losses.Add(opp);
            this.addDiffPoints(-1);
            this.addPoints(-1);
            this.removeOpponent(opp);
        }

        public void removeWin(string opp, int c)
        {
            this.wins.Remove(opp);
            this.addPoints(-c + 1);
            this.addDiffPoints(-c + 1);
            this.addOpponent(opp);
        }

        public void removeLoss(string opp)
        {
            this.losses.Remove(opp);
            this.addDiffPoints(1);
            this.addPoints(1);
            this.addOpponent(opp);
        }

        public int getLevel()
        {
            return this.level;
        }

        public void setLevel(int p)
        {
            this.level = p;
        }

        public Team getExitOpponent()
        {
            return this.exitOpponent;
        }

        public void setExitOpponent(Team k)
        {
            this.exitOpponent = k;
        }
    }
}