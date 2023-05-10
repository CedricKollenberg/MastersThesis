using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    abstract class Graph
    {
        protected int[] counts;

        abstract public Graph copy();
        abstract public void addNode(int n);
        abstract public void removeNode(int n);
        abstract public void addEdge(int m, int n, float c);
        abstract public void removeEdge(int m, int n);
        public List<int> getNodes()
        {
            List<int> nodes = new();

            for (int i = 0; i < getSize(); i++)
            {
                nodes.Add(i);
            }

            return nodes;
        }
        abstract public List<Edge> getEdges();
        abstract public List<Edge> getNeighbours(int n);
        abstract public List<Edge> getPredecessors(int n);
        public List<int> breadthFirstSearch(int start, int dest)
        {
            List<int> way = new List<int>();

            int size = getSize();

            bool[] visited = new bool[size];
            int[] predecessors = new int[size];

            for (int i = 0; i < size; i++)
            {
                visited[i] = false;
                predecessors[i] = -1;
            }

            Queue<int> q = new();
            q.Enqueue(start);
            visited[start] = true;

            while (q.Count > 0)
            {
                int u = q.Dequeue();

                foreach (Edge e in getNeighbours(u))
                {
                    int i = e.n2;

                    if (!visited[i])
                    {
                        visited[i] = true;
                        predecessors[i] = u;

                        if (i == dest)
                        {
                            int node = i;

                            do
                            {
                                way.Add(node);
                                node = predecessors[node];
                            }
                            while (node != -1);

                            way.Reverse();

                            return way;
                        }

                        q.Enqueue(i);
                    }
                }
            }

            return way;
        }
        public float getInDegree(int i)
        {
            float degree = 0;

            for (int j = 0; j < getSize(); j++)
            {
                degree += getCapacity(j, i);
            }

            return degree;
        }

        public float getOutDegree(int i)
        {
            float degree = 0;

            for (int j = 0; j < getSize(); j++)
            {
                degree += getCapacity(i, j);
            }

            return degree;
        }

        abstract public void setCapacity(int n1, int n2, float c);
        abstract public void addCapacity(int n1, int n2, float c);
        abstract public float getCapacity(int n1, int n2);
        abstract public int getSize();

        public int[] getCounts()
        {
            return counts;
        }

        public void addCounts(int[] counts)
        {
            for (int i = 0; i < 11; i++)
            {
                this.counts[i] += counts[i];
            }
        }
    }
}
