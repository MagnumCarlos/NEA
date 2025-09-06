using System.Collections.Generic;

namespace NEA
{
    public interface IRectangle
    {
        int GetOriginX();
        int GetOriginY();
        int GetWidth();
        int GetHeight();
    }
    public enum SpawnPattern
    {
        Point,
        MultiPoint,
        Area,
        Dynamic
    }
    public enum DetectionMethod
    {
        Direct,
        Instrumental //requires a specific device 
    }
    public class Evidence
    {
        string Name;
        public SpawnPattern SpawnType;
        public DetectionMethod DetectionType;
        public List<(int, int)> DetectionCoords;
        public Evidence(string NAME, SpawnPattern SPAWNTYPE, DetectionMethod DETECTIONTYPE)
        {
            Name = NAME;
            SpawnType = SPAWNTYPE;
            DetectionType = DETECTIONTYPE;
            DetectionCoords = new List<(int,int)>();
        }
        public string GetName()
        {
            return Name;
        }
        public void Spawn(Room room)
        {
            List<(int, int)> Coords = new List<(int, int)>();
            switch (SpawnType)
            {
                case SpawnPattern.Point:
                    
                    break;
                case SpawnPattern.MultiPoint:
                    if(Name == "Ghost Orbs")
                    {
                        Coords = MultiPointSpawn(room, 5);
                    }
                    else if(Name == "Fingerprints")
                    {
                        Coords = MultiPointSpawn(room.GetProps(),5);    
                    }
                    break;
                /*case SpawnPattern.Area:
                    
                    break;
                case SpawnPattern.Dynamic:
                    
                    break;*/
            }
        }
        private static List<Evidence> GetEvidences()
        {
            return new List<Evidence>
            {
                new Evidence("EMF Level 5", SpawnPattern.Dynamic, DetectionMethod.Instrumental),
                new Evidence("Ghost Orbs", SpawnPattern.MultiPoint, DetectionMethod.Instrumental),
                new Evidence("Freezing Temps", SpawnPattern.Area, DetectionMethod.Direct),
                new Evidence("Spirit Box", SpawnPattern.Dynamic, DetectionMethod.Instrumental),
                new Evidence("Fingerprints", SpawnPattern.MultiPoint, DetectionMethod.Direct),
                //new Evidence("Ghost Writing", SpawnPattern.Point, DetectionMethod.Direct),
                //new Evidence("Motion Sensor", SpawnPattern.Point, DetectionMethod.Instrumental)
            };
        }
        public static List<Evidence> SelectEvidences(List<string> TargetNames)
        {
            return GetEvidences().Where(e => TargetNames.Contains(e.GetName())).ToList();
            //REALLY shorthand version. It gets all the evidences, and for each evidence (e), if its name matches something in TargetName,
            //the evidence is added to a list to be returned.
        }
        private List<(int, int)> RectangleSpawn(IRectangle Rect, int DetectionModifier = 0)
        {
            int StartX = Rect.GetOriginX() - DetectionModifier;
            int StartY = Rect.GetOriginY() - DetectionModifier;

            List<(int, int)> Coords = new List<(int, int)>();
            for (int i = 0; i < Rect.GetWidth() + (DetectionModifier * 2); i++)
            {
                for (int j = 0; j < Rect.GetHeight() + (DetectionModifier * 2); j++)
                {
                    Coords.Add((StartX + i, StartY + j));
                }
            }
            return Coords;
        }
        private List<(int, int)> MultiPointSpawn(Room room, int Frequency) 
        {
            Random rng = new Random();
            Cell[,] Cells = room.GetCells();
            int SpawnX, SpawnY;
            List<(int,int)> SpawnCoords = new List<(int,int)>();
            for(int i =0; i < Cells.Length /Frequency;i++) //spawns a ghost orb for every x cells
            {
                do
                {
                    SpawnX = rng.Next(1, room.GetWidth());
                    SpawnY = rng.Next(1, room.GetHeight());
                }
                while (!Cells[SpawnX, SpawnY].IsWalkable() || SpawnCoords.Contains((SpawnX,SpawnY)));
                SpawnCoords.Add((SpawnX,SpawnY));
            }
            return SpawnCoords;
        }
        private List<(int, int)> MultiPointSpawn(List<Prop>Props, int PrintsPerProp) //fingerprints
        {
            Random rng = new Random();
            List<(int, int)> SpawnCoords = new List<(int, int)>();
            for (int i = 0; i < Props.Count; i++)
            {
                for (int j = 0; j < PrintsPerProp; j++)
                {
                    if (Props[i] is RectangularProp Rect)
                    {
                        int SpawnX = rng.Next(Rect.GetOriginX(), Rect.GetOriginX() + Rect.GetWidth()+1);
                        int SpawnY = rng.Next(Rect.GetOriginY(), Rect.GetOriginY() + Rect.GetHeight()+1);
                        SpawnCoords.Add((SpawnX, SpawnY));
                    }
                    else if (Props[i] is CircularProp Circ)
                    {
                        bool InCircle = false;
                        int Radius = Circ.GetRadius();
                        int CentreX = Circ.GetCentreX();
                        int CentreY = Circ.GetCentreY();
                        while (!InCircle)
                        {
                            int SpawnX = rng.Next(CentreX - Radius, CentreX + Radius+1);
                            int SpawnY = rng.Next(CentreY - Radius, CentreY + Radius +1);
                            if ((SpawnX - CentreX) * (SpawnX - CentreX) +(SpawnY - CentreY) * (SpawnY - CentreY) <= Radius * Radius)
                            {
                                SpawnCoords.Add((SpawnX, SpawnY));
                                InCircle = true;
                            }
                        }
                        
                    }
                }
            }
            return SpawnCoords;
        }
        
    }
}