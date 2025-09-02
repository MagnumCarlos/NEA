using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    public class Equipment : Item
    {

        int Level;
        public Equipment(string NAME, int LEVEL) : base(NAME, true)
        {
            this.Level = LEVEL;
        }
    }
}
