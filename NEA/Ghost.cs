namespace NEA
{
    public class Ghost
    {
        string Name;
        int GXCoord, GYCoord;
        List<Evidence> Evidences;
        public Ghost(int GXCOORD, int GYCOORD, string NAME, List<Evidence> EVIDENCES)
        {
            this.GXCoord = GXCOORD;
            this.GYCoord = GYCOORD;
            this.Name = NAME;
            this.Evidences = EVIDENCES;
        }
        public static Ghost LoadGhost(string FileName, Room GhostRoom)
        {
            if (Program.CheckForFile(FileName))
            {
                string[] Lines = File.ReadAllLines(FileName);
                Random rng = new Random();
                int Random = rng.Next(Lines.Length);
                string line = Lines[Random];

                string[] Parts = line.Split();
                string GhostType = Parts[0];
                List<Evidence> Evidences = new List<Evidence>();
            
                (int SpawnX, int SpawnY) = SpawnGhost(GhostRoom);
                return new Ghost(SpawnX, SpawnY, GhostType, Evidences);
            }
            Console.WriteLine("error occurred during ghost loading");
            return null;
        }
        public static (int,int) SpawnGhost(Room GhostRoom)
        {
            int SpawnX = (GhostRoom.GetWidth() / 2) + GhostRoom.GetOriginX();
            int SpawnY = (GhostRoom.GetHeight() / 2) + GhostRoom.GetOriginY();
            Cell[,] cells = GhostRoom.GetCells();
            if (cells[SpawnX,SpawnY].IsWalkable())
            {
                return (SpawnX, SpawnY);
            }
            else
            {
                for(int i =0; i < GhostRoom.GetWidth();i++)
                {
                    for(int j =0; j < GhostRoom.GetHeight(); j++)
                    {
                        if(cells[i,j].IsWalkable())
                        {
                            return(i + GhostRoom.GetOriginX(), j + GhostRoom.GetOriginY());
                        }
                    }
                }
            }
            return (-1, -1); 
        }
    }
}
