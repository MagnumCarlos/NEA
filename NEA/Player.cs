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
    }
}
