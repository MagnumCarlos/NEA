
namespace NEA
{
    using System;
    public class Map
    {
        Room[] Rooms;
        int RoomsPerRow;
        List<DoorCell> AllDoors;
        public Map(Room[] ROOMS, int ROOMSPERROW)
        {
            this.Rooms = ROOMS;
            this.RoomsPerRow = ROOMSPERROW;
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
                    List<DoorCell> AllDoors = new List<DoorCell>();
                    for (int i = 0; i < NumberOfRooms; i++)
                    {
                        Room NewRoom = Room.LoadRoom(Reader, i, RunningTotalWidth, RunningTotalHeight, RoomsPerRow, ref StartPos);
                        foreach(DoorCell door in NewRoom.GetDoors())
                        {
                            AllDoors.Add(door);
                        }
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
                    NewMap.AllDoors = AllDoors;
                    return NewMap;
                }
            }
            return null;
        }
        public Room[] GetRooms()
        {
            return Rooms;
        }
        public List<DoorCell> GetAllDoors()
        {
            return AllDoors;
        }
    }
}
