using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku2
{
    public class Table
    {
        private int[,] table;
        public Table()
        {
            table = new int[9, 9] { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
        }
        public int[,] BoardTable
        {
            get
            {
                int[,] tmp = new int[9, 9] { { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
                CopySudokuTable(tmp, table);
                return tmp;
            }
            set
            {
                CopySudokuTable(table, value);
            }
        }
        public int this[int coordX, int coordY]
        {
            get
            {
                return table[coordX, coordY];

            }
            set
            {
                if (value < 10 && value > 0) table[coordX, coordY] = value;
                else table[coordX, coordY] = 0;
            }
        }
        public void CopySudokuTable(int[,] to, int[,] from)
        {
            if (from.Length == 9 * 9 && to.Length == 9 * 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        to[i, j] = from[i, j];
                    }
                }
            }
            else throw new Exception("Problem with sudoku table sizes with copy");
        }
    }
}
