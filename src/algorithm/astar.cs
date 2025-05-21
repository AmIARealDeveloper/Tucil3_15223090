using System;
using System.Collections.Generic;

namespace RushHour
{
    public class AStar
    {
        public static (List<State> path, int nodesVisited) Solve(State initialState, int heuristicChoice)
        {
            int nodesVisited = 0;
            var openSet = new PriorityQueue<Node>((a, b) => (a.Cost + a.Heuristic) < (b.Cost + b.Heuristic) ? -1 : 1);
            var closedSet = new HashSet<string>();
            openSet.Enqueue(new Node(initialState, null, 0, CalculateHeuristic(initialState, heuristicChoice)));
            
            Console.WriteLine("Memulai eksplorasi A Star");
            DateTime startTime = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            while (openSet.Count > 0)
            {
                if (DateTime.Now - startTime > timeout)
                {
                    Console.WriteLine("Timeout tercapai setelah 30 detik. Tidak ada solusi ditemukan.");
                    return (null, nodesVisited);
                }

                var current = openSet.Dequeue();
                nodesVisited++;

                if (current.State.IsGoal())
                {
                    Console.WriteLine($"Hasil tercapai setelah {nodesVisited} simpul.");
                    return (ReconstructPath(current), nodesVisited);
                }

                closedSet.Add(current.State.GetHash());

                foreach (var move in current.State.GetPossibleMoves())
                {
                    var newState = current.State.ApplyMove(move);
                    if (closedSet.Contains(newState.GetHash())) continue;

                    int tentativeCost = current.Cost + 1;
                    var newNode = new Node(newState, current, tentativeCost, CalculateHeuristic(newState, heuristicChoice));
                    openSet.Enqueue(newNode);
                }
            }
            Console.WriteLine("Tidak ditemukan solusi.");
            return (null, nodesVisited);
        }

        private static int CalculateHeuristic(State state, int heuristicChoice)
        {
            // heuristik: 1 = blocking vehicles, 2 = blocking vehicles + Manhattan distance
            if (heuristicChoice == 1)
            {
                return state.GetBlockingVehicles();
            }
            else
            {
                return state.GetBlockingVehicles() + state.GetManhattanDistance();
            }
        }

        private static List<State> ReconstructPath(Node node)
        {
            var path = new List<State>();
            while (node != null)
            {
                path.Add(node.State);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}