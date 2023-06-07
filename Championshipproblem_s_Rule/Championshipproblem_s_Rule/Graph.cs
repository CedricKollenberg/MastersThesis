using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    abstract class Graph
    {
        protected int[] counts;

        abstract public Graph copy(); // copy graph
        abstract public void addNode(int n); // add node to the graph
        abstract public void removeNode(int n); // remove node from the graph
        abstract public void addEdge(int m, int n, float c); // add edge to the graph
        abstract public void removeEdge(int m, int n); // remove edge from the graph
        
        // get all nodes in a list
        public List<int> getNodes()
        {
            List<int> nodes = new();

            for (int i = 0; i < getSize(); i++)
            {
                nodes.Add(i);
            }

            return nodes;
        }
        abstract public List<Edge> getEdges(); // get all edges
        abstract public List<Edge> getNeighbours(int n); // get all neighbours
        abstract public List<Edge> getPredecessors(int n); // get all predecessors

        // breath first search algorithm
        public List<int> breadthFirstSearch(int start, int dest)
        {
            List<int> way = new List<int>(); // initialize path

            int size = getSize();

            bool[] visited = new bool[size]; // array of visited nodes
            int[] predecessors = new int[size]; // array of predecessors on the path

            
            for (int i = 0; i < size; i++)
            {
                visited[i] = false;
                predecessors[i] = -1;
            }

            Queue<int> q = new(); // queue for nodes to be considered
            q.Enqueue(start);
            visited[start] = true;

            while (q.Count > 0)
            {
                int u = q.Dequeue();

                foreach (Edge e in getNeighbours(u)) // get all neighbours
                {
                    int i = e.n2;

                    if (!visited[i]) // check if already visited
                    {
                        visited[i] = true;
                        predecessors[i] = u; // set predecessor

                        if (i == dest) // if destination is reached
                        {
                            int node = i;

                            do
                            {
                                way.Add(node); // add nodes to the path
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

        // get in-degree of a node
        public float getInDegree(int i)
        {
            float degree = 0;

            for (int j = 0; j < getSize(); j++)
            {
                degree += getCapacity(j, i);
            }

            return degree;
        }

        // get out-degree of a node
        public float getOutDegree(int i)
        {
            float degree = 0;

            for (int j = 0; j < getSize(); j++)
            {
                degree += getCapacity(i, j);
            }

            return degree;
        }

        abstract public void setCapacity(int n1, int n2, float c); // set the capacity of an edge
        abstract public void addCapacity(int n1, int n2, float c); // add a value to an edge capacity
        abstract public float getCapacity(int n1, int n2); // get the capacity of an edge
        abstract public int getSize(); // get numberof nodes of the graph

        // get number of graph-specif operations
        public int[] getCounts()
        {
            return counts;
        }

        // add values to the number of graph-specific operations
        public void addCounts(int[] counts)
        {
            for (int i = 0; i < 11; i++)
            {
                this.counts[i] += counts[i];
            }
        }
    }
}