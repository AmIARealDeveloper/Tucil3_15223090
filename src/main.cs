using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RushHour
{
    public class Vehicle
    {
        public char Id { get; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Length { get; }
        public bool IsHorizontal { get; }

        public Vehicle(char id, int row, int col, int length, bool isHorizontal)
        {
            Id = id;
            Row = row;
            Col = col;
            Length = length;
            IsHorizontal = isHorizontal;
        }

        public Vehicle Clone()
        {
            return new Vehicle(Id, Row, Col, Length, IsHorizontal);
        }
    }

    public class State
    {
        public List<Vehicle> Vehicles { get; }
        public char[,] Board { get; }
        public int Rows { get; }
        public int Cols { get; }
        public (int Row, int Col) Exit { get; }
        public char LastMoveId { get; set; }

        public State(char[,] board, List<Vehicle> vehicles, (int Row, int Col) exit)
        {
            Rows = board.GetLength(0);
            Cols = board.GetLength(1);
            Vehicles = vehicles.Select(v => v.Clone()).ToList();
            Board = (char[,])board.Clone();
            Exit = exit;
            LastMoveId = '\0';
        }

        public bool IsGoal()
        {
            var primary = Vehicles.First(vehicle => vehicle.Id == 'P');
            if (primary.IsHorizontal)
            {
                return primary.Row == Exit.Row && primary.Col + primary.Length - 1 == Exit.Col;
            }
            else
            {
                return primary.Col == Exit.Col && primary.Row + primary.Length - 1 == Exit.Row;
            }
        }

        public List<(Vehicle, int)> GetPossibleMoves()
        {
            var moves = new List<(Vehicle, int)>();
            foreach (var vehicle in Vehicles)
            {
                if (vehicle.IsHorizontal)
                {
                    int left = vehicle.Col - 1;
                    if (left >= 0 && Board[vehicle.Row, left] == '.')
                    {
                        moves.Add((vehicle, -1));
                    }
                    int right = vehicle.Col + vehicle.Length;
                    if (right < Cols && Board[vehicle.Row, right] == '.')
                    {
                        moves.Add((vehicle, 1));
                    }
                }
                else
                {
                    int up = vehicle.Row - 1;
                    if (up >= 0 && Board[up, vehicle.Col] == '.')
                    {
                        moves.Add((vehicle, -1));
                    }
                    int down = vehicle.Row + vehicle.Length;
                    if (down < Rows && Board[down, vehicle.Col] == '.')
                    {
                        moves.Add((vehicle, 1));
                    }
                }
            }
            return moves;
        }

        public State ApplyMove((Vehicle vehicle, int distance) move)
        {
            var newVehicles = Vehicles.Select(v => v.Clone()).ToList();
            var vehicle = newVehicles.First(v => v.Id == move.vehicle.Id);
            var newBoard = (char[,])Board.Clone();

            for (int i = 0; i < vehicle.Length; i++)
            {
                int r = vehicle.IsHorizontal ? vehicle.Row : vehicle.Row + i;
                int c = vehicle.IsHorizontal ? vehicle.Col + i : vehicle.Col;
                newBoard[r, c] = '.';
            }

            if (vehicle.IsHorizontal)
            {
                vehicle.Col += move.distance;
            }
            else
            {
                vehicle.Row += move.distance;
            }

            for (int i = 0; i < vehicle.Length; i++)
            {
                int r = vehicle.IsHorizontal ? vehicle.Row : vehicle.Row + i;
                int c = vehicle.IsHorizontal ? vehicle.Col + i : vehicle.Col;
                newBoard[r, c] = vehicle.Id;
            }
            var newState = new State(newBoard, newVehicles, Exit);
            newState.LastMoveId = vehicle.Id;
            return newState;
        }

        public int GetBlockingVehicles()
        {
            var primary = Vehicles.First(vehicle => vehicle.Id == 'P');
            int count = 0;
            if (primary.IsHorizontal)
            {
                for (int c = primary.Col + primary.Length; c < Exit.Col; c++)
                {
                    if (Board[primary.Row, c] != '.' && Board[primary.Row, c] != 'K')
                    {
                        count++;
                    }
                }
            }
            else
            {
                for (int r = primary.Row + primary.Length; r < Exit.Row; r++)
                {
                    if (Board[r, primary.Col] != '.' && Board[r, primary.Col] != 'K')
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int GetManhattanDistance()
        {
            var primary = Vehicles.First(v => v.Id == 'P');
            if (primary.IsHorizontal)
            {
                return Math.Abs(Exit.Col - (primary.Col + primary.Length - 1));
            }
            else
            {
                return Math.Abs(Exit.Row - (primary.Row + primary.Length - 1));
            }
        }

        public string GetHash()
        {
            return string.Join(",", Vehicles.Select(v => $"{v.Id}:{v.Row},{v.Col}"));
        }

        public void Print()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    char c = Board[i, j];
                    if (c == 'P')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (c == 'K')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (c == LastMoveId && c != '.' && c != 'P' && c != 'K')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(c + " ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            while (true)
            {
                Console.WriteLine("Silahkan input file path: ");
                filePath = Console.ReadLine();

                // Validasi ekstensi file
                string extension = Path.GetExtension(filePath).ToLower();
                if (string.IsNullOrEmpty(extension) || extension != ".txt")
                {
                    Console.WriteLine("Error: File harus memiliki ekstensi .txt. Silakan coba lagi.");
                    continue;
                }

                // Periksa apakah file ada
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Error: File tidak ditemukan. Silakan coba lagi.");
                    continue;
                }

                break; // Keluar dari loop jika valid
            }

            Console.WriteLine("Silahkan input algoritma pencarian (1: Greedy, 2: UCS, 3: A*): ");
            int algo = int.Parse(Console.ReadLine());

            var (board, vehicles, exit) = LoadBoard(filePath);
            var initialState = new State(board, vehicles, exit);

            var runs = new List<(string name, Func<(List<State> path, int nodesVisited)> solve)>();
            switch (algo)
            {
                case 1:
                    runs.Add(("Greedy (Heuristic 1)", () => GreedyBestFirstSearch.Solve(initialState, 1)));
                    runs.Add(("Greedy (Heuristic 2)", () => GreedyBestFirstSearch.Solve(initialState, 2)));
                    break;
                case 2:
                    runs.Add(("UCS", () => UniformCostSearch.Solve(initialState)));
                    break;
                case 3:
                    runs.Add(("A* (Heuristic 1)", () => AStar.Solve(initialState, 1)));
                    runs.Add(("A* (Heuristic 2)", () => AStar.Solve(initialState, 2)));
                    break;
                default:
                    Console.WriteLine("Algoritma tidak valid.");
                    return;
            }

            foreach (var (name, solve) in runs)
            {
                Console.WriteLine($"\nRunning {name}:");
                var stopwatch = Stopwatch.StartNew();
                var (path, nodesVisited) = solve();
                stopwatch.Stop();

                if (path == null)
                {
                    Console.WriteLine("Tidak ada solusi.");
                }
                else
                {
                    Console.WriteLine($"Solusi ditemukan dalam {stopwatch.ElapsedMilliseconds} ms.");
                    Console.WriteLine($"Jumlah node yang dikunjungi: {nodesVisited}.");
                    Console.WriteLine($"Solusi ditemukan setelah {path.Count - 1} langkah.");
                    for (int i = 0; i < path.Count; i++)
                    {
                        Console.WriteLine($"Langkah {i + 1}:");
                        path[i].Print();
                    }
                }
            }
        }

        static (char[,] board, List<Vehicle> vehicles, (int Row, int Col) exit) LoadBoard(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            int idx = 0;

            // Melakukan pengecekan File .txt dan konten file
            if (idx >= lines.Length)
            {
                throw new ArgumentException("File kosong.");
            }
            var dims = lines[idx++].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            if (dims.Length != 2)
            {
                throw new ArgumentException("Baris pertama harus berisikan dua integer A B untuk mendefinisikan Baris dan Kolom.");
            }
            int rows = dims[0], cols = dims[1];

            if (rows <= 0 || cols <= 0)
            {
                throw new ArgumentException("Periksa kembali integer, harus positif.");
            }

            if (idx >= lines.Length)
            {
                throw new ArgumentException("File kekurangan jumlah blok non-primary.");
            }
            int n = int.Parse(lines[idx++].Trim());
            if (n < 0)
            {
                throw new ArgumentException("Periksa kembali jumlah blok non-primary, harus positif.");
            }

            var board = new char[rows, cols];
            (int Row, int Col) exit = (-1, -1);
            var vehicles = new List<Vehicle>();

            // Read the board
            for (int i = 0; i < rows; i++, idx++)
            {
                if (idx >= lines.Length)
                {
                    throw new ArgumentException($"Jumlah baris tidak sesuai. Diberikan bahwa terdapat {rows + 2} baris, tetapi hanya menemukan {lines.Length}.");
                }
                var line = lines[idx].Trim();
                if (line.Length < cols)
                {
                    throw new ArgumentException($"Baris {i + 1} memiliki {line.Length} karakter, seharusnya memiliki {cols}. Konten Baris: '{line}'");
                }
                
                int charsToRead = Math.Min(line.Length, cols);
                bool hasExtraK = line.Length == cols + 1 && line[cols] == 'K';
                if (hasExtraK)
                {
                    charsToRead = cols - 1; 
                }
                for (int j = 0; j < cols; j++)
                {
                    if (hasExtraK && j == cols - 1)
                    {
                        board[i, j] = 'K'; 
                        exit = (i, j);
                    }
                    else
                    {
                        char c = (j < charsToRead) ? line[j] : '.';
                        board[i, j] = c;
                        if (c == 'K')
                        {
                            exit = (i, j);
                        }
                    }
                }
            }

            
            if (exit.Row == -1 || exit.Col == -1)
            {
                throw new ArgumentException("Tidak ada exit (K) pada papan di file.");
            }

            
            var processed = new HashSet<(int, int)>();
            var vehicleIds = new HashSet<char>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (processed.Contains((i, j)) || board[i, j] == '.' || board[i, j] == 'K') continue;
                    char id = board[i, j];
                    if (vehicleIds.Contains(id)) continue; 
                    int length = 1;
                    bool isHorizontal;

                    if (j + 1 < cols && board[i, j + 1] == id)
                    {
                        isHorizontal = true;
                        while (j + length < cols && board[i, j + length] == id)
                        {
                            length++;
                        }
                    }
                    else if (i + 1 < rows && board[i + 1, j] == id)
                    {
                        isHorizontal = false;
                        while (i + length < rows && board[i + length, j] == id)
                        {
                            length++;
                        }
                    }
                    else
                    {
                        isHorizontal = true;
                    }

                    vehicles.Add(new Vehicle(id, i, j, length, isHorizontal));
                    vehicleIds.Add(id);
                    for (int k = 0; k < length; k++)
                    {
                        int r = isHorizontal ? i : i + k;
                        int c = isHorizontal ? j + k : j;
                        processed.Add((r, c));
                    }
                }
            }

            
            int nonPrimaryCount = vehicles.Count(v => v.Id != 'P');
            if (nonPrimaryCount != n && Math.Abs(nonPrimaryCount - n) > 1)
            {
                throw new ArgumentException($"Diberikan jumlah non=primary sebanyak {n}, tetapi hanya menemukan sebanyak {nonPrimaryCount}.");
            }
            else if (nonPrimaryCount != n)
            {
                Console.WriteLine($"Peringatan: Diberikan jumlah non=primary sebanyak {n}, tetapi hanya menemukan sebanyak {nonPrimaryCount}. Melanjutkan prosedur.");
            }

            if (!vehicles.Any(v => v.Id == 'P'))
            {
                throw new ArgumentException("Tidak ada primary vehicle (P) pada papan di file.");
            }

            return (board, vehicles, exit);
        }
    }

    public class Node
    {
        public State State { get; }
        public Node Parent { get; }
        public int Cost { get; }
        public int Heuristic { get; }

        public Node(State state, Node parent, int cost, int heuristic)
        {
            State = state;
            Parent = parent;
            Cost = cost;
            Heuristic = heuristic;
        }

        public Node(State state, Node parent, int cost) : this(state, parent, cost, 0)
        {
        }
    }

    public class PriorityQueue<T>
    {
        private List<T> items = new List<T>();
        private readonly Comparison<T> comparison;

        public PriorityQueue(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        public void Enqueue(T item)
        {
            items.Add(item);
            items.Sort(comparison);
        }

        public T Dequeue()
        {
            if (items.Count == 0) throw new InvalidOperationException("Queue kosong.");
            var item = items[0];
            items.RemoveAt(0);
            return item;
        }

        public int Count => items.Count;
    }
}