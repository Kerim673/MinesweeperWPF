using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Board
    {
        Random RNG = new Random();
        public static int BoardSize = 9;
        public static int TotalMines = 10;
        public static int FlaggedMines = 0;
        public static int CorrectFlaggedMines = 0;
        //int CurrentMines = 0;
        int PossibleMineLocation;
        bool PositionTaken;
        List<int> MineLocations = new List<int>();

        public ObservableCollection<Tile> Tiles { get; set; }
        public static ObservableCollection<Tile> TheBoard;

        public Board()
        {
            TheBoard = new ObservableCollection<Tile>();

            // Generates mines in random locations within the specified board size.
            while (MineLocations.Count < TotalMines)
            {
                PossibleMineLocation = RNG.Next(BoardSize * BoardSize);
                PositionTaken = false;
                foreach (int j in Enumerable.Range(0, MineLocations.Count))
                {
                    if (PossibleMineLocation == MineLocations[j])
                    {
                        PositionTaken = true;
                    }
                }
                if (PositionTaken == false)
                {
                    MineLocations.Add(PossibleMineLocation);
                }
            }

            foreach (int i in Enumerable.Range(0, BoardSize * BoardSize))
            {
                //IsMine = RNG.Next(0, 3);
                if (MineLocations.Contains(i)) //(IsMine == 1 && CurrentMines < TotalMines)
                {
                    TheBoard.Add(new Tile(true, BoardSize));
                    //CurrentMines++;
                }
                else
                {
                    TheBoard.Add(new Tile(false, BoardSize));
                }
            }
        }

        public static Board InitiateBoard()
        {
            return new Board() { Tiles = TheBoard };
        }

        public static bool GameWon()
        {
            if (FlaggedMines == CorrectFlaggedMines && CorrectFlaggedMines == TotalMines)
            {
                int TilesClicked = 0;
                foreach (int i in Enumerable.Range(0, TheBoard.Count))
                {
                    if (TheBoard[i].HasBeenClicked == true && TheBoard[i].Flagged == false)
                    {
                        TilesClicked += 1;
                    }
                }
                if (TilesClicked == TheBoard.Count - TotalMines)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public static void RevealAll()// needs to change the mine and flag pictures to show if the prediction was correct or not
        {
            foreach (int i in Enumerable.Range(0, TheBoard.Count))
            {
                TheBoard[i].HasBeenClicked = true;
            }
        }
    }
}
