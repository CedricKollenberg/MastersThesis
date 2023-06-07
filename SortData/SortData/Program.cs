// author: Cedric Kollenberg
//         Hochschule Fulda, University of Applied Sciences
//         Department of Applied Computer Science
//
// for detailed explanations, check out my master's thesis

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SortData
{
    // create comparer
    // first condition: "Matchday" line
    // second condition: concrete matchday
    public class Comparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.StartsWith("Matchday"))
            {
                return -1;
            }
            if (y.StartsWith("Matchday"))
            {
                return 1;
            }

            return Convert.ToInt32(x.Split(",")[0]) - Convert.ToInt32(y.Split(",")[0]);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            string data = @"..\..\..\..\..\Data\";

            foreach (string csv in Directory.GetFiles(data, "*_*.csv")) // get all data files
            {
                string season = Path.GetFileNameWithoutExtension(csv);

                string[] lines = File.ReadAllLines(csv);

                List<string> games = new();

                foreach (string line in lines) // change separator
                {
                    string l = line.Replace(",", ";");
                    games.Add(l);
                }

                IEnumerable<string> query = lines.OrderBy(a => a, new Comparer()); // order lines by comparer

                string newFile = null;

                foreach (string line in query) // write line
                {
                    newFile += line + "\n";
                }

                File.WriteAllText(data + season + "_new.csv", newFile); // create new file
            }
        }
    }
}