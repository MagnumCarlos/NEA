using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public class Player
    {
        int GXCoord, GYCoord;
        List<Item> Inventory;
        public Player(int GXCOORD, int GYCOORD,  List<Item> INVENTORY = null)
        {
            this.GXCoord = GXCOORD;
            this.GYCoord = GYCOORD;
            Inventory = INVENTORY ?? new List<Item>(); //the ?? means that if INVENTORY is null (by default it is) then make a new empty list
        }
        public int GetGXCoord()
        {
            return GXCoord;
        }
        public int GetGYCoord()
        {
            return GYCoord;
        }
        public void SetPosition(int NewX, int NewY)
        {
            this.GXCoord = NewX;
            this.GYCoord = NewY;
        }
        public static Player SpawnPlayer(Map GameMap)
        {
            Room CurrentRoom = GameMap.getRooms()[0];
            int SpawnX = CurrentRoom.GetWidth() / 2;
            int SpawnY = CurrentRoom.GetHeight() / 2;
            if (CurrentRoom.GetCells()[SpawnX, SpawnY].IsWalkable())
            {
                return new Player(SpawnX, SpawnY);
            }
            Console.WriteLine("error while loading player: cell occupied");
            return null;
        }
        private static Room MovePlayer(Map GameMap, Room CurrentRoom, Player player, int newX, int newY)
        {
            Cell[,] Cells = CurrentRoom.GetCells();
            int localX = newX - CurrentRoom.GetOriginX();
            int localY = newY - CurrentRoom.GetOriginY();
            int Width = Cells.GetLength(0);
            int Height = Cells.GetLength(1);
            if (localX >= 0 && localX < Width && localY >= 0 && localY < Height)
            {
                Cell nextCell = Cells[localX, localY];

                if (nextCell is DoorCell door)
                {
                    int TargetX = door.getTargetX();
                    int TargetY = door.getTargetY();
                    Room TargetRoom = door.FindTargetRoom(GameMap, CurrentRoom.GetRoomNumber());
                    player.SetPosition(TargetX, TargetY);
                    return TargetRoom;
                }
                else if (nextCell.IsWalkable()) 
                {
                    player.SetPosition(newX, newY);
                }
            }
            return CurrentRoom;
        }

        public static Room CheckForMovement(Map GameMap, Room CurrentRoom, Player player)
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
            Console.WriteLine("X:" + newX);
            Console.WriteLine("Y: " + newY);
            Console.WriteLine("LX: " + NewLocalX);
            Console.WriteLine("LY: " + NewLocalY); //for debugging
            return CurrentRoom;
        }
        private static (int, int) GetPlayerInput(Player player)
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
    }
}
