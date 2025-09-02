namespace NEA
{
    public class Cell
    {
        int XCoord, YCoord; //local ones
        int GXCoord, GYCoord; //global
        char Symbol;
        Room RoomRef;
        public Cell(int XCOORD, int YCOORD, char SYMBOL, int GLOBALX, int GLOBALY)
        {
            this.XCoord = XCOORD;
            this.YCoord = YCOORD;
            this.Symbol = SYMBOL;
            this.GXCoord = GLOBALX;
            this.GYCoord = GLOBALY;
        }
        public char GetSymbol()
        {
            return Symbol;
        }
        public int GetXCoord()
        {
            return XCoord;
        }
        public int GetYCoord()
        {
            return YCoord;
        }
        public int GetGXCoord()
        {
            return GXCoord;
        }
        public int GetGYCoord()
        {
            return GYCoord;
        }
        public Room GetRoomRef()
        {
            return RoomRef;
        }
        public void SetRoomRef(Room room)
        {
            this.RoomRef = room;
        }
        public virtual bool IsWalkable()
        {
            return char.IsWhiteSpace(Symbol) || Symbol == '■';
        }
    }
    class DoorCell : Cell
    {
        public int TargetGlobalX, TargetGlobalY;
        public DoorCell(int LOCALX, int LOCALY, char SYMBOL, int GLOBALX, int GLOBALY, int TARGETGLOBALX, int TARGETGLOBALY) : base(LOCALX, LOCALY, SYMBOL, GLOBALX, GLOBALY)
        {
            this.TargetGlobalX = TARGETGLOBALX;
            this.TargetGlobalY = TARGETGLOBALY;
        }
        public int getTargetX()
        {
            return this.TargetGlobalX;
        }
        public int getTargetY()
        {
            return this.TargetGlobalY;
        }
        public static (int, int) LoadTarget(string FileName, int StartPos)
        {
            if (Program.CheckForFile(FileName))
            {
                string CurrentLine;
                int CurrentLineNo = 0;
                StreamReader Reader = new StreamReader(FileName);
                using (Reader)
                {
                    while ((CurrentLine = Reader.ReadLine()) != null)
                    {
                        if (CurrentLineNo == StartPos)
                        {
                            int TargetX = Convert.ToInt32(CurrentLine);
                            int TargetY = Convert.ToInt32(Reader.ReadLine());
                            return (TargetX, TargetY);
                        }
                        CurrentLineNo++; //reads lines until start position found
                    }
                }
            }
            return (-1, -1); //error values
        }
        public Room FindTargetRoom(Map GameMap, int CurrentRoomNo)
        {
            Room[] Rooms = GameMap.getRooms();
            int NoOfRooms = Rooms.Count();
            for (int i = 0; i < NoOfRooms; i++)
            {
                Room room = Rooms[i];
                int OriginX = room.GetOriginX();
                int OriginY = room.GetOriginY();
               
                if(i!=CurrentRoomNo) //so this method doesn't just return the room the player is currently in
                {
                    if (TargetGlobalX >= OriginX && TargetGlobalX < OriginX + room.GetWidth() && TargetGlobalY >= OriginY && TargetGlobalY < OriginY + room.GetHeight())
                    {
                        return room;
                    }
                }
            }
            return null;
        }
        public override bool IsWalkable()
        {
            return true;
        }
    }
    class PropCell : Cell
    {
        Prop PropReference;
        public PropCell(int XCOORD, int YCOORD, char SYMBOL, int GLOBALX, int GLOBALY, Prop PROP): base(XCOORD, YCOORD, SYMBOL, GLOBALX, GLOBALY)
        {
            this.PropReference = PROP;
        }
        public override bool IsWalkable()
        {
            return false; // Props typically block movement
        }
        public Prop GetProp()
        {
            return PropReference;
        }
    }
}
