using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Sapper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public Field sapperField = new Field();
        DispatcherTimer timer;

        public int counter = 0;
        public int seconds = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeField();
            StartTimer();
        }

        void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(evTick);
            timer.Start();
        }
        void StopTimer()
        {
            timer.Stop();
        }

        void evTick(object sender, EventArgs e)
        {
            seconds++;
            StopWatch.Content = seconds;
        }



        public void InitializeField()
        {
            for (int i = 0; i < sapperField.YSize; i++)
            {
                for (int j = 0; j < sapperField.XSize; j++)
                {
                    Label lbl = new Label();
                    string name = String.Format("N_{0}_{1}", j, i);
                    lbl.Name = name;
                    this.RegisterName(name, lbl);
                    //////внешний вид
                    lbl.Background = Brushes.Olive;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Black;

                    lbl.MouseLeftButtonUp += OnLeftClick;
                    lbl.MouseRightButtonUp += OnRightClick;
                    lbl.MouseMove += OnMyMouseMove;
                    lbl.MouseLeave += OnMyMouseLeave;

                    Cell cell = new Cell();
                    cell.Cx = j;
                    cell.Cy = i;



                    //добавляем ячейку в поле
                    sapperField.SetCellByCoord(cell, j, i);


                    Field.Children.Add(lbl);

                }
            }
            MinesLeft.Content = sapperField.MineCount;
            SetMines();
            //ShowMines();
        }

        //расставить мины
        private void SetMines()
        {
            Random random = new Random();
            int i = 0;
            while (i < sapperField.MineCount)
            {
                int r = random.Next(sapperField.XSize * sapperField.YSize);
                if (sapperField.Cells[r].Mined == false)
                {
                    sapperField.Cells[r].Mined = true;
                    i++;
                }
                else
                {
                    continue;
                }
            }
           

        }

        //показать мины
        private void ShowMines()//for debugging
        {
            for (int i = 0; i < sapperField.YSize; i++)
            {
                for (int j = 0; j < sapperField.YSize; j++)
                {
                   
                    string name = String.Format("N_{0}_{1}", j, i);

                    Label tmp = this.FindName(name) as Label;
                    int c = i * sapperField.YSize + j;
                    if (tmp != null)
                    {
                        if (sapperField.Cells[c].Mined)
                        {
                            tmp.Content = sapperField.Cells[c].Mined.ToString(); 
                        }
                    }
                }
            }   
        }



        private void OnRightClick(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Label;
            int x = NameToX(clicked.Name);
            int y = NameToY(clicked.Name);
            //если не помечена и не раскрыта
            if (sapperField.GetCellByCoord(x, y).IsFlagged == false && sapperField.GetCellByCoord(x,y).IsOpened==false)
            {
                clicked.Background = Brushes.Magenta;
                MinesLeft.Content = ((int)MinesLeft.Content) - 1;
                sapperField.GetCellByCoord(x, y).IsFlagged = true;
                sapperField.FlaggedCount++;
            }
            else if(sapperField.GetCellByCoord(x, y).IsFlagged == true)
            {
                clicked.Background = Brushes.Olive;
                MinesLeft.Content = ((int)MinesLeft.Content) + 1;
                sapperField.GetCellByCoord(x, y).IsFlagged = false;
                sapperField.FlaggedCount--;
            }
            CheckForVictory();


        }

        private void OnLeftClick(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Label;
            int x = NameToX(clicked.Name);
            int y = NameToY(clicked.Name);
            //если не помечена
            if (sapperField.GetCellByCoord(x, y).IsFlagged == false)
            {
                //если не открыта
                if (sapperField.GetCellByCoord(x, y).IsOpened == false)
                {
                    //если мина - game over
                    if (sapperField.GetCellByCoord(x, y).Mined == true)
                    {
                        clicked.Content = "Minen";
                        clicked.Background = Brushes.Red;
                        GameOver();
                    }
                    //иначе - подсчитать количество мин в соседних ячейках
                    else
                    {
                        CountingNearest(x, y);
                    }
                }

            }

        }

        private void GameOver()
        {
            ShowMines();
            StopTimer();
        }

        private void CheckForVictory()
        {
            int square = sapperField.XSize * sapperField.YSize;

            //проверка по количеству открытых
            if (sapperField.OpenedCount == square - sapperField.MineCount)
            {
                MessageBox.Show("Winner");
            }
            //проверка по отмеченным
            else
            {
                int isWinner = 0;
                var arr = sapperField.Cells.Where(c => c.IsFlagged);
                foreach (Cell a in arr)
                {
                    if (a.Mined == true)
                    {
                        isWinner++;
                    }
                }
                if (isWinner == sapperField.MineCount && arr.Count() == isWinner)
                {
                    MessageBox.Show("Winner");
                }
            }
        }

        private int NameToX(string name)
        {
            string[] parts = name.Split('_');
            int x = Int32.Parse(parts[1]);
            return x;
        }

        private int NameToY(string name)
        {
            string[] parts = name.Split('_');
            int y = Int32.Parse(parts[2]);
            return y;
        }

        private void CountingNearest(int x, int y)
        {
            if (sapperField.GetCellByCoord(x, y).IsOpened != true)
            {

                //справа
                if (sapperField.GetCellByCoord(x + 1, y) != null && sapperField.GetCellByCoord(x + 1, y).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //вниз вправо
                if (sapperField.GetCellByCoord(x + 1, y + 1) != null && sapperField.GetCellByCoord(x + 1, y + 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //вниз
                if (sapperField.GetCellByCoord(x, y + 1) != null && sapperField.GetCellByCoord(x, y + 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //вниз влево
                if (sapperField.GetCellByCoord(x - 1, y + 1) != null && sapperField.GetCellByCoord(x - 1, y + 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //влево
                if (sapperField.GetCellByCoord(x - 1, y) != null && sapperField.GetCellByCoord(x - 1, y).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //влево вверх
                if (sapperField.GetCellByCoord(x - 1, y - 1) != null && sapperField.GetCellByCoord(x - 1, y - 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //вверх
                if (sapperField.GetCellByCoord(x, y - 1) != null && sapperField.GetCellByCoord(x, y - 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }
                //вверх вправо
                if (sapperField.GetCellByCoord(x + 1, y - 1) != null && sapperField.GetCellByCoord(x + 1, y - 1).Mined == true)
                {
                    sapperField.GetCellByCoord(x, y).MinesNear++;
                }


                string name = String.Format("N_{0}_{1}", x, y);
                Label tmp = this.FindName(name) as Label;
                tmp.Content = sapperField.GetCellByCoord(x, y).MinesNear;
                if ((int)tmp.Content == 0)
                {
                    tmp.Background = Brushes.Gray;
                }
                else
                {
                    tmp.Background = Brushes.Blue;
                }

                sapperField.GetCellByCoord(x, y).IsOpened = true;
                sapperField.OpenedCount++;

                if (sapperField.GetCellByCoord(x, y).MinesNear == 0)
                {
                    if (sapperField.GetCellByCoord(x + 1, y) != null)
                    {
                        CountingNearest(x + 1, y); //справа
                    }
                    if (sapperField.GetCellByCoord(x + 1, y + 1) != null)
                    {
                        CountingNearest(x + 1, y + 1);//вниз вправо

                    }
                    if (sapperField.GetCellByCoord(x, y + 1) != null)
                    {
                        CountingNearest(x, y + 1);  //вниз

                    }
                    if (sapperField.GetCellByCoord(x - 1, y + 1) != null)
                    {
                        CountingNearest(x - 1, y + 1);//вниз влево

                    }
                    if (sapperField.GetCellByCoord(x - 1, y) != null)
                    {
                        CountingNearest(x - 1, y); //влево

                    }
                    if (sapperField.GetCellByCoord(x - 1, y - 1) != null)
                    {
                        CountingNearest(x - 1, y - 1); //влево вверх

                    }
                    if (sapperField.GetCellByCoord(x, y - 1) != null)
                    {
                        CountingNearest(x, y - 1);//вверх

                    }
                    if (sapperField.GetCellByCoord(x + 1, y - 1) != null)
                    {
                        CountingNearest(x + 1, y - 1);//вверхвправо

                    }
                }
            }
            CheckForVictory();
        }

        private void OnMyMouseMove(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Label;
            int x = NameToX(clicked.Name);
            int y = NameToY(clicked.Name);
            if (sapperField.GetCellByCoord(x, y).IsOpened == false && sapperField.GetCellByCoord(x, y).IsFlagged == false)
            {
                clicked.Background = Brushes.DarkOliveGreen;
            }
        }

        private void OnMyMouseLeave(object sender, RoutedEventArgs e)
        {
            var clicked = sender as Label;
            int x = NameToX(clicked.Name);
            int y = NameToY(clicked.Name);

            if (sapperField.GetCellByCoord(x, y).IsOpened == false && sapperField.GetCellByCoord(x, y).IsFlagged == false)
            {
                clicked.Background = Brushes.Olive;
            }

        }

        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
