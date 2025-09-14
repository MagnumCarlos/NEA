
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
                int MapChosen = new Menu("Choose a Map", new[] { "Map1", "Small Bungalow" }).Show();
                if (MapChosen == 0)
                {
                    Map Map1 = Map.LoadMap("Map1.txt");
                    PlayGame(Map1);
                }
                else if(MapChosen ==1)
                {
                    Map Map2 = Map.LoadMap("Bungalow.txt");
                    PlayGame(Map2);
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
            foreach(Room r in GameMap.GetRooms())
            {
                if(r.isGhostRoom())
                {
                    ghost = Ghost.LoadGhost("Ghosts.txt", r);
                    break;
                }
            }
            bool Playing = true;
            Room CurrentRoom = GameMap.GetRooms()[0];
            while (Playing)
            {
                CurrentRoom = Player.CheckForMovement(GameMap, CurrentRoom, player);
                Cell[,] Cells = CurrentRoom.GetCells();
<<<<<<< HEAD
                if(ghost.GetGXCoord() > CurrentRoom.GetOriginX() && ghost.GetGXCoord() < CurrentRoom.GetOriginX() + CurrentRoom.GetWidth())
=======
                if(ghost.GetGXCoord() > CurrentRoom.GetOriginX() && ghost.GetGXCoord() < CurrentRoom.GetOriginX()+CurrentRoom.GetWidth())
>>>>>>> 78bae7f91f5cb4eabcd96620156f8abbc130e5e3
                {
                    List<Cell> pf = Pathfinder.FindShortestPath(Cells[player.GetGXCoord() - CurrentRoom.GetOriginX(), player.GetGYCoord() - CurrentRoom.GetOriginY()], Cells[ghost.GetGXCoord() - CurrentRoom.GetOriginX(), ghost.GetGYCoord() - CurrentRoom.GetOriginY()]);
                    player.Flashlight.Illuminate(player, CurrentRoom);
                    CurrentRoom.DisplayRoom(player, ghost, pf);
                }
                else
                {
                    player.Flashlight.Illuminate(player, CurrentRoom);
<<<<<<< HEAD
                    CurrentRoom.DisplayRoom(player);
                }
=======
                    CurrentRoom.DisplayRoom(player, ghost);
                }
                CurrentRoom.ClearLighting();
>>>>>>> 78bae7f91f5cb4eabcd96620156f8abbc130e5e3
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


