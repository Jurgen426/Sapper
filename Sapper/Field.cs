using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sapper
{
    public class Field
    {

        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public List<Cell> Cells { get; set; }

        public int MineCount { get; set; }
        public int FlaggedCount { get; set; }
        public int OpenedCount { get; set; }

        public Field()
        {
            XSize = 9;
            YSize = 9;
            MineCount = 10;
            OpenedCount = 0;
            Cells = new List<Cell>();
        }

        public int GetMineCount()
        {
            return MineCount;
        }

        public Cell GetCellByCoord(int x, int y)
        {
            if ((x >= 0 && x <= XSize - 1) && (y >= 0 && y <= YSize - 1))
            {
                int index = y * YSize + x;
                return Cells[index];
            }
            else
            {
                return null;
            }
        }

        public void SetCellByCoord(Cell cell, int x, int y)
        {
            int index = y * YSize + x;
            //если такой индекс уже есть - заменить
            if (Cells.Count > index)
            {
                Cells[index] = cell;
            }
            //иначе - добавить в конец листа
            else
            {
                Cells.Add(cell);
            }
        }
    }
}
