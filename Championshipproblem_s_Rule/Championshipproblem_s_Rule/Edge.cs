using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    // data structure to store an edge
    class Edge
    {
        public int n1; // start node
        public int n2; // end node
        public float c; // capacity

        public Edge(int n1, int n2, float c)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.c = c;
        }

        // check if a list of edges contains an edge with one specific node
        public static Edge contains(List<Edge> edges, int? n1, int? n2)
        {
            foreach (Edge e in edges)
            {
                if (n1 is not null)
                {
                    if (n2 is not null)
                    {
                        if (e.n1.Equals(n1) && e.n2.Equals(n2))
                        {
                            return e;
                        }
                    }
                    else
                    {
                        if (e.n1.Equals(n1))
                        {
                            return e;
                        }
                    }
                }
                else
                {
                    if (n2 is not null)
                    {
                        if (e.n2.Equals(n2))
                        {
                            return e;
                        }
                    }
                    else
                    {
                        return e;
                    }
                }
            }

            return null;
        }
    }
}