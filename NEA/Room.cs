using System.IO;
using System.Runtime.CompilerServices;
namespace NEA
{
    public class Room : IRectangle
    {
        Cell[,] Cells;
        bool IsGhostRoom;
        int RoomNumber;
        int OriginX, OriginY;
        int GridX, GridY; //position of room in the global grid, used for pathfinding later on
        List<Prop> PropsInRoom = new List<Prop>();
        public Room(int ROOMNUMBER, Cell[,] CELLS, int ORIGINX, int ORIGINY, int GRIDX, int GRIDY)
        {
            this.RoomNumber = ROOMNUMBER;
            this.Cells = CELLS;
            this.OriginX = ORIGINX;
            this.OriginY = ORIGINY;
            this.GridX = GRIDX;
            this.GridY = GRIDY;
        }
        public void ClearLighting()
        {
            for(int i =0; i < GetWidth();i++)
            {
                for(int j =0; j < GetHeight();j++)
                {
                    Cells[i, j].SetLightState(false);
                }
            }
        }
        public void AddProp(Prop p)
        {
            PropsInRoom.Add(p);
        }
        public List<Prop> GetProps()
        {
            return PropsInRoom;
        }
        public int GetWidth()
        {
            return Cells.GetLength(0);
        }
        public int GetHeight()
        {
            return Cells.GetLength(1);
        }
        public int GetOriginX()
        {
            return OriginX;
        }
        public int GetOriginY()
        {
            return OriginY;
        }
        public Cell[,] GetCells()
        {
            return Cells;
        }
        public int GetRoomNumber()
        {
            return RoomNumber;
        }
        public void SetGhostRoom()
        {
            this.IsGhostRoom = true;
        }
        public bool isGhostRoom()
        {
            return IsGhostRoom;
        }
        public static Room LoadRoom(StreamReader Reader, int RoomNumber, int OriginX, int OriginY, int RoomsPerRow, ref int StartPos)
        {
            int MaxWidth = Convert.ToInt32(Reader.ReadLine());
            int RoomHeight = Convert.ToInt32(Reader.ReadLine());
            Cell[,] Cells = new Cell[MaxWidth, RoomHeight];
            for (int y = 0; y < RoomHeight; y++)
            {
                string Line = Reader.ReadLine();
                Line = Line.PadRight(MaxWidth); //this makes sure all lines are of the same length by padding shorter lines with spaces
                for (int x = 0; x < MaxWidth; x++)
                {
                    if (Line[x].Equals('■'))
                    {
                        (int TargetX, int TargetY) = DoorCell.LoadTarget("Bungalow_Doors.txt", StartPos); //StartPos indicates where in doors.txt to read
                        DoorCell doorCell = new DoorCell(x, y, Line[x], x + OriginX, y + OriginY, TargetX, TargetY);
                        Cells[x, y] = doorCell;
                        StartPos += 2;
                    }
                    else
                    {
                        Cells[x, y] = new Cell(x, y, Line[x], x + OriginX, y + OriginY);
                    }
                }
            }
            int GridX = RoomNumber % RoomsPerRow;
            int GridY = RoomNumber / RoomsPerRow;
            Room NewRoom = new Room(RoomNumber, Cells, OriginX, OriginY, GridX, GridY);
            NewRoom.LoadProps("Props.txt");
            /*foreach(Prop P in NewRoom.GetProps())
            {
                if(P is RectangularProp Rect)
                {
                    int X = Rect.GetOriginX();
                    int Y = Rect.GetOriginY(); 
                    int Width = Rect.GetWidth();
                    int Height = Rect.GetHeight();
                    for (int j = X; j < X + Width; j++)      
                    {
                        for (int i = Y; i < Y + Height; i++)  
                        {
                            Cells[j, i] = new PropCell(j, i, '□', j + OriginX, i + OriginY, Rect);
                        }
                    }
                }
                else if (P is CircularProp Circ)
                {
                    int X = Circ.GetCentreX();
                    int Y = Circ.GetCentreY();
                    int R = Circ.GetRadius();
                    for (int j = X - R; j <= X + R; j++) 
                    {
                        for (int i = Y - R; i <= Y + R; i++) // Y - R is the y coordinate of the point furthest to the TOP of the circle, and Y + R the bottom
                        {
                            if (j >= 0 && j < Cells.GetLength(0) && i >= 0 && i < Cells.GetLength(1))
                            {
                                double dist = Math.Sqrt(Math.Pow(j - X, 2) + Math.Pow(i - Y, 2));
                                if (dist <= R)
                                {
                                    Cells[j, i] = new PropCell(j, i, '○', j + OriginX, i + OriginY, Circ); 
                                }
                            }
                        }
                    }
                }
            }*/
            NewRoom.AssignRoomToCells(Cells);
            return NewRoom;
        }
        private void AssignRoomToCells(Cell[,] Cells)
        {
            for(int i =0; i < Cells.GetLength(0); i++)
            {
                for(int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i,j].SetRoomRef(this);
                }
            }
        }
        private void LoadProps(string FileName)
        {
            if (Program.CheckForFile(FileName))
            {
                string[] Lines = File.ReadAllLines(FileName);
                foreach (string line in Lines)//example format of a line: 0 RECT 5 5 3 2 RoomNumber Type X Y Width Height
                {
                    string[] Parts = line.Split();
                    int RoomNumberRead = int.Parse(Parts[0]);
                    if(RoomNumberRead == this.GetRoomNumber())
                    {
                        string Type = Parts[1];
                        int x = int.Parse(Parts[2]);
                        int y = int.Parse(Parts[3]);
                        if (Type == "RECT")
                        {
                            int Width = int.Parse(Parts[4]);
                            int Height = int.Parse(Parts[5]);
                            RectangularProp Rectangle = new RectangularProp(x, y, Width, Height);
                            this.AddProp(Rectangle);
                        }
                        else if (Type == "CIRC") //for circles, x and y represent the centre rather than the top left corner
                        {
                            int Radius = int.Parse(Parts[4]);
                            CircularProp Circle = new CircularProp(x, y, Radius);
                            this.AddProp(Circle);
                        }
                    }
                }
            }
        }
        public void DisplayRoom(Player player, Ghost ghost = null, List<Cell> path = null)
        {
            int LocalX = player.GetGXCoord() - this.OriginX;
            int LocalY = player.GetGYCoord() - this.OriginY;
            int GhostLocalX = 0;
            int GhostLocalY = 0;
            if(ghost!= null)
            {
                GhostLocalX = ghost.GetGXCoord() - OriginX;
                GhostLocalY = ghost.GetGYCoord() - OriginY;
            }
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    if (player != null && x == LocalX && y == LocalY)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("P");
                        Console.ResetColor();
                    }
                    if (Cells[x, y].GetLightState() == false)
                    {
                        Console.Write(".");
                        continue;
                    }
                    else if(ghost != null && x == GhostLocalX && y == GhostLocalY)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("G");
                        Console.ResetColor();
                    }
                    else if (path != null && path.Any(c => c.GetXCoord() == x && c.GetYCoord() == y))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("*");
                        Console.ResetColor();
                    }
                    else 
                    {
                        Console.Write(Cells[x, y].GetSymbol());
                    }

                }
                Console.WriteLine();
            }
        }
        
    }
}
