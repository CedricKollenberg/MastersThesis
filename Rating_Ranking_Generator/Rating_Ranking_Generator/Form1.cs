﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rating_Ranking_Generator
{
    public partial class Form1 : Form
    {
        string data = @"D:\Documents\Studium\Hochschule Fulda\Master\Masterarbeit\Implementierungen\Data";
        Dictionary<string, int> inputData;

        public Form1()
        {
            InitializeComponent();
            inputData = new();
            tableLayoutPanel7.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string csv in Directory.GetFiles(data, "*_*.csv"))
            {
                string season = Path.GetFileNameWithoutExtension(csv);
                season = season.Replace("_", "-");

                if (season.Equals("1963-1964") || season.Equals("1964-1965"))
                {
                    inputData.Add(season, 30);
                }
                else if (season.Equals("1991-1992"))
                {
                    inputData.Add(season, 38);
                }
                else
                {
                    inputData.Add(season, 34);
                }

                comboBox1.Items.Add(season);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int numberGamedays = inputData[comboBox1.SelectedItem.ToString()];

            int selVal = Convert.ToInt32(comboBox2.SelectedItem);

            comboBox2.Items.Clear();

            for (int i = 1; i <= numberGamedays; i++)
            {
                comboBox2.Items.Add(i);
            }

            if (selVal <= numberGamedays)
            {
                comboBox2.SelectedItem = selVal;
            }
            else
            {
                comboBox2.SelectedItem = numberGamedays;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = "You did not enter a ";
            string caption = "Input incomplete";

            if (comboBox1.SelectedItem is null)
            {
                message += "season.";

                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (comboBox2.SelectedItem is null)
            {
                message += "gameday.";

                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (comboBox3.SelectedItem is null)
            {
                message += "method.";

                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            string season = comboBox1.SelectedItem.ToString().Replace("-", "_");
            int gameday = Convert.ToInt32(comboBox2.SelectedItem);
            string method = comboBox3.SelectedItem.ToString();

            int numberTeams = 0;
            List<Team> sortedRatings = new();
            List<Team> sortedReal = new();

            ApplyRatingMethods(season, gameday, method, numericUpDown1.Value, numericUpDown2.Value, ref numberTeams, ref sortedRatings, ref sortedReal);

            DataTable ratingsTable = new DataTable();
            ratingsTable.Columns.Add("Rank", typeof(int));
            ratingsTable.Columns.Add("Name", typeof(string));
            ratingsTable.Columns.Add("Rating", typeof(double));

            for (int i = 0; i < numberTeams; i++)
            {
                ratingsTable.Rows.Add(i + 1, sortedRatings[i].name, sortedRatings[i].rating);
            }

            dataGridView1.DataSource = ratingsTable;
            dataGridView1.Columns[0].Width = Convert.ToInt32(dataGridView1.Width * 0.15);
            dataGridView1.Columns[1].Width = Convert.ToInt32(dataGridView1.Width * 0.35);
            dataGridView1.Columns[2].Width = Convert.ToInt32(dataGridView1.Width * 0.5);

            DataTable realTable = new DataTable();
            realTable.Columns.Add("Rank", typeof(int));
            realTable.Columns.Add("Name", typeof(string));
            realTable.Columns.Add("Goals", typeof(string));
            realTable.Columns.Add("Points", typeof(int));

            for (int i = 0; i < numberTeams; i++)
            {
                string g = sortedReal[i].goals + ":" + sortedReal[i].counterGoals;
                realTable.Rows.Add(i + 1, sortedReal[i].name, g, sortedReal[i].points);
            }

            dataGridView2.DataSource = realTable;
            dataGridView2.Columns[0].Width = Convert.ToInt32(dataGridView2.Width * 0.15);
            dataGridView2.Columns[1].Width = Convert.ToInt32(dataGridView2.Width * 0.4);
            dataGridView2.Columns[2].Width = Convert.ToInt32(dataGridView2.Width * 0.2);
            dataGridView2.Columns[2].Width = Convert.ToInt32(dataGridView2.Width * 0.25);
        }

        private void ApplyRatingMethods(string season, int gameday, string method, decimal K, decimal xi, ref int numberTeams, ref List<Team> sortedRatings, ref List<Team> sortedReal)
        {
            if (season.Equals("1963_1964") || season.Equals("1964_1965"))
            {
                numberTeams = 16;
            }
            else if (season.Equals("1991_1992"))
            {
                numberTeams = 20;
            }
            else
            {
                numberTeams = 18;
            }

            int numberGames = numberTeams / 2 * gameday;

            List<Team> ratings = new();
            Dictionary<string, Team> teams = new();

            double[] r = new double[numberTeams];

            double[,] M = new double[numberTeams, numberTeams];
            double[] p = new double[numberTeams];
            double[,] S = new double[numberTeams, numberTeams];

            for (int j = 0; j < numberTeams; j++)
            {
                for (int k = 0; k < numberTeams; k++)
                {
                    if (j == k)
                    {
                        if (method.Equals("Colley"))
                        {
                            M[j, k] = gameday + 2;
                        }
                        else
                        {
                            M[j, k] = gameday;
                        }
                    }
                    else
                    {
                        M[j, k] = 0;
                    }
                }

                p[j] = 0;

                if (method.Equals("Elo"))
                {
                    r[j] = 5000;
                }
            }

            for (int j = 0; j < numberTeams; j++)
            {
                for (int k = 0; k < numberTeams; k++)
                {
                    S[j, k] = 0;
                }
            }

            string[] lines = File.ReadAllLines(data + @"\" + season + ".csv");

            int counter = 0;

            for (int i = 1; i <= numberGames; i++)
            {
                string[] entries = lines[i].Split(",");

                string homeTeam = entries[2];
                string awayTeam = entries[4];
                string result = entries[3];

                int homegoals = Convert.ToInt32(result.Split("-")[0]);
                int awaygoals = Convert.ToInt32(result.Split("-")[1]);

                if (i <= numberTeams / 2)
                {
                    teams.Add(homeTeam, new Team(homeTeam, counter++));
                    teams.Add(awayTeam, new Team(awayTeam, counter++));
                }

                teams[homeTeam].goals += homegoals;
                teams[homeTeam].counterGoals += awaygoals;
                teams[awayTeam].goals += awaygoals;
                teams[awayTeam].counterGoals += homegoals;

                double Sij, Sji;

                if (homegoals > awaygoals)
                {
                    teams[homeTeam].points += 3;
                    teams[homeTeam].wins += 1;
                    teams[awayTeam].losses += 1;
                    S[teams[homeTeam].index, teams[awayTeam].index] += 1;
                    Sij = 1;
                    Sji = 0;
                }
                else if (awaygoals > homegoals)
                {
                    teams[awayTeam].points += 3;
                    teams[awayTeam].wins += 1;
                    teams[homeTeam].losses += 1;
                    S[teams[awayTeam].index, teams[homeTeam].index] += 1;
                    Sij = 0;
                    Sji = 1;
                }
                else
                {
                    teams[homeTeam].points += 1;
                    teams[awayTeam].points += 1;
                    S[teams[homeTeam].index, teams[awayTeam].index] += 0.5;
                    S[teams[awayTeam].index, teams[homeTeam].index] += 0.5;
                    Sij = 0.5;
                    Sji = 0.5;
                }

                if (method.Equals("Elo"))
                {
                    double ratingX = r[teams[homeTeam].index];
                    double ratingY = r[teams[awayTeam].index];
                    double Eij = 1.0 / (1.0 + Math.Pow(10, -(ratingX - ratingY) / (double)xi));
                    double Eji = 1.0 / (1.0 + Math.Pow(10, -(ratingY - ratingX) / (double)xi));
                    r[teams[homeTeam].index] += (double)K * (Sij - Eij);
                    r[teams[awayTeam].index] += (double)K * (Sji - Eji);
                }

                M[teams[homeTeam].index, teams[awayTeam].index] -= 1;
                M[teams[awayTeam].index, teams[homeTeam].index] -= 1;
            }

            foreach (Team t in teams.Values)
            {
                t.diffGoals = t.goals - t.counterGoals;
            }

            switch (method)
            {
                case "Massey":

                    for (int j = 0; j < numberTeams; j++)
                    {
                        M[numberTeams - 1, j] = 1;
                    }

                    foreach (Team t in teams.Values)
                    {
                        p[t.index] = t.diffGoals;
                    }

                    p[numberTeams - 1] = 0;

                    var matrix = Matrix<double>.Build.DenseOfArray(M);
                    var vector = Vector<double>.Build.Dense(p);

                    var x = matrix.Solve(vector);

                    r = x.ToArray();

                    break;
                case "Colley":

                    foreach (Team t in teams.Values)
                    {
                        t.diffGoals = t.goals - t.counterGoals;
                        p[t.index] = 1.0 + 1.0 / 2.0 * (double)(t.wins - t.losses);
                    }

                    matrix = Matrix<double>.Build.DenseOfArray(M);
                    vector = Vector<double>.Build.Dense(p);

                    x = matrix.Solve(vector);

                    r = x.ToArray();

                    break;
                case "Keener":

                    double[,] A = new double[numberTeams, numberTeams];

                    double epsilon = 0.01;
                    int limit = 1000;

                    r = new double[numberTeams];

                    for (int i = 0; i < numberTeams; i++)
                    {
                        for (int j = 0; j < numberTeams; j++)
                        {
                            A[i, j] = (S[i, j] + 1) / (S[i, j] + S[j, i] + 2) + epsilon;
                        }

                        r[i] = 1.0 / numberTeams;
                    }

                    matrix = Matrix<double>.Build.DenseOfArray(A);
                    vector = Vector<double>.Build.Dense(r);

                    var h = Matrix<double>.Build.DenseIdentity(numberTeams);

                    for (int i = 0; i < limit; i++)
                    {
                        vector = matrix.Add(epsilon).Multiply(vector).Normalize(1);
                    }

                    r = vector.ToArray();

                    break;
                case "Elo":
                    break;
                default:
                    return;
            }

            ratings = new();

            foreach (Team t in teams.Values)
            {
                t.rating = r[t.index];
                ratings.Add(t);
            }

            sortedRatings = ratings.OrderBy(s => s.rating).Reverse().ToList();

            List<Team> real = new();

            foreach (Team t in teams.Values)
            {
                real.Add(t);
            }

            sortedReal = real.OrderBy(s => s.points).ThenBy(s => s.diffGoals).ThenBy(s => s.goals).Reverse().ToList();
        }

        private void CalculateDifferences()
        {
            string[] methods = new string[] { "Massey", "Colley", "Keener", "Elo" };

            decimal xi = 100;
            decimal K = 50;

            var csvData = new StringBuilder();
            csvData.AppendLine("Method;Season;Gameday;K-Factor;Xi-Factor;Difference");

            foreach (string method in methods)
            {
                foreach (string csv in Directory.GetFiles(data, "*_*.csv"))
                {
                    string season = Path.GetFileNameWithoutExtension(csv);

                    int numberTeams;

                    if (season.Equals("1963_1964") || season.Equals("1964_1965"))
                    {
                        numberTeams = 16;
                    }
                    else if (season.Equals("1991_1992"))
                    {
                        numberTeams = 20;
                    }
                    else
                    {
                        numberTeams = 18;
                    }

                    int numberGamedays = (numberTeams - 1) * 2;

                    for (int i = 1; i <= numberGamedays; i++)
                    {
                        List<Team> sortedRatings = new();
                        List<Team> sortedReal = new();

                        if (method.Equals("Elo"))
                        {
                            for (K = 10; K <= 100; K += 10)
                            {
                                for (xi = 100; xi <= 1000; xi += 100)
                                {
                                    ApplyRatingMethods(season, i, method, K, xi, ref numberTeams, ref sortedRatings, ref sortedReal);
                                    int diff = CalculateDifference(sortedReal, sortedRatings);
                                    csvData.AppendLine(method + ";" + season + ";" + i + ";" + K + ";" + xi + ";" + diff);
                                }
                            }
                        }
                        else
                        {
                            ApplyRatingMethods(season, i, method, K, xi, ref numberTeams, ref sortedRatings, ref sortedReal);
                            int diff = CalculateDifference(sortedReal, sortedRatings);
                            csvData.AppendLine(method + ";" + season + ";" + i + ";;;" + diff);
                        }                        
                    }
                }
            }

            File.WriteAllText(data + "\\Results\\RatingMethods_Differences.csv", csvData.ToString());
        }

        private int CalculateDifference(List<Team> sortedReal, List<Team> sortedRatings)
        {
            int diff = 0;

            for (int i = 0; i < sortedReal.Count; i++)
            {
                diff += Math.Abs(i - sortedRatings.IndexOf(sortedRatings.Find(s => s.name.Equals(sortedReal[i].name))));
            }

            return diff;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem.Equals("Elo"))
            {
                tableLayoutPanel7.Visible = true;
            }
            else
            {
                tableLayoutPanel7.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            CalculateDifferences();
            Cursor.Current = Cursors.Default;
        }
    }
}