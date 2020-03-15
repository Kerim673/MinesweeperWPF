using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Interop;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            Board Tiles = Board.InitiateBoard();
            foreach (int i in Enumerable.Range(0, Board.TheBoard.Count()))
            {
                Board.TheBoard[i].CalculateMinesSurrounding(i);
            }
            //DataContext = Board.TheBoard;
            
            ButtonGrid.ItemsSource = Board.TheBoard;
            Application.Current.MainWindow.Width = Board.BoardSize * 21 + 54;
            Application.Current.MainWindow.Height = Board.BoardSize * 21 + 139;
            DebugBlock.Text = "Try not to explode";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var TileClickedIDObject = ((Button)sender).Tag;
            int TileClickedID = Convert.ToInt32(TileClickedIDObject);
            if (Board.TheBoard[TileClickedID].Flagged == false)
            {
                if (Board.TheBoard[TileClickedID].HasMine == true)
                {
                    Board.TheBoard[TileClickedID].HasBeenClicked = true;
                    Board.RevealAll(); // end game by blowing up
                    DebugBlock.Text = "You died";
                }
                else
                {
                    if (Board.TheBoard[TileClickedID].MinesSurrounding != 0)
                    {
                        // reveal the number of this tile
                        Board.TheBoard[TileClickedID].HasBeenClicked = true;
                        //DebugBlock.Text = Board.TheBoard[TileClickedID].HasBeenClicked.ToString();
                    }
                    if (Board.TheBoard[TileClickedID].MinesSurrounding == 0)
                    {
                        // discover the tiles
                        Board.TheBoard[TileClickedID].HasBeenClicked = true;
                        Board.TheBoard[TileClickedID].RevealTiles(TileClickedID); // parameter not needed if function is done properly
                    }
                }
            }
            if (Board.GameWon() == true)
            {
                DebugBlock.Text = "Win";
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemDifficulty_Click(object sender, RoutedEventArgs e)
        {
            var GameSetiings = ((MenuItem)sender).Tag;
            Board.BoardSize = Convert.ToInt32(GameSetiings.ToString().Substring(0, 2));
            Board.TotalMines = Convert.ToInt32(GameSetiings.ToString().Substring(3, 3));
        }

        private void MineTile_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var TileClickedIDObject = ((Button)sender).Tag;
            int TileClickedID = Convert.ToInt32(TileClickedIDObject);
            if (Board.TheBoard[TileClickedID].HasBeenClicked == false)
            {
                if (Board.TheBoard[TileClickedID].Flagged == false)
                {
                    Board.TheBoard[TileClickedID].Flagged = true;
                    Board.FlaggedMines += 1;
                    if (Board.TheBoard[TileClickedID].HasMine == true)
                    {
                        Board.CorrectFlaggedMines += 1;
                    }
                }
                else
                {
                    Board.TheBoard[TileClickedID].Flagged = false;
                    Board.FlaggedMines -= 1;
                    if (Board.TheBoard[TileClickedID].HasMine == true)
                    {
                        Board.CorrectFlaggedMines -= 1;
                    }
                }

                if (Board.GameWon() == true)
                {
                    DebugBlock.Text = "Win";
                }
            }
        }
    }

    public class ConvertSquaresToPic : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int TileID = (int)value;
            if (Board.TheBoard[TileID].HasBeenClicked == false)
            {
                if (Board.TheBoard[TileID].Flagged == true)
                {
                    return "⛿";
                }
                else
                {
                    return " ";
                }
            }
            else
            {
                if (Board.TheBoard[TileID].MinesSurrounding == 0)
                {
                    return " ";
                }
                if (Board.TheBoard[TileID].MinesSurrounding == 9)
                {
                    return "⬤";
                }
                else
                {
                    return Board.TheBoard[TileID].MinesSurrounding;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertMineNumberToColour : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int TileID = (int)value;
            if (Board.TheBoard[TileID].Flagged == true)
            {
                return new SolidColorBrush(Colors.Black);
            }
            int Number = Board.TheBoard[TileID].MinesSurrounding;
            switch (Number)
            {
                case 8:
                    return new SolidColorBrush(Colors.Pink);
                case 7:
                    return new SolidColorBrush(Colors.Red);
                case 6:
                    return new SolidColorBrush(Colors.Orange);
                case 5:
                    return new SolidColorBrush(Colors.Yellow);
                case 4:
                    return new SolidColorBrush(Colors.YellowGreen);
                case 3:
                    return new SolidColorBrush(Colors.LawnGreen);
                case 2:
                    return new SolidColorBrush(Colors.Blue);
                case 1:
                    return new SolidColorBrush(Colors.DarkBlue);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertButtonBG : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) //this converter is a mess
        {
            bool Clicked = (bool)value;

            //ImageBrush Ranjit = new ImageBrush();
            //Ranjit.ImageSource = new BitmapImage();

            var SquareOff = Imaging.CreateBitmapSourceFromHBitmap(Minesweeper.Properties.Resources.SquareOff.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            //MyButton.Background = new ImageBrush(bitmapSource);
            var SquareOn = Imaging.CreateBitmapSourceFromHBitmap(Minesweeper.Properties.Resources.SquareOn.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            //MyButton.Background = new ImageBrush(bitmapSource);

            if (Clicked == false)
            {
                //return new ImageBrush(new BitmapImage(new Uri("../../Properties/Images/SquareOff.png", UriKind.RelativeOrAbsolute)));
                return new ImageBrush(SquareOff);
            }
            else
            {
                //return new ImageBrush(new BitmapImage(new Uri("../../Properties/Images/SquareOn.png", UriKind.RelativeOrAbsolute)));
                return new ImageBrush(SquareOn);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

// This is what the board will look like, so I don't forget.

// 0  1  2  3  4
// 5  6  7  8  9
// 10 11 12 13 14
// 15 16 17 18 19
// 20 21 22 23 24