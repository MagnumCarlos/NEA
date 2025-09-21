
using System.Security.AccessControl;

namespace NEA
{
    public class Equipment : Item
    {

        int Level;
        Evidence EvidenceRef;
        public Equipment(string NAME, int LEVEL, Evidence EVIDENCEREF = null) : base(NAME, true)
        {
            this.Level = LEVEL;
            this.EvidenceRef = EVIDENCEREF;
        }
    }
    public class Flashlight : Equipment
    {
        double FacingAngle;
        int ConeAngle = 30;
        int Range = 16;
        int AoERange = 1;
        bool IsOn = false;
        public Flashlight (int LEVEL) : base("Flashlight", LEVEL)
        {
        }
        public void Toggle()
        {
            IsOn = !IsOn;
        }
        public void Rotate(double DeltaAngle)
        {
            FacingAngle = (FacingAngle + DeltaAngle + 360) % 360; //for example if you start at 0 and turn -20 (anticlockwise) then this becomes 340
        }
        public void Illuminate(Player player, Room room)
        {
            room.ClearLighting();
            int PlayerX = player.GetGXCoord() - room.GetOriginX();
            int PlayerY = player.GetGYCoord() - room.GetOriginY();
            if(IsOn)
            {
                for(int i =0; i < room.GetWidth(); i++)
                {
                    for(int j =0; j < room.GetHeight(); j++)
                    {
                        Cell cell = room.GetCells()[i, j];
                        if (IsInCone(PlayerX, PlayerY, cell.GetXCoord(), cell.GetYCoord()))
                        {
                            cell.SetLightState(true);
                        }
                    }
                }
            }
        }
        private bool IsInCone(int PlayerX, int PlayerY, int CellX, int CellY)
        {
            double VectorX = CellX - PlayerX;
            double VectorY = CellY - PlayerY;
            double Distance = Math.Sqrt((VectorX * VectorX) + (VectorY * VectorY));
            if(Distance > Range || Distance ==0)
            {
                return false;
            }
            if(Distance <= AoERange)
            {
                return true;
            }
            double DirX = Math.Cos(FacingAngle * Math.PI / 180);
            double DirY = Math.Sin(FacingAngle * Math.PI / 180); //unit vector representing where flashlight is facing
            //A unit vector is a vector of magnitude 1, so Cos(FacingAngle) = Adjacent / 1 = Adjacent = X Component and similar for Sin

            double Dot = (VectorX * DirX) + (VectorY * DirY); //The dot product between player -> cell vector and facing direction

            double MagA = Math.Sqrt((VectorX * VectorX) + (VectorY * VectorY)); //Magnitude of A, same as distance but CosTheta below makes a lot more sense with this
            double MagB = Math.Sqrt((DirX * DirX) + (DirY * DirY));

            double CosTheta = Dot / (MagA * MagB); //dot product formula

            CosTheta = Math.Max(-1.0, Math.Min(1.0, CosTheta)); //range of the cosine function (included to be safe)
            double Angle = Math.Acos(CosTheta) * 180 / Math.PI; //convert back to degrees
            return Angle < ConeAngle / 2; //if the angle the cell is at from the facing direction is less than half of the cone's spread, it's illuminated

        }
    }
}
