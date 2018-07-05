using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sapper
{
    public class Cell
    {
        public int Cx { get; set; }
        public int Cy { get; set; }
        public bool Mined { get; set; }
        public bool IsOpened { get; set; }
        public bool IsFlagged { get; set; }
        public int MinesNear { get; set; }
        public Cell()
        {
            Cx = 0;
            Cy = 0;
            Mined = false;
            IsOpened = false;
            IsFlagged = false;
            MinesNear = 0;
        }
    }
}
