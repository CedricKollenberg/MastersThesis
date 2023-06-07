using System;
using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    // class to calculate the max flow of a network
    // implemented algorithms
    //      - Edmonds-Karp
    //      - Dinic
    //      - push relabel
    //      - relabel to front
    class MaxFlow
    {
        // Edmonds-Karp algorithm (based on Introduction to Algorithms, Cormen et al.)
        public static float EdmondsKarp(Graph graph, ref int iterations)
        {
            int size = graph.getSize(); // initialize size

            // initialize flow
            float[,] flowMatrix = new float[size, size]; 

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (graph.getCapacity(i, j) == 0)
                    {
                        flowMatrix[i, j] = -1;
                    }
                    else
                    {
                        flowMatrix[i, j] = 0;
                    }
                }
            }

            List<int> path; // declare path from source to sink

            while ((path = graph.breadthFirstSearch(0, size - 1)).Count > 0) // find path from source to sink via the breadth first search
            {
                iterations++;

                // extract min capacity on the path
                float minVal = float.MaxValue;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    int n1 = path[i];
                    int n2 = path[i + 1];

                    float actVal = graph.getCapacity(n1, n2);

                    if (actVal < minVal)
                    {
                        minVal = actVal;
                    }
                }

                // adjust capacity of the edges
                for (int i = 0; i < path.Count - 1; i++)
                {
                    int n1 = path[i];
                    int n2 = path[i + 1];

                    if (flowMatrix[n1, n2] == -1)
                    {
                        flowMatrix[n2, n1] -= minVal;
                    }
                    else
                    {
                        flowMatrix[n1, n2] += minVal;
                    }

                    graph.addCapacity(n1, n2, -minVal);
                    graph.addCapacity(n2, n1, minVal);
                }
            }

            return checkResult(flowMatrix, size); // get flow value
        }

        // Dinic's algorithm (based on Algorithmische Graphentheorie, Turau and Weyer)
        public static float Dinic(Graph graph, int mode, ref int iterations)
        {
            // set graph-specific operations counter
            int[] counts = new int[11];
            for (int i = 0; i < 11; i++)
            {
                counts[i] = 0;
            }

            switch (mode)
            {
                case 0: // adjacency matrix
                    return DinicProcedure(graph, new AdjacencyMatrix(new(), counts), ref iterations, counts);
                case 1: // adjacency list
                    return DinicProcedure(graph, new AdjacencyList(new(), counts), ref iterations, counts);
                default:
                    return 0;
            }
        }

        // 
        private static float DinicProcedure(Graph graph, Graph levelNetwork, ref int iterations, int[] counts)
        {
            int size = graph.getSize(); // initialize size

            // initialize flow
            float[,] flowMatrix = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    flowMatrix[i, j] = 0;
                }
            }

            while (helpNetwork(graph, flowMatrix, levelNetwork)) // create level network
            {
                iterations++;
                blockFlow(levelNetwork, flowMatrix); // calculate blocking flow and increase flow
                graph.addCounts(levelNetwork.getCounts());
            }

            return checkResult(flowMatrix, size); // calculate flow value
        }

        // create level network, ref: Algorithmische Graphentheorie pp. 237-239
        private static bool helpNetwork(Graph graph, float[,] flow, Graph helpNetwork)
        {
            int size = graph.getSize(); // initialize size

            // initialize visited array
            int[] visited = new int[size];

            for (int i = 0; i < size; i++)
            {
                visited[i] = size - 1;
            }

            visited[0] = 0;

            Queue<int> q = new(); // set queue for nodes            

            helpNetwork.addNode(0); // add source to level network

            q.Enqueue(0); // add source to queue

            do
            {
                int n = q.Dequeue(); // remove first node of queue

                foreach (Edge m in graph.getNeighbours(n)) // check out all neighbours
                {
                    if (visited[m.n2] > visited[n] && flow[n, m.n2] < m.c) // if neighbour is visited later and there is still capacity
                    {
                        if (visited[m.n2].Equals(size - 1)) // if not visited yet
                        {
                            q.Enqueue(m.n2); // add to queue
                            visited[m.n2] = visited[n] + 1; // set visited value
                            helpNetwork.addNode(m.n2); // add node to level network
                        }

                        helpNetwork.addEdge(n, m.n2, m.c - flow[n, m.n2]); // add edge to level network
                    }
                }
                foreach (Edge m in graph.getPredecessors(n)) // check out all predecessors
                {
                    if (visited[m.n1] > visited[n] && flow[m.n1, n] > 0) // if predecessor is visited later and there is a reverse flow
                    {
                        if (visited[m.n1].Equals(size - 1)) // if not visited yet
                        {
                            q.Enqueue(m.n1); // add to queue
                            visited[m.n1] = visited[n] + 1; // set visited value
                            helpNetwork.addNode(m.n1); // add node to level network
                        }

                        helpNetwork.addEdge(n, m.n1, flow[m.n1, n]); // add edge to level network
                    }
                }
            }
            while (q.Count > 0 && visited[size - 1] > visited[q.Peek()]); // as long as there are elements and the sink is not reached

            // remove irrelevant nodes and edges
            if (visited[size - 1] < size - 1)
            {
                for (int i = 0; i < size - 1; i++)
                {
                    if (visited[size - 1].Equals(visited[i]))
                    {
                        helpNetwork.removeNode(i);
                    }
                }

                return true;
            }

            return false;
        }

        // create blocking flow, ref: Algorithmische Graphentheorie pp. 239-240
        private static void blockFlow(Graph g, float[,] flow)
        {
            // flow arrays
            float[] D = new float[g.getSize()];
            float[] Dplus = new float[g.getSize()];
            float[] Dminus = new float[g.getSize()];

            // initialize arrays
            for (int i = 0; i < g.getSize(); i++)
            {
                Dplus[i] = 0;
                Dminus[i] = 0;

                foreach (Edge e in g.getNeighbours(i))
                {
                    Dplus[i] += e.c;
                }

                foreach (Edge e in g.getPredecessors(i))
                {
                    Dminus[i] += e.c;
                }
            }

            Dminus[0] = float.MaxValue;
            Dplus[g.getSize() - 1] = float.MaxValue;

            // initialized sets of nodes
            List<int> E = new();
            List<Edge> K = new();
            Stack<int> S;

            E = g.getNodes();
            K = g.getEdges();

            do
            {
                // get min value
                float min = float.MaxValue;
                int minNode = 0;

                foreach (int n in E)
                {
                    D[n] = Math.Min(Dplus[n], Dminus[n]);

                    if (D[n] < min)
                    {
                        min = D[n];
                        minNode = n;
                    }
                }

                S = new();

                // expand
                expandBackward(g, minNode, flow);
                expandForward(g, minNode, flow);

                // add min node to stack
                if (!S.Contains(minNode))
                {
                    S.Push(minNode);
                }

                // adjust capacities of edges
                while (S.Count > 0)
                {
                    int e = S.Pop();

                    E.Remove(e);

                    for (int i = 0; i < K.Count; i++)
                    {
                        Edge k = K[i];

                        if (k.n1.Equals(e))
                        {
                            Dminus[k.n2] -= g.getCapacity(e, k.n2);

                            if (Dminus[k.n2].Equals(0))
                            {
                                S.Push(k.n2);
                            }

                            K.Remove(k);
                            i--;
                        }
                        else if (k.n2.Equals(e))
                        {
                            Dplus[k.n1] -= g.getCapacity(k.n1, e);

                            if (Dplus[k.n1].Equals(0))
                            {
                                S.Push(k.n1);
                            }

                            K.Remove(k);
                            i--;
                        }
                    }
                }
            }
            while (E.Contains(0) && E.Contains(g.getSize() - 1));

            return;

            // expand backward, ref: Algorithmische Graphentheorie pp. 240-241
            void expandBackward(Graph g, int e, float[,] flow)
            {
                float[] f = new float[g.getSize()];  // current flow

                Queue<int> q = new(); // initiliaze queue

                for (int i = 0; i < g.getSize(); i++)
                {
                    f[i] = 0;
                }

                f[e] = D[e]; // set current flow
                q.Enqueue(e); // add current node to queue

                do
                {
                    int i = q.Dequeue(); // current node
                    Edge k; // current edge

                    while (f[i] != 0 && (k = Edge.contains(K, null, i)) is not null) // while there still is a current flow and the current edge has a capacity
                    {
                        float m = Math.Min(g.getCapacity(k.n1, i), f[i]); // get minimum of the two

                        flow[k.n1, i] += m; // add flow
                        g.addCapacity(k.n1, i, -m); // adjust capacity

                        if (g.getCapacity(k.n1, i).Equals(0))
                        {
                            K.Remove(k); // remove edge
                        }

                        Dminus[i] -= m; // adjust incoming value

                        if (Dminus[i].Equals(0))
                        {
                            S.Push(i);
                        }

                        Dplus[k.n1] -= m; // adjust outgoing value

                        if (Dplus[k.n1].Equals(0))
                        {
                            S.Push(k.n1);
                        }

                        q.Enqueue(k.n1); // add partner node to queue

                        // adjust current flow
                        f[k.n1] += m;
                        f[i] -= m;
                    }
                }
                while (q.Count > 0);
            }

            // expand forward, ref: Algorithmische Graphentheorie pp. 240-241
            void expandForward(Graph g, int e, float[,] flow)
            {
                float[] f = new float[g.getSize()];  // current flow

                Queue<int> q = new(); // initiliaze queue

                for (int i = 0; i < g.getSize(); i++)
                {
                    f[i] = 0;
                }

                f[e] = D[e]; // set current flow
                q.Enqueue(e); // add current node to queue

                do
                {
                    int i = q.Dequeue();// current node
                    Edge k; // current edge

                    while (f[i] != 0 && (k = Edge.contains(K, i, null)) is not null)  // while there still is a current flow and the current edge has a capacity
                    {
                        float m = Math.Min(g.getCapacity(i, k.n2), f[i]); // get minimum of the two

                        flow[i, k.n2] += m; // add flow
                        g.addCapacity(i, k.n2, -m); // adjust capacity

                        if (g.getCapacity(i, k.n2).Equals(0))
                        {
                            K.Remove(k); // remove edge
                        }

                        Dminus[k.n2] -= m; // adjust incoming value

                        if (Dminus[k.n2].Equals(0))
                        {
                            S.Push(k.n2);
                        }

                        Dplus[i] -= m; // adjust outgoing value

                        if (Dplus[i].Equals(0))
                        {
                            S.Push(i);
                        }

                        q.Enqueue(k.n2); // add partner node to queue

                        // adjust current flow
                        f[k.n2] += m;
                        f[i] -= m;
                    }
                }
                while (q.Count > 0);
            }
        }

        // push relabel algorithm
        // based on Introduction to Algorithms, Cormen et al.
        // mode 0 = generic (chapter 26.4)
        // mode 1 = realbel to front (chapter 26.5)
        public static float PushRelabel(Graph graph, int mode, ref int iterations)
        {
            int size = graph.getSize(); // initialize size

            // initialze relevant variables
            float[,] flowMatrix = new float[size, size];
            int[] heights = new int[size];
            float[] excessFlows = new float[size];
            List<LinkedList<int>> neighbours = new();
            List<LinkedListNode<int>> pointers = new();
            LinkedList<int> order = new();

            initializeFlow(); // initialize preflow

            switch (mode)
            {
                case 0: // generic
                    bool changed = true;

                    while (changed) // since there was an operation
                    {
                        iterations++;

                        changed = false;

                        // relabel
                        for (int i = 1; i < size - 1; i++)
                        {
                            if (relabel(i))
                            {
                                changed = true;
                            }
                        }

                        // push
                        foreach (Edge e in graph.getEdges())
                        {
                            if (push(e.n1, e.n2))
                            {
                                changed = true;
                            }
                        }
                    }

                    break;
                case 1: // relabel to front
                    LinkedListNode<int> u = order.First; // get first node of ordered list

                    while (u is not null)
                    {
                        iterations++;

                        int oldHeight = heights[u.Value]; // save old height

                        discharge(u.Value); // discharge

                        if (heights[u.Value] > oldHeight) // check new height
                        {
                            // switch u to front
                            order.Remove(u);
                            order.AddFirst(u);
                        }

                        u = u.Next; // set next node
                    }

                    break;
            }

            return checkResult(flowMatrix,size); // get flow value

            // discharge
            void discharge(int n)
            {
                while (excessFlows[n] > 0) // as long as there is an excess flow
                {
                    LinkedListNode<int> v = pointers[n]; // get next node

                    if (v is not null)
                    {
                        if (graph.getCapacity(n,v.Value) > 0 && heights[n] == heights[v.Value] + 1)
                        {
                            push(n, v.Value); // push excess
                        }
                        else
                        {
                            pointers[n] = v.Next; // set next element as pointer
                        }
                    }
                    else
                    {
                        relabel(n); // relabel
                        pointers[n] = neighbours[n].First; // set pointer to the front of the list
                    }
                }
            }

            // initialize flow
            void initializeFlow()
            {
                for (int i = 0; i < size; i++)
                {
                    heights[i] = 0; // set height
                    excessFlows[i] = 0; // set excess flow
                    neighbours.Add(new()); // initialize neighbours

                    // set each neighbour
                    foreach (Edge m in graph.getNeighbours(i))
                    {
                        neighbours[i].AddLast(new LinkedListNode<int>(m.n2));
                    }
                    // set each predecessor
                    foreach (Edge m in graph.getPredecessors(i))
                    {
                        neighbours[i].AddLast(new LinkedListNode<int>(m.n1));
                    }

                    pointers.Add(neighbours[i].First); // set pointer to the first neighbour

                    if (i != 0 && i != size - 1) // not for the source and sink
                    {
                        order.AddLast(i); // add nodes at the end
                    }

                    // fill flow matrix 
                    for (int j = 0; j < size; j++)
                    {
                        if (graph.getCapacity(i, j) == 0) // not an edge in the graph
                        {
                            flowMatrix[i, j] = -1;
                        }
                        else // edge in the graph
                        {
                            flowMatrix[i, j] = 0;
                        }
                    }
                }

                heights[0] = size - 1; // set height of source

                List<Edge> n = graph.getNeighbours(0);

                // manage excess for the neighbours of the source
                for (int i = 0; i < n.Count;)
                {
                    Edge m = n[i];

                    float c = m.c;

                    graph.removeEdge(0, m.n2);
                    n.Remove(m);
                    graph.setCapacity(m.n2, 0, m.c);

                    flowMatrix[0, m.n2] += c;
                    excessFlows[m.n2] += c;
                }
            }

            // push excess
            bool push(int n1, int n2)
            {
                if (excessFlows[n1] > 0 && heights[n1] == heights[n2] + 1) // if there is an excess and an appropriate neighbour
                {
                    float tempFlow = Math.Min(excessFlows[n1], graph.getCapacity(n1, n2)); // get minimum of excess flow and edge capacity

                    // adjust capacities
                    graph.addCapacity(n1, n2, -tempFlow);
                    graph.addCapacity(n2, n1, tempFlow);

                    // adjust flow
                    if (flowMatrix[n1,n2] == -1)
                    {
                        flowMatrix[n2, n1] -= tempFlow;
                    }
                    else
                    {
                        flowMatrix[n1, n2] += tempFlow;
                    }

                    // adjust excess
                    if (n1 != 0 && n1 != size-1)
                    {
                        excessFlows[n1] -= tempFlow;
                    }

                    if (n2 != 0 && n2 != size-1)
                    {
                        excessFlows[n2] += tempFlow;
                    }

                    return true;
                }

                return false;
            }

            // relabel
            bool relabel(int n)
            {
                int minHeight;

                if (excessFlows[n] > 0 && (minHeight = checkNeighboursHeight(n)) >= 0) // get minimum height of the neighbours
                {
                    heights[n] = 1 + minHeight; // adjust height
                    return true;
                }

                return false;
            }

            // get the minimum height of the neighbours
            int checkNeighboursHeight(int n)
            {
                int minHeight = int.MaxValue;

                foreach (Edge m in graph.getNeighbours(n))
                {
                    if (heights[n] > heights[m.n2])
                    {
                        return -1;
                    }
                    
                    if (heights[m.n2] < minHeight)
                    {
                        minHeight = heights[m.n2];
                    }
                }

                if (minHeight.Equals(int.MaxValue))
                {
                    return -minHeight;
                }
                return minHeight;
            }
        }

        // calculate the flow value
        public static float checkResult(float[,] flowMatrix, int size)
        {
            float flow = 0;

            for (int i = 1; i < size; i++)
            {
                if (flowMatrix[i, size - 1] > 0)
                {
                    flow += flowMatrix[i, size - 1];
                }
            }

            return flow;
        }
    }
}