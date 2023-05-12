using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    class Team
    {
        string name;
        int points;
        int maxPoints;
        private List<string> opponents;

        public Team(string n, int p)
        {
            name = n;
            points = p;
            opponents = new();
        }

        public void addPoints(int p)
        {
            points += p;
        }

        public int getPoints()
        {
            return points;
        }

        public void setMaxPoints(int p)
        {
            maxPoints = p;
        }

        public void addMaxPoints(int p)
        {
            maxPoints += p;
        }

        public int getMaxPoints()
        {
            return maxPoints;
        }

        public string getName()
        {
            return name;
        }

        public List<string> getOpponents()
        {
            return this.opponents;
        }

        public void addOpponent(string opp)
        {
            this.opponents.Add(opp);
        }

        public void removeOpponent(string opp)
        {
            this.opponents.Remove(opp);
        }
    }
}