
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
        private static void PlayGame(Map GameMap)
        {
            Player player = Player.SpawnPlayer(GameMap);
            Ghost ghost = null;
            foreach(Room r in GameMap.getRooms())
            {
                if(r.isGhostRoom())
                {
                    ghost = Ghost.LoadGhost("Ghosts.txt", r);
                    break;
                }
            }
            bool Playing = true;
            Room CurrentRoom = GameMap.getRooms()[0];
            Pathfinder pf = new Pathfinder();
            while (Playing)
            {
                CurrentRoom = Player.CheckForMovement(GameMap, CurrentRoom, player);
                Cell[,] Cells = CurrentRoom.GetCells();
                if(ghost.GetGXCoord() < CurrentRoom.GetOriginX() + CurrentRoom.GetWidth())
                {
                    int PlayerLocalX = player.GetGXCoord() - CurrentRoom.GetOriginX();
                    int PlayerLocalY = player.GetGYCoord() - CurrentRoom.GetOriginY();
                    int GhostLocaLX = ghost.GetGXCoord() - CurrentRoom.GetOriginX();
                    int GhostLocaLY = ghost.GetGYCoord() - CurrentRoom.GetOriginY();
                    List<Cell> SP = pf.FindShortestPath(Cells[PlayerLocalX, PlayerLocalY], Cells[GhostLocaLX,GhostLocaLY]);
                    CurrentRoom.DisplayRoom(player, ghost,SP);
                }
                
            }
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


