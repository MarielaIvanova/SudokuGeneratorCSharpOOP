using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Sudoku2
{
    public class SudokuGame
    {
        public readonly int NUMBEROFROWS = 9;
        public Table sudokuTable, filledTable;
        private Stack<KeyValuePair<TextBox, string>> history, redo;
        public bool gameStarted { set; get; }
        public bool LOADED { set; get; }
        public Complexity complexity { get; set; }

        public SudokuGame()
        {
            sudokuTable = new Table();
            filledTable = new Table();
            history = new Stack<KeyValuePair<TextBox, string>>();
            redo = new Stack<KeyValuePair<TextBox, string>>();
            gameStarted = false;
            LOADED = false;
            complexity = Complexity.EASY;
        }

        
        public Stack<KeyValuePair<TextBox, string>> History
        {
            get
            {
                Stack<KeyValuePair<TextBox, string>> tmp = new Stack<KeyValuePair<TextBox, string>>();
                Stack<KeyValuePair<TextBox, string>> tmp2 = new Stack<KeyValuePair<TextBox, string>>();

                while (history.Count != 0)
                {
                    tmp.Push(history.Pop());
                }
                KeyValuePair<TextBox, string> curr=new KeyValuePair<TextBox, string>();
                while (tmp.Count != 0)
                {
                    curr = tmp.Pop();
                    tmp2.Push(curr);
                    history.Push(curr);
                }
                return tmp2;
            }
            set
            {
                if (value != null)
                {
                    Stack<KeyValuePair<TextBox, string>> tmp = new Stack<KeyValuePair<TextBox, string>>();
                    while (value.Count != 0)
                    {
                        tmp.Push(value.Pop());
                    }
                    while (tmp.Count != 0)
                    {
                        history.Push(tmp.Pop());
                    }
                }
            }
        }
        public Stack<KeyValuePair<TextBox, string>> Redo
        {
            get
            {
                Stack<KeyValuePair<TextBox, string>> tmp = new Stack<KeyValuePair<TextBox, string>>();
                Stack<KeyValuePair<TextBox, string>> tmp2 = new Stack<KeyValuePair<TextBox, string>>();

                while (redo.Count != 0)
                {
                    tmp.Push(redo.Pop());
                }
                KeyValuePair<TextBox, string> curr = new KeyValuePair<TextBox, string>();
                while (tmp.Count != 0)
                {
                    curr = tmp.Pop();
                    tmp2.Push(curr);
                    redo.Push(curr);
                }
                return tmp2;
            }
            set
            {
                if (value != null)
                {
                    Stack<KeyValuePair<TextBox, string>> tmp = new Stack<KeyValuePair<TextBox, string>>();
                    while (value.Count != 0)
                    {
                        tmp.Push(value.Pop());
                    }
                    while (tmp.Count != 0)
                    {
                        redo.Push(tmp.Pop());
                    }
                }
            }
        }

    }
}
