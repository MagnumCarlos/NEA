
using System.Numerics;

namespace NEA
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int OptionChosen = new Menu("Ghost Hunter", new[]{ "Play", "Exit" }).Show();
            if (OptionChosen == 0)
            {
                int MapChosen = new Menu("Choose a Map", new[] { "Map1", "Map2" }).Show();
                if (MapChosen == 0)
                {
                    Map Map1 = Map.LoadMap("Map1.txt");
                    PlayGame(Map1);
                }
            }
            else if (OptionChosen == 1)
            {
                Console.WriteLine("Thanks for playing dawg");
            }
        }
        static void PlayGame(Map GameMap)
        {
            Player player = SpawnPlayer(GameMap);
            foreach(Room r in GameMap.getRooms())
            {
                if(r.isGhostRoom())
                {
                    Ghost ghost = Ghost.LoadGhost("Ghosts.txt", r);
                    break;
                }
            }
            bool Playing = true;
            Room CurrentRoom = GameMap.getRooms()[0];
            /*Pathfinder pf = new Pathfinder();
            List<Cell> ShortestPath = pf.FindShortestPath(Cells[19, 18], Cells[25,18]);
            foreach(Cell cell in ShortestPath)
            {
                Console.Write(cell.GetXCoord() + " " + cell.GetYCoord());
                Console.WriteLine();
            }*/
            
            while (Playing)
            {
                CurrentRoom = CheckForMovement(GameMap, CurrentRoom, player);
            }
        }
        static Room CheckForMovement(Map GameMap, Room CurrentRoom, Player player)
        {
            (int newX, int newY) = GetPlayerInput(player);
            Console.Clear();
            Room NewRoom = MovePlayer(GameMap, CurrentRoom, player, newX, newY);
            if (NewRoom != null) //room transition has occurred
            {
                CurrentRoom = NewRoom;
            }
            int NewLocalX = newX - CurrentRoom.GetOriginX();
            int NewLocalY = newY - CurrentRoom.GetOriginY();
            CurrentRoom.DisplayRoom(player);
            Console.WriteLine("X:" + newX);
            Console.WriteLine("Y: " + newY);
            Console.WriteLine("LX: " + NewLocalX);
            Console.WriteLine("LY: " + NewLocalY); //for debugging
            return CurrentRoom;
        }
        static Room? MovePlayer(Map GameMap, Room CurrentRoom, Player player, int newX, int newY)
        {
            Cell[,] Cells = CurrentRoom.GetCells();
            int localX = newX - CurrentRoom.GetOriginX();
            int localY = newY - CurrentRoom.GetOriginY();
            int Width = Cells.GetLength(0);
            int Height = Cells.GetLength(1);
            if (localX >= 0 && localX < Width && localY >= 0 && localY < Height)
            {
                Cell nextCell = Cells[localX, localY];
                if(nextCell is DoorCell door) //by default door cells are walkable so a check isn't needed
                {
                    int TargetX = door.getTargetX();
                    int TargetY = door.getTargetY();
                    Room TargetRoom = door.FindTargetRoom(GameMap,CurrentRoom.GetRoomNumber());
                    player.SetPosition(TargetX, TargetY);
                    return TargetRoom;
                }
                else if (nextCell.IsWalkable()) 
                {
                    player.SetPosition(newX, newY);
                }
            }
            return null;
        }
        static Player SpawnPlayer(Map GameMap)
        {
            Room CurrentRoom = GameMap.getRooms()[0];
            int SpawnX = CurrentRoom.GetWidth() / 2;
            int SpawnY = CurrentRoom.GetHeight() / 2;
            if (CurrentRoom.GetCells()[SpawnX,SpawnY].IsWalkable())
            {
                return new Player(SpawnX, SpawnY);
            }
            Console.WriteLine("error while loading player: cell occupied");
            return null;
        }
        static (int, int) GetPlayerInput(Player player)
        {
            ConsoleKeyInfo KeyPressed = Console.ReadKey(true);
            int NewX = player.GetGXCoord();
            int NewY = player.GetGYCoord();

            switch (KeyPressed.Key)
            {
                case ConsoleKey.W:
                    NewY -= 1;
                    break;
                case ConsoleKey.S:
                    NewY += 1;
                    break;
                case ConsoleKey.A:
                    NewX -= 1;
                    break;
                case ConsoleKey.D:
                    NewX += 1;
                    break;
            }

            return (NewX, NewY);
        }
        public static bool CheckForFile(string FileName)
        {
            if (File.Exists(FileName))
            {
                return true;
            }
            else
            {
                Console.WriteLine("An error occured whilst loading a file");
            }
            return false;
        }
    }
    class Menu
    {
        string Heading;
        string[] Options;
        int CurrentOption = 0;
        public Menu(string HEADING, string[] OPTIONS)
        {
            this.Heading = HEADING;
            this.Options = OPTIONS;
        }
        public int Show()
        {
            ConsoleKeyInfo key;
            int SelectedOption = -1;

            while (SelectedOption == -1)
            {
                Console.Clear();
                Console.WriteLine($" {Heading}\n");

                for (int i = 0; i < Options.Length; i++)
                {
                    if (i == CurrentOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("> ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.WriteLine(Options[i]);
                }

                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (CurrentOption > 0)
                        {
                            CurrentOption--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (CurrentOption < Options.Length - 1)
                        {
                            CurrentOption++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        SelectedOption = CurrentOption;
                        break;
                }
            }

            return SelectedOption;
        }
    }
}


