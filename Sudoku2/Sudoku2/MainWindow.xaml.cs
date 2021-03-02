using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sudoku2
{
    
    public partial class MainWindow : Window
    {
        readonly int MAX_TRIES = 30;
        int complexity = 45;
        int h = 0, m = 0, s = 0;
        SudokuGame sudoku= new SudokuGame();
        Random rnd= new Random();

        public MainWindow()
        {
            InitializeComponent();
            sudoku = new SudokuGame();
            
        }

        #region Methods For Filling Of The Table
        private void ShowOnTheBoard()
        {
            int i = 0, j = 0;
            int r = 0, c = 0;
            foreach (var item in grSudokoTable.Children)
            {
                j = 0;
                foreach (var box in (item as Grid).Children)
                {
                    // having i and j for each box we calculate which i and j is which row and column index of the small grids
                    r = (((int)Math.Floor((decimal)i / 3)) * 3) + (int)Math.Floor((decimal)j / 3);//1,2,3 са на 0ви ред, 3 4 5 на 1ви...
                    c = (((int)Math.Floor((decimal)i % 3)) * 3) + j % 3;

                    if (sudoku.sudokuTable[r, c] == 0) (box as TextBox).Text = "";
                    else (box as TextBox).Text = "" + sudoku.sudokuTable[r, c];
                    j++;
                }
                i++;
            }
        }
        private void FillTheTable()
        {
            FillDiagonal();

            while (!TryFillTheWholeTable())
            {
                FillDiagonal();
            }
            sudoku.sudokuTable.CopySudokuTable(sudoku.filledTable.BoardTable, sudoku.sudokuTable.BoardTable);

        }
        private void FillDiagonal()
        {
            FillBox(0, 0);
            FillBox(3, 3);
            FillBox(6, 6);
        }
        private void FillBox(int row, int col)
        {
            int num=0;
            int leftUpBoxRow = row - (row % 3);
            int leftUpBoxCol = col - (col % 3);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    do
                    {
                        num = rnd.Next(1, sudoku.NUMBEROFROWS+1); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! to numberOfRows
                    }
                    while (!IsPossibleInSQ(leftUpBoxRow, leftUpBoxCol, num));
                    sudoku.sudokuTable[leftUpBoxRow + i, leftUpBoxCol + j] = num;
                }
            }
        }
        private bool IsPossibleInSQ(int r, int c, int currNum)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (sudoku.sudokuTable[r + i, c + j] == currNum) return false;
                }
            }
            return true;
        }
        private bool IsPossibleInRow(int row, int currN)
        {
            for (int i = 0; i < sudoku.NUMBEROFROWS; i++)
            {
                if (sudoku.sudokuTable[row, i] == currN) return false;
            }
            return true;
        }
        private bool IsPossibleInColumn(int col, int currN)
        {
            for (int j = 0; j < sudoku.NUMBEROFROWS; j++)
            {
                if (sudoku.sudokuTable[j, col] == currN) return false;
            }
            return true;
        }
        private bool TryFillTheWholeTable()
        {
            int curr;

            for (int i = 0; i < sudoku.NUMBEROFROWS; i++)
            {
                for (int j = 0; j < sudoku.NUMBEROFROWS; j++)
                {
                    int tries = 0;
                    if (sudoku.sudokuTable[i, j] == 0)
                    {
                        do
                        {
                            tries++;
                            curr = rnd.Next(1, sudoku.NUMBEROFROWS + 1);
                            if (tries == MAX_TRIES) return false;
                        }
                        while (!(IsPossibleInRow(i, curr) && IsPossibleInColumn(j, curr) && IsPossibleInSQ(i - (i % 3), j - (j % 3), curr)));

                        sudoku.sudokuTable[i, j] = curr;
                    }
                }
            }
            return true;
        }
        private void RemoveDigits()
        {
            int numOfDigits = GetComplexity();
            while (numOfDigits != 0)
            {
                int col = rnd.Next(0, sudoku.NUMBEROFROWS);
                int row = rnd.Next(0, sudoku.NUMBEROFROWS);
                if (sudoku.sudokuTable[row, col] != 0)
                {
                    numOfDigits--;
                    sudoku.sudokuTable[row, col] = 0;
                }
            }
        }
        private int GetComplexity()
        {
            if (sudoku.complexity == Complexity.EASY) return 45;
            else if (sudoku.complexity == Complexity.MEDIUM) return 55;
            else return 68;
        }
        #endregion

        #region EventsChangingNumbersOnTheBoard
        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            int posX=0, posY=0;
            if (sudoku.LOADED)
            {
                int curr;
                TextBox txtBox = (TextBox)sender;
                if (txtBox.Text == "")
                {
                    txtBox.Background = Brushes.White;
                }
                else if (int.TryParse(txtBox.Text, out curr))
                {
                    string[] name = txtBox.Name.Split('_');
                    try
                    {
                        posX = Int32.Parse(name[1]);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unable to parse {name[1]}");
                    }
                    try
                    {
                        posY = Int32.Parse(name[2]);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unable to parse {name[2]}");
                    }

                    if (CheckIfCorrect(posX, posY, curr))
                    {
                        txtBox.Background = Brushes.Green;
                        txtBox.IsEnabled = false;
                        lblMessage.Content = "";
                        sudoku.sudokuTable[posX, posY] = curr;
                        sudoku.History.Push(new KeyValuePair<TextBox, string>(sender as TextBox, txtBox.Text));
                    }
                    else
                    {
                        txtBox.Background = Brushes.Red;
                        lblMessage.Content = "Wrong Number: " + curr;
                        sudoku.History.Push(new KeyValuePair<TextBox, string>(sender as TextBox, txtBox.Text));
                    }
                }
                else
                {
                    lblMessage.Content = "INCORECT INPUT";
                }
            }
            else
            {
                lblMessage.Content = "GAME NOT LOADED";
            }
        }
        private bool CheckIfCorrect(int posX, int posY, int currN)
        {
            return sudoku.filledTable[posX, posY] == currN;
        }
        #endregion

        #region Menu File
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            //restartTheGame();
            sudoku = new SudokuGame();

            sudoku.LOADED = true;

            FillTheTable();

            RemoveDigits();

            ShowOnTheBoard();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Text|*.txt";
                saveFileDialog1.Title = "Save a sudoku table";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    StringBuilder values = new StringBuilder();
                    foreach (var candidate in sudoku.sudokuTable.BoardTable)
                        values.Append(candidate);
                    foreach (var candidate in sudoku.filledTable.BoardTable)
                        values.Append(candidate);
                    string res = values.ToString();
                    Stream fileStream = saveFileDialog1.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    sw.Write(res);
                    sw.Close();
                    fileStream.Close();

                }
            }
            }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //Load from file
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.ShowDialog();
            string strFileName = openFileDialog.FileName;
            string text = System.IO.File.ReadAllText(strFileName);
            for (int i = 0; i < sudoku.NUMBEROFROWS; i++)
            {
                for (int j = 0; j < sudoku.NUMBEROFROWS; j++)
                {
                    int position = i * 9 + j;
                    char currNumber = text[position];
                    sudoku.sudokuTable[i, j] = (int)currNumber - 48;
                }
            }
            for (int i = 0; i < sudoku.NUMBEROFROWS; i++)
            {
                for (int j = 0; j < sudoku.NUMBEROFROWS; j++)
                {
                    sudoku.filledTable[i, j] = (int)text[(i + 9) * 9 + j] - 48;
                }
            }
            sudoku.LOADED = true;

            ShowOnTheBoard();
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                Solve();
                dispatcherTimer.Stop();
            }
        }
        private void Solve()
        {
            sudoku.sudokuTable.CopySudokuTable(sudoku.sudokuTable.BoardTable, sudoku.filledTable.BoardTable);
            ShowOnTheBoard();
        }
        #endregion

        #region Edit
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED && sudoku.gameStarted)
            {
                if (sudoku.History.Count != 0)
                {
                    KeyValuePair<TextBox, string> last = sudoku.History.Pop();
                    sudoku.Redo.Push(new KeyValuePair<TextBox, string>(last.Key, last.Value));
                    if (sudoku.History.Count == 0)//ако първото въведено число е правилно но искаме да направим undo
                    {
                        last.Key.IsEnabled = true;
                        last.Key.Text = "";
                        last.Key.Background = Brushes.White;

                    }
                    else if (last.Key == sudoku.History.Peek().Key)
                    {
                        sudoku.History.Peek().Key.Text = sudoku.History.Peek().Value;
                        if (sudoku.History.Peek().Key.Background == Brushes.Red || sudoku.History.Peek().Key.Background == Brushes.White) last.Key.IsEnabled = true;
                    }
                    else
                    {
                        last.Key.Text = "";
                    }
                }
                else lblMessage.Content = "Can not undo anything more!";
            }
            else
            {
                lblMessage.Content = "Game not loaded";
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                if (sudoku.Redo.Count != 0)
                {
                    var last = sudoku.Redo.Pop();
                    (last.Key as TextBox).Text = last.Value;
                    sudoku.History.Push(new KeyValuePair<TextBox, string>(last.Key, last.Value));
                }
                else lblMessage.Content = "Can not redo anything more!";
            }
            else
            {
                lblMessage.Content = "Game not loaded";
            }
        }
        #endregion

        #region Level
        private void btnEasy_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                if (sudoku.complexity == Complexity.MEDIUM)
                {
                    for (int i = 0; i < 13; i++) Help();
                }
                else if (sudoku.complexity == Complexity.DIFFICULT)
                {
                    for (int i = 0; i < 25; i++) Help();
                }
                else
                {
                    complexity = 45;
                    sudoku.sudokuTable.CopySudokuTable(sudoku.sudokuTable.BoardTable, sudoku.filledTable.BoardTable);
                    RemoveDigits();
                }

                ShowOnTheBoard();

                MarkButton(btnEasy);
            }
        }

        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                if (sudoku.complexity == Complexity.EASY)
                {
                    for (int i = 0; i < 13; i++) Remove();
                }
                else if (sudoku.complexity == Complexity.DIFFICULT)
                {
                    for (int i = 0; i < 12; i++) Help();
                }
                else
                {
                    complexity = 58;
                    sudoku.sudokuTable.CopySudokuTable(sudoku.sudokuTable.BoardTable, sudoku.filledTable.BoardTable);
                    RemoveDigits();
                }

                MarkButton(btnMedium);

                ShowOnTheBoard();
            }
        }

        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                if (sudoku.complexity == Complexity.EASY)
                {
                    for (int i = 0; i < 25; i++) Remove();
                }
                else if (sudoku.complexity == Complexity.MEDIUM)
                {
                    for (int i = 0; i < 12; i++) Remove();
                }
                else
                {
                    complexity = 70;
                    sudoku.sudokuTable.CopySudokuTable(sudoku.sudokuTable.BoardTable, sudoku.filledTable.BoardTable);
                    RemoveDigits();
                }

                MarkButton(btnHard);
                ShowOnTheBoard();
            }
        }

        private void MarkButton(Button button)
        {
            btnEasy.FontWeight = FontWeights.Normal;
            btnMedium.FontWeight = FontWeights.Normal;
            btnHard.FontWeight = FontWeights.Normal;
            button.FontWeight = FontWeights.Bold;
        }

        private void Help()
        {
            int i, j;
            do
            {
                i = rnd.Next(0, sudoku.NUMBEROFROWS);
                j = rnd.Next(0, sudoku.NUMBEROFROWS);
            } while (sudoku.sudokuTable[i, j] != 0);
            sudoku.sudokuTable[i, j] = sudoku.filledTable[i, j];
        }

        private void Remove()
        {
            int i, j;
            do
            {
                i = rnd.Next(0, sudoku.NUMBEROFROWS);
                j = rnd.Next(0, sudoku.NUMBEROFROWS);
            } while (sudoku.sudokuTable[i, j] == 0);
            sudoku.sudokuTable[i, j] = 0;
        }

        #endregion

        #region Other
        private void btnHint_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED)
            {
                Help();
                ShowOnTheBoard();
            }
            else
            {
                lblMessage.Content = "Game not loaded";
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED && !sudoku.gameStarted)
            {
                sudoku.gameStarted = true;
                foreach (var item in grSudokoTable.Children)
                {
                    foreach (var box in (item as Grid).Children)
                    {
                        if ((box as TextBox).Background == Brushes.White || (box as TextBox).Background == Brushes.Red) (box as TextBox).IsEnabled = true;
                    }
                }

                var timer = new DispatcherTimer(); // creating a new timer
                TimerTest();

            }
            else
            {
                lblMessage.Content = "The game is not loaded";
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.LOADED && sudoku.gameStarted)
            {
                dispatcherTimer.Stop();
                foreach (var item in grSudokoTable.Children)
                {
                    foreach (var box in (item as Grid).Children)
                    {
                        if ((box as TextBox).Background != Brushes.Green)
                        {
                            var result = MessageBox.Show("The sudoku is not solved. Do you want to finish the game?", "Finish Game?", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes) Solve();
                            else
                            {
                                lblMessage.Content = "GOOD JOB";
                                return;
                            }
                        }
                    }
                }
                sudoku.gameStarted = false;

            }
            else
            {
                lblMessage.Content = "The game is not started or loaded";
            }
        }
        #endregion

        #region Timer
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private void TimerTest()
        {

            dispatcherTimer.Tick += new EventHandler((sender, e) => dispatcherTimer_Tick(sender, e, ref s, ref m, ref h));
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e, ref int s, ref int m, ref int h)
        {
            if (s + 1 == 60)
            {
                s = 0;
                m += 1;
            }
            else s += 1;
            if (m == 60)
            {
                m = 0;
                h += 1;
            }
            lblTime.Content = $"{h}:{m}:{s}";
        }
        #endregion
    }
}