
namespace NEA
{
    using System;
    public class Map
    {
        Room[] Rooms;
        int RoomsPerRow;
        public Map(Room[] ROOMS, int ROOMSPERROW)
        {
            this.Rooms = ROOMS;
            this.RoomsPerRow = ROOMSPERROW;
        }
        public int getRoomsPerRow()
        {
            return RoomsPerRow;
        }
        public static Map LoadMap(string FileName)
        {
            if(Program.CheckForFile(FileName))
            {
                StreamReader Reader = new StreamReader(FileName);
                using (Reader)
                {
                    int RoomsPerRow = Convert.ToInt32(Reader.ReadLine()); //in the global coordinate system, the rooms are laid out in a grid pattern
                    int NumberOfRooms = Convert.ToInt32(Reader.ReadLine());
                    Room[] Rooms = new Room[NumberOfRooms];
                    Random rng = new Random();
                    int GhostRoomNo = rng.Next(NumberOfRooms);
                    int RunningTotalWidth = 0;
                    int RunningTotalHeight = 0; //these will be used to assign the origins of each room
                    int StartPos = 0; //for loading doors
                    List<int> Heights = new List<int>();    
                    for (int i = 0; i < NumberOfRooms; i++)
                    {
                        Room NewRoom = Room.LoadRoom(Reader, i, RunningTotalWidth, RunningTotalHeight, RoomsPerRow, ref StartPos);
                        if(i == GhostRoomNo)
                        {
                            NewRoom.SetGhostRoom();
                        }
                        Rooms[i] = NewRoom;
                        Heights.Add(NewRoom.GetHeight());
                        if (i % RoomsPerRow ==0 && i >0) 
                        {
                            RunningTotalWidth = 0; 
                            int HighestRoom = Heights.Max();
                            RunningTotalHeight += HighestRoom;
                            Heights.Clear();
                        }
                        else
                        {
                            RunningTotalWidth += NewRoom.GetWidth();
                        }
                    }
                    Map NewMap = new Map(Rooms, RoomsPerRow);
                    return NewMap;
                }
            }
            return null;
        }
        public Room[] getRooms()
        {
            return Rooms;
        }

    }
}
