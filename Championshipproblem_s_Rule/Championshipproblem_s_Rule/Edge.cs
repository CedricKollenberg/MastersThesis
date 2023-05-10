using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    class Edge
    {
        public int n1;
        public int n2;
        public float c;

        public Edge(int n1, int n2, float c)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.c = c;
        }

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
