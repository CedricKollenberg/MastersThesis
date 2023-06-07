using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    // data structure for the adjacency list
    class AdjacencyList : Graph
    {
        private List<List<Edge>> adjacencyList;

        // constructor
        public AdjacencyList(List<List<Edge>> adjacencyList, int[] counts)
        {
            this.adjacencyList = adjacencyList;
            this.counts = counts;
        }

        // copy graph
        public override Graph copy()
        {
            List<List<Edge>> g = new();

            for (int i = 0; i < adjacencyList.Count; i++)
            {
                g.Add(new());

                foreach (Edge e in adjacencyList[i])
                {
                    g[i].Add(new Edge(e.n1,e.n2,e.c));
                }
            }

            return new AdjacencyList(g,counts);
        }

        // add node to the graph
        public override void addNode(int n)
        {
            counts[0]++;

            int size = getSize();

            for (int j = n - size; j >= 0; j--)
            {
                adjacencyList.Add(new());
            }
        }

        // remove node from the graph
        public override void removeNode(int n)
        {
            counts[1]++;

            if (n >= getSize())
            {
                return;
            }

            adjacencyList[n] = new();

            for (int i = 0; i < getSize(); i++)
            {
                for (int j = 0; j < adjacencyList[i].Count; j++)
                {
                    Edge e = adjacencyList[i][j];

                    if (e.n2.Equals(n))
                    {
                        adjacencyList[i].RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        // add edge to the graph
        public override void addEdge(int m, int n, float c)
        {
            counts[2]++;

            if (m >= getSize())
            {
                addNode(m);
            }

            if (n >= getSize())
            {
                addNode(n);
            }

            adjacencyList[m].Add(new Edge(m,n,c));
        }

        // remove edge from the graph
        public override void removeEdge(int m, int n)
        {
            counts[3]++;

            if (m <= getSize() && n <= getSize())
            {
                adjacencyList[m].RemoveAll(e => e.n2.Equals(n));
            }
        }

        // get all the edges of the graph
        public override List<Edge> getEdges()
        {
            counts[4]++;

            List<Edge> edges = new();

            for (int i = 0; i < getSize(); i++)
            {
                foreach (Edge e in adjacencyList[i])
                {
                    edges.Add(e);
                }
            }

            return edges;
        }

        // get all neighbours
        public override List<Edge> getNeighbours(int n)
        {
            counts[5]++;

            return adjacencyList[n];
        }

        // get all predecessors
        public override List<Edge> getPredecessors(int n)
        {
            counts[6]++;

            List<Edge> predecessors = new();

            for (int i = 0; i < getSize(); i++)
            {
                foreach (Edge e in adjacencyList[i])
                {
                    if (e.n2.Equals(n))
                    {
                        predecessors.Add(e);
                        break;
                    }
                }
            }

            return predecessors;
        }

        // set the capacity of an edge
        public override void setCapacity(int n1, int n2, float c)
        {
            counts[7]++;

            Edge e;

            if ((e = adjacencyList[n1].Find(m => m.n2.Equals(n2))) is null)
            {
                e = new Edge(n1, n2, c);
                adjacencyList[n1].Add(e);

                return;
            }

            e.c = c;

            if (e.c == 0)
            {
                adjacencyList[n1].Remove(e);
            }
        }

        // add a value to an edge capacity
        public override void addCapacity(int n1, int n2, float c)
        {
            counts[8]++;

            Edge e;

            if ((e = adjacencyList[n1].Find(m => m.n2.Equals(n2))) is null)
            {
                e = new Edge(n1, n2, c);
                adjacencyList[n1].Add(e);

                return;
            }

            e.c += c;

            if (e.c == 0)
            {
                adjacencyList[n1].Remove(e);
            }
        }

        // get the capacity of an edge
        public override float getCapacity(int n1, int n2)
        {
            counts[9]++;

            Edge e;

            if ((e = adjacencyList[n1].Find(m => m.n2.Equals(n2))) is null)
            {
                return 0;
            }

            return e.c;
        }

        // get the number of nodes of the graph
        public override int getSize()
        {
            counts[10]++;

            return adjacencyList.Count;
        }
    }
}