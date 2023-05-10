using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    class AdjacencyMatrix : Graph
    {
        private List<List<float>> adjacencyMatrix;

        public AdjacencyMatrix(List<List<float>> adjacencyMatrix, int[] counts)
        {
            this.adjacencyMatrix = adjacencyMatrix;
            this.counts = counts;
        }

        public override Graph copy()
        {
            Graph g = new AdjacencyMatrix(new(),counts);

            foreach (Edge e in getEdges())
            {
                g.addEdge(e.n1, e.n2, e.c);
            }

            return g;
        }

        public override void addNode(int n)
        {
            counts[0]++;

            int size = getSize();

            for (int j = n - size; j >= 0; j--)
            {
                adjacencyMatrix.Add(new());

                for (int i = 0; i < getSize(); i++)
                {
                    if (i != getSize() - 1)
                    {
                        adjacencyMatrix[i].Add(0);
                    }
                    
                    adjacencyMatrix[getSize() - 1].Add(0);
                }
            }
        }

        public override void removeNode(int n)
        {
            counts[1]++;

            if (n >= getSize())
            {
                return;
            }

            for (int i = 0; i < getSize(); i++)
            {
                adjacencyMatrix[i][n] = 0;
                adjacencyMatrix[n][i] = 0;
            }
        }

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

            adjacencyMatrix[m][n] = c;
        }

        public override void removeEdge(int m, int n)
        {
            counts[3]++;

            if (m <= getSize() && n <= getSize())
            {
                adjacencyMatrix[m][n] = 0;
            }
        }

        public override List<Edge> getEdges()
        {
            counts[4]++;

            List<Edge> edges = new();

            for (int i = 0; i < getSize(); i++)
            {
                for (int j = 0; j < getSize(); j++)
                {
                    if (adjacencyMatrix[i][j] > 0)
                    {
                        edges.Add(new Edge(i, j, adjacencyMatrix[i][j]));
                    }
                }
            }

            return edges;
        }

        public override List<Edge> getNeighbours(int n)
        {
            counts[5]++;

            List<Edge> neighbours = new();

            for (int i = 0; i < getSize(); i++)
            {
                if (adjacencyMatrix[n][i] != 0)
                {
                    neighbours.Add(new Edge(n,i, adjacencyMatrix[n][i]));
                }
            }

            return neighbours;
        }

        public override List<Edge> getPredecessors(int n)
        {
            counts[6]++;

            List<Edge> predecessors = new();

            for (int i = 0; i < getSize(); i++)
            {
                if (adjacencyMatrix[i][n] != 0)
                {
                    predecessors.Add(new Edge(i, n, adjacencyMatrix[i][n]));
                }
            }

            return predecessors;
        }

        public override void setCapacity(int n1, int n2, float c)
        {
            counts[7]++;

            adjacencyMatrix[n1][n2] = c;
        }

        public override void addCapacity(int n1, int n2, float c)
        {
            counts[8]++;

            adjacencyMatrix[n1][n2] += c;
        }

        public override float getCapacity(int n1, int n2)
        {
            counts[9]++;

            return adjacencyMatrix[n1][n2];
        }

        public override int getSize()
        {
            counts[10]++;

            return adjacencyMatrix.Count;
        }
    }
}
