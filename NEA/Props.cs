namespace NEA
{
    public abstract class Prop
    {
    }

    public class RectangularProp : Prop
    {
        int TopLeftX, TopLeftY, Width, Height;
        public RectangularProp(int TOPLEFTX, int TOPLEFTY, int WIDTH, int HEIGHT)
        {
            this.TopLeftX = TOPLEFTX;
            this.TopLeftY = TOPLEFTY;
            this.Width = WIDTH;
            this.Height = HEIGHT;
        }
        public int GetOriginX()
        {
            return TopLeftX;
        }
        public int GetOriginY()
        {
            return TopLeftY;
        }
        public int GetWidth()
        {
            return Width;
        }
        public int GetHeight()
        {
            return Height;
        }
        public bool DetectCollision(RectangularProp Other) //collisions with other rectangles
        {
            int RightA = TopLeftX + Width;
            int BottomA = TopLeftY - Height;

            int RightB = Other.TopLeftX + Other.Width;
            int BottomB = Other.TopLeftY - Other.Height;
            //these find the bounds of the projected rectangle 
            if (TopLeftX < RightB && RightA > Other.TopLeftX)
            {
                if (TopLeftY > BottomB && BottomA < Other.TopLeftY)
                {
                    return true;
                }
            }
            return false;
        }
        public bool DetectCollision(int X, int Y) //collisions with points
        {
            int RightEdge = TopLeftX + Width;
            int BottomEdge = TopLeftY - Height;
            if (X >= TopLeftX && X < RightEdge && Y <= TopLeftY && Y > BottomEdge)
            {
                return true;
            }
            return false;
        }


    }
    public class CircularProp : Prop
    {
        int CentreXCoord, CentreYCoord, Radius;
        public CircularProp(int CENTREXCOORD, int CENTREYCOORD, int RADIUS)
        {
            this.CentreXCoord = CENTREXCOORD;
            this.CentreYCoord = CENTREYCOORD;
            this.Radius = RADIUS;
        }
        public int GetCentreX()
        {
            return CentreXCoord;
        }
        public int GetCentreY()
        {
            return CentreYCoord;
        }
        public int GetRadius()
        {
            return Radius;
        }
        public bool DetectCollision(CircularProp P)
        {
            double DistanceFromCentresX = Math.Pow(CentreXCoord - P.CentreXCoord, 2);
            double DistanceFromCentresY = Math.Pow(CentreYCoord - P.CentreYCoord, 2);
            double DistanceFromCentre = Math.Sqrt(DistanceFromCentresX + DistanceFromCentresY);
            if (Radius + P.Radius >= DistanceFromCentre)
            {
                return true;
            }
            return false;
        }
        public bool DetectCollision(int X, int Y)
        {
            double DistanceFromCentresX = Math.Pow(X - CentreXCoord,2);
            double DistanceFromCentresY = Math.Pow(Y - CentreYCoord,2);
            double distance = Math.Sqrt(DistanceFromCentresX+DistanceFromCentresY);
            if(distance <= Radius)
            {
                return true;
            }
            return false;
        }
    }
}

