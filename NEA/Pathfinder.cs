namespace NEA
{
    class Pathfinder
    {
        public List<Cell> FindShortestPath(Cell StartCell, Cell EndCell)
        {
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            Dictionary<Cell, Node> AllNodes = new Dictionary<Cell, Node>();
            Node Start = new Node(StartCell, 0);
            Node End = new Node(EndCell, 0);
            Start.SetHCost(CalculateH(Start, End));
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
        private Node GetLowestFCost(List<Node> OpenList)
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
        private List<Cell> GetNeighbours(Node Current)
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
        private List<Cell> ReconstructPath(Node EndNode)
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
}
