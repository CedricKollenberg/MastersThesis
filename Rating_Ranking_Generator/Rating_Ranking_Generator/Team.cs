using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rating_Ranking_Generator
{
    class Team
    {
        public string name;
        public int goals;
        public int counterGoals;
        public int points;
        public int diffGoals;
        public int index;
        public double rating;
        public int wins;
        public int losses;

        public Team(string name, int index)
        {
            this.name = name;
            this.index = index;
            points = 0;
            goals = 0;
            counterGoals = 0;
            diffGoals = 0;
            rating = 0.0;
            wins = 0;
            losses = 0;
        }
    }
}
