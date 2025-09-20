namespace NEA
{
    class Pathfinder
    {
        public static List<Cell> FindShortestPath(Cell StartCell, Cell EndCell)
        {
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            Dictionary<Cell, Node> AllNodes = new Dictionary<Cell, Node>(); 
            Node Start = new Node(StartCell, 0);
            Node End = new Node(EndCell, 0);
            Start.SetHCost(CalculateH(Start, End)); //Estimated cost to the end
            OpenList.Add(Start);
            AllNodes[StartCell] = Start; 
            while (OpenList.Count!=0)
            {
                Node Current = GetLowestFCost(OpenList); //if F costs are equal, this will prefer lower H costs

                if(Current.GetRefCell().GetXCoord() == End.GetRefCell().GetXCoord() && Current.GetRefCell().GetYCoord() == End.GetRefCell().GetYCoord())
                {
                    List<Cell> ShortestPath = ReconstructPath(Current);
                    return ShortestPath;
                }
                OpenList.Remove(Current);
                ClosedList.Add(Current);
                List<Cell> Neighbours = GetNeighbours(Current);
                foreach (Cell neighbourCell in Neighbours)
                {
                    bool IsInClosedList = false;
                    foreach(Node n in ClosedList)
                    {
                        if(n.GetRefCell() == neighbourCell)
                        {
                            IsInClosedList=true;
                            break; 
                        }
                    }
                    if (IsInClosedList) 
                    {
                        continue; 
                    }
                    Node neighbourNode;
                    if (AllNodes.ContainsKey(neighbourCell))
                    {
                        neighbourNode = AllNodes[neighbourCell];
                    }
                    else
                    {
                        neighbourNode = new Node(neighbourCell,int.MaxValue); 
                        AllNodes[neighbourCell] = neighbourNode;
                    }
                    int tentativeG = Current.GetGCost() + 1;
                    if (tentativeG < neighbourNode.GetGCost())
                    {
                        neighbourNode.SetGCost(tentativeG);
                        neighbourNode.SetHCost(CalculateH(neighbourNode, End));
                        neighbourNode.SetParent(Current);

                        if (!OpenList.Contains(neighbourNode))
                        {
                            OpenList.Add(neighbourNode);
                        }
                    }
                }
            }
            Console.WriteLine("ERROR NO PATH FOUND");
            return new List<Cell>();
        }
     
        private static Node GetLowestFCost(List<Node> OpenList)
        {
            Node Lowest = OpenList[0];
            for(int i =1; i < OpenList.Count;i++)
            {
                if (OpenList[i].GetFCost() < Lowest.GetFCost() || OpenList[i].GetFCost() == Lowest.GetFCost() && OpenList[i].GetHCost() < Lowest.GetHCost())
                { 
                    Lowest = OpenList[i];
                }
            }
            return Lowest;
        }
        private static List<Cell> GetNeighbours(Node Current)
        {
            List<Cell> Neighbours = new List<Cell>();
            Cell RefCell = Current.GetRefCell();
            int X = RefCell.GetXCoord();
            int Y = RefCell.GetYCoord();
            Cell[,] Cells = RefCell.GetRoomRef().GetCells();
            int Width = Cells.GetLength(0);
            int Height = Cells.GetLength(1);
            List<(int dx, int dy)> Directions = new List<(int, int)>
    {
        (0,-1), // up
        (0,1),  // down
        (-1,0), // left
        (1,0)   // right
    };

            // shuffle
            Random rng = new Random();
            Directions = Directions.OrderBy(_ => rng.Next()).ToList();

            foreach (var dir in Directions)
            {
                int nX = X + dir.dx;
                int nY = Y + dir.dy;
                if (nX >= 0 && nX < Width && nY >= 0 && nY < Height)
                {
                    Cell neighbour = Cells[nX, nY];
                    if (neighbour.IsWalkable())
                    {
                        Neighbours.Add(neighbour);
                    }
                }
            }
            return Neighbours;
        }

        private static int CalculateH(Node Current, Node End) //Manhattan distance method since my grid only allows for 4 directional movement
        {
            int x1 = Current.GetRefCell().GetGXCoord();
            int x2 = End.GetRefCell().GetGXCoord();
            int y1 = Current.GetRefCell().GetGYCoord();
            int y2 = End.GetRefCell().GetGYCoord();
            int Distance = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            return Distance;
        }
        private static List<Cell> ReconstructPath(Node EndNode)
        {
            List<Cell> Path = new List<Cell>();
            Node Current = EndNode;
            while(Current!=null) //loop until start node reached which has null as its parent
            {
                Path.Add(Current.GetRefCell());
                Current = Current.GetParent();
            }
            Path.Reverse();
            return Path;
        }
    }
    class Node
    {
        Cell RefCell;
        int GCost, FCost, HCost;
        Node Parent;
        public Node(Cell REFCELL, int GCOST = 0, int HCOST = 0, Node PARENT = null)
        {
            this.RefCell = REFCELL;
            this.GCost = GCOST;
            this.HCost = HCOST;
            this.FCost = GCost + HCost;
            this.Parent = PARENT;
        }
        public Node GetParent()
        {
            return Parent;
        }
        public Cell GetRefCell()
        {
            return RefCell;
        }
        public int GetFCost()
        {
            return FCost;
        }
        public int GetHCost()
        {
            return HCost;
        }
        public int GetGCost()
        {
            return GCost;
        }
        public void SetHCost(int NewHCost)
        {
            FCost -= HCost;
            this.HCost = NewHCost;
            FCost += HCost;
        }
        public void SetGCost(int NewGCost)
        {
            FCost -= GCost;
            this.GCost = NewGCost;
            FCost += NewGCost;
        }
        public void SetParent(Node New)
        {
            this.Parent = New;
        }
    }
    class RoomGraph
    {
        RoomGraphEdge[,] AdjacencyMatrix; //Dense graph; there are much more doors (edges) than rooms (vertices)
        public RoomGraph(int NumberOfRooms)
        {
            AdjacencyMatrix = new RoomGraphEdge[NumberOfRooms,NumberOfRooms];
        }   
        public void PopulateMatrix(Map GameMap)
        {
            List<DoorCell> AllDoors = GameMap.GetAllDoors();
            foreach(DoorCell EntryDoorA in AllDoors)
            {
                Room RoomA = EntryDoorA.GetRoomRef();
                int i = RoomA.GetRoomNumber();
                Room RoomB = EntryDoorA.FindTargetRoom(GameMap,RoomA.GetRoomNumber());
                int j = RoomB.GetRoomNumber();
                DoorCell EntryDoorB = (DoorCell) RoomB.GetCells()[EntryDoorA.GetTargetX() - RoomB.GetOriginX(), EntryDoorA.GetTargetY() - RoomB.GetOriginY()];
                AdjacencyMatrix[i, j] = new RoomGraphEdge(EntryDoorA, EntryDoorB, 1);/*Each edge represents a door connection, and the cost is the same for each edge
                Therefore the pathfinding will prefer paths with the least number of doors which is accurate enough, since a) there aren't hundreds of Rooms unlike Cells 
                b) most rooms are square shaped so it's representative*/
                AdjacencyMatrix[j,i] = new RoomGraphEdge(EntryDoorB,EntryDoorA,1);    
                //Every door is bidirectional which is why both [i,j] and [j,i] are populated for one Door
            }
        }
        public List<Room> FindRoomPath(Room StartRoom, Room EndRoom, Room[] AllRooms) //Breadth First Search, since all edges are of the same weight in the graph
        {
            int StartIndex = StartRoom.GetRoomNumber(); //Same as Array.IndexOf - i added a new RoomNumber property to each room
            int EndIndex = EndRoom.GetRoomNumber(); 
            Queue<int> queue = new Queue<int>();
            bool[] Visited = new bool[AllRooms.Length];
            int[] Parent = new int[AllRooms.Length];
            for(int i =0; i < Parent.Length; i++)
            {
                Parent[i] = -1;
            } //By default every element is 0, but 0 is actually a valid RoomNumber so I use -1 to represent "no parent"
            Visited[StartIndex] = true;
            queue.Enqueue(StartIndex);
            while(queue.Count !=0)
            {
                int Current =queue.Dequeue();
                if(Current == EndIndex)
                {
                    break;
                }
                for(int i =0; i < AllRooms.Length;i++)
                {
                    if (AdjacencyMatrix[Current,i] !=null && !Visited[i])
                    {
                        Visited[i] = true;
                        Parent[i] = Current;
                        queue.Enqueue(i);
                    }
                }
            }
            List<Room> Path = new List<Room>();
            for(int i = EndIndex; i != -1; i = Parent[i])
            {
                Path.Add(AllRooms[i]);
            }
            Path.Reverse();
            return Path;
        }
    }
    class RoomGraphEdge
    {
        DoorCell EntryDoor, ExitDoor;
        int Cost;
        public RoomGraphEdge(DoorCell ENTRYDOOR, DoorCell EXITDOOR, int COST)
        {
            EntryDoor = ENTRYDOOR;
            ExitDoor = EXITDOOR;
            Cost = COST;
        }   
    }
}
