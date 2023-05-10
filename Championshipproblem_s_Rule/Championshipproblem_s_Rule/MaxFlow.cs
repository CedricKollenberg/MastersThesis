using System;
using System.Collections.Generic;

namespace Championshipproblem_s_Rule
{
    class MaxFlow
    {
        public static void blockFlow(Graph g, float[,] flow)
        {
            float[] D = new float[g.getSize()];
            float[] Dplus = new float[g.getSize()];
            float[] Dminus = new float[g.getSize()];

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

            List<int> E = new();
            List<Edge> K = new();
            Stack<int> S;

            E = g.getNodes();
            K = g.getEdges();

            do
            {
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

                expandBackward(g, minNode, flow);
                expandForward(g, minNode, flow);

                if (!S.Contains(minNode))
                {
                    S.Push(minNode);
                }

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

            void expandBackward(Graph g, int e, float[,] flow)
            {
                float[] f = new float[g.getSize()];

                Queue<int> q = new();

                for (int i = 0; i < g.getSize(); i++)
                {
                    f[i] = 0;
                }

                f[e] = D[e];
                q.Enqueue(e);

                do
                {
                    int i = q.Dequeue();
                    Edge k;

                    while (f[i] != 0 && (k = Edge.contains(K, null, i)) is not null)
                    {
                        float m = Math.Min(g.getCapacity(k.n1, i), f[i]);

                        flow[k.n1, i] += m;
                        g.addCapacity(k.n1, i, -m);

                        if (g.getCapacity(k.n1, i).Equals(0))
                        {
                            K.Remove(k);
                        }

                        Dminus[i] -= m;

                        if (Dminus[i].Equals(0))
                        {
                            S.Push(i);
                        }

                        Dplus[k.n1] -= m;

                        if (Dplus[k.n1].Equals(0))
                        {
                            S.Push(k.n1);
                        }

                        q.Enqueue(k.n1);
                        f[k.n1] += m;
                        f[i] -= m;
                    }
                }
                while (q.Count > 0);
            }

            void expandForward(Graph g, int e, float[,] flow)
            {
                float[] f = new float[g.getSize()];

                Queue<int> q = new();

                for (int i = 0; i < g.getSize(); i++)
                {
                    f[i] = 0;
                }

                f[e] = D[e];
                q.Enqueue(e);

                do
                {
                    int i = q.Dequeue();
                    Edge k;

                    while (f[i] != 0 && (k = Edge.contains(K, i, null)) is not null)
                    {
                        float m = Math.Min(g.getCapacity(i, k.n2), f[i]);

                        flow[i, k.n2] += m;
                        g.addCapacity(i, k.n2, -m);

                        if (g.getCapacity(i, k.n2).Equals(0))
                        {
                            K.Remove(k);
                        }

                        Dminus[k.n2] -= m;

                        if (Dminus[k.n2].Equals(0))
                        {
                            S.Push(k.n2);
                        }

                        Dplus[i] -= m;

                        if (Dplus[i].Equals(0))
                        {
                            S.Push(i);
                        }

                        q.Enqueue(k.n2);
                        f[k.n2] += m;
                        f[i] -= m;
                    }
                }
                while (q.Count > 0);
            }
        }

        public static bool helpNetwork(Graph graph, float[,] flow, Graph helpNetwork)
        {
            int size = graph.getSize();

            int[] visited = new int[size];

            Queue<int> q = new();

            for (int i = 0; i < size; i++)
            {
                visited[i] = size - 1;
            }

            visited[0] = 0;

            helpNetwork.addNode(0);

            q.Enqueue(0);

            do
            {
                int n = q.Dequeue();

                foreach (Edge m in graph.getNeighbours(n))
                {
                    if (visited[m.n2] > visited[n] && flow[n, m.n2] < m.c)
                    {
                        if (visited[m.n2].Equals(size - 1))
                        {
                            q.Enqueue(m.n2);
                            visited[m.n2] = visited[n] + 1;
                            helpNetwork.addNode(m.n2);
                        }

                        helpNetwork.addEdge(n, m.n2, m.c - flow[n, m.n2]);
                    }
                }
                foreach (Edge m in graph.getPredecessors(n))
                {
                    if (visited[m.n1] > visited[n] && flow[m.n1, n] > 0)
                    {
                        if (visited[m.n1].Equals(size - 1))
                        {
                            q.Enqueue(m.n1);
                            visited[m.n1] = visited[n] + 1;
                            helpNetwork.addNode(m.n1);
                        }

                        helpNetwork.addEdge(n, m.n1, flow[m.n1, n]);
                    }
                }
            }
            while (q.Count > 0 && visited[size - 1] > visited[q.Peek()]);

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

        public static float Dinic(Graph graph, int mode, ref int iterations)
        {
            int size = graph.getSize();

            float[,] flowMatrix = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    flowMatrix[i, j] = 0;
                }
            }

            Graph helpN;

            int[] counts = new int[11];
            for (int i = 0; i < 11; i++)
            {
                counts[i] = 0;
            }

            switch (mode)
            {
                case 0:
                    while (helpNetwork(graph, flowMatrix, helpN = new AdjacencyMatrix(new(), counts)))
                    {
                        iterations++;
                        blockFlow(helpN, flowMatrix);
                        graph.addCounts(helpN.getCounts());
                    }

                    break;
                case 1:
                    while (helpNetwork(graph, flowMatrix, helpN = new AdjacencyList(new(), counts)))
                    {
                        iterations++;
                        blockFlow(helpN, flowMatrix);
                        graph.addCounts(helpN.getCounts());
                    }

                    break;
            }

            return checkResult(flowMatrix, size);
        }

        public static float EdmondsKarp(Graph graph, ref int iterations)
        {
            int size = graph.getSize();

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

            List<int> way;

            while ((way = graph.breadthFirstSearch(0, size - 1)).Count > 0)
            {
                iterations++;

                float minVal = float.MaxValue;

                for (int i = 0; i < way.Count - 1; i++)
                {
                    int n1 = way[i];
                    int n2 = way[i + 1];

                    float actVal = graph.getCapacity(n1, n2);

                    if (actVal < minVal)
                    {
                        minVal = actVal;
                    }
                }

                for (int i = 0; i < way.Count - 1; i++)
                {
                    int n1 = way[i];
                    int n2 = way[i + 1];

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

            return checkResult(flowMatrix,size);
        }

        public static float PushRelabel(Graph graph, int mode, ref int iterations)
        {
            int size = graph.getSize();

            float[,] flowMatrix = new float[size, size];
            int[] heights = new int[size];
            float[] excessFlows = new float[size];
            List<LinkedList<int>> neighbours = new();
            List<LinkedListNode<int>> pointers = new();
            LinkedList<int> order = new();

            initializeFlow();

            switch (mode)
            {
                case 0:
                    bool changed = true;

                    while (changed)
                    {
                        iterations++;

                        changed = false;

                        for (int i = 1; i < size - 1; i++)
                        {
                            if (relabel(i))
                            {
                                changed = true;
                            }
                        }

                        foreach (Edge e in graph.getEdges())
                        {
                            if (push(e.n1, e.n2))
                            {
                                changed = true;
                            }
                        }
                    }

                    break;
                case 1:

                    LinkedListNode<int> u = order.First;

                    while (u is not null)
                    {
                        iterations++;

                        int oldHeight = heights[u.Value];

                        discharge(u.Value);

                        if (heights[u.Value] > oldHeight)
                        {
                            order.Remove(u);
                            order.AddFirst(u);
                        }

                        u = u.Next;
                    }

                    break;
            }

            return checkResult(flowMatrix,size);

            void discharge(int n)
            {
                while (excessFlows[n] > 0)
                {
                    LinkedListNode<int> v = pointers[n];

                    if (v is not null)
                    {
                        if (graph.getCapacity(n,v.Value) > 0 && heights[n] == heights[v.Value] + 1)
                        {
                            push(n, v.Value);
                        }
                        else
                        {
                            pointers[n] = v.Next;
                        }
                    }
                    else
                    {
                        relabel(n);
                        pointers[n] = neighbours[n].First;
                    }
                }
            }

            void initializeFlow()
            {
                for (int i = 0; i < size; i++)
                {
                    heights[i] = 0;
                    excessFlows[i] = 0;
                    neighbours.Add(new());

                    foreach (Edge m in graph.getNeighbours(i))
                    {
                        neighbours[i].AddLast(new LinkedListNode<int>(m.n2));
                    }
                    foreach (Edge m in graph.getPredecessors(i))
                    {
                        neighbours[i].AddLast(new LinkedListNode<int>(m.n1));
                    }

                    pointers.Add(neighbours[i].First);

                    if (i != 0 && i != size - 1)
                    {
                        order.AddLast(i);
                    }

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

                heights[0] = size - 1;

                List<Edge> n = graph.getNeighbours(0);

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

            bool push(int n1, int n2)
            {
                if (excessFlows[n1] > 0 && heights[n1] == heights[n2] + 1)
                {
                    float tempFlow = Math.Min(excessFlows[n1], graph.getCapacity(n1, n2));

                    graph.addCapacity(n1, n2, -tempFlow);
                    graph.addCapacity(n2, n1, tempFlow);

                    if (flowMatrix[n1,n2] == -1)
                    {
                        flowMatrix[n2, n1] -= tempFlow;
                    }
                    else
                    {
                        flowMatrix[n1, n2] += tempFlow;
                    }

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

            bool relabel(int n)
            {
                int minHeight;

                if (excessFlows[n] > 0 && (minHeight = checkNeighboursHeight(n)) >= 0)
                {
                    heights[n] = 1 + minHeight;
                    return true;
                }

                return false;
            }

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
    }
}