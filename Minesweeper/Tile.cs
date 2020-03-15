using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    public class Tile : INotifyPropertyChanged
    {
        bool hasbeenClicked;
        bool flagged;

        public int TileID { get; set; }
        public int BoardSize { get; set; }
        public bool HasMine { get; set; }
        public int MinesSurrounding { get; set; }
        public bool HasBeenClicked { get { return hasbeenClicked; } set { hasbeenClicked = value; NotifyPropertyChanged("TileID"); NotifyPropertyChanged("HasBeenClicked"); } }
        public bool Flagged { get { return flagged; } set { flagged = value; NotifyPropertyChanged("TileID"); NotifyPropertyChanged("Flagged"); } }

        public Tile(bool MineBool, int Size)
        {
            TileID = Board.TheBoard.Count;
            HasMine = MineBool;
            HasBeenClicked = false;
            Flagged = false;
            BoardSize = Size;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int GetTileID(int Xpos, int Ypos)
        {
            if (Xpos < 0 || Ypos < 0)
            {
                return -1;
            }
            if (Xpos >= BoardSize || Ypos >= BoardSize)
            {
                return -1;
            }
            int ID = (Ypos * BoardSize) + Xpos;
            return ID;
        }

        public int[] GetTilePos(int TileIDInitial)
        {
            int TileID = TileIDInitial;
            int TileIDX = TileID;
            int TileIDY = 0;
            while (TileIDX > 0)
            {
                TileIDX = TileIDX - BoardSize;
            } // 5 + TileIDX to get the x axis
            if (TileIDX < 0)
            {
                TileIDX = TileIDX + BoardSize;
            }
            while (TileID >= 0)
            {
                TileID = TileID - BoardSize;
                TileIDY = TileIDY + 1;
            }
            TileIDY = TileIDY - 1;
            int[] TilePos = { TileIDX, TileIDY };
            return TilePos;
        }

        public void CalculateMinesSurrounding(int TileID) // Parameter might not be needed.
        {
            int[] CurrentTilePos = GetTilePos(TileID);
            int Xpos = CurrentTilePos[0];
            int Ypos = CurrentTilePos[1];
            int Mines = 9;
            int[] TileIDs =
            {
                GetTileID(Xpos - 1, Ypos - 1), GetTileID(Xpos, Ypos - 1), GetTileID(Xpos + 1,Ypos - 1),
                GetTileID(Xpos - 1, Ypos),                                GetTileID(Xpos + 1,Ypos),
                GetTileID(Xpos - 1, Ypos + 1), GetTileID(Xpos, Ypos + 1), GetTileID(Xpos + 1,Ypos + 1)
                //TileID - (BoardSize + 1), TileID - BoardSize, TileID - (BoardSize - 1),
                //TileID - 1,                                   TileID + 1,
                //TileID + (BoardSize - 1), TileID + BoardSize, TileID + (BoardSize + 1)
            };

            if (Board.TheBoard[TileID].HasMine == false)
            {
                Mines = 0;
                foreach (int i in Enumerable.Range(0, 8))
                {
                    if (TileIDs[i] >= 0 && TileIDs[i] < Math.Pow(BoardSize, 2))
                    {
                        if (Board.TheBoard[TileIDs[i]].HasMine == true)
                        {
                            Mines++;
                        }
                    }
                }
            }
            this.MinesSurrounding = Mines;
        }

        public void RevealTiles(int TileID) // im just putting this parameter in because the thing above this has it
        {
            int[] CurrentTilePos = GetTilePos(TileID);
            int Xpos = CurrentTilePos[0];
            int Ypos = CurrentTilePos[1];
            List<int> TilesToCheck = new List<int>();

            // get ids of tiles surrounding this tile
            int[] TileIDs =
            {
                GetTileID(Xpos - 1, Ypos - 1), GetTileID(Xpos, Ypos - 1), GetTileID(Xpos + 1,Ypos - 1),
                GetTileID(Xpos - 1, Ypos),                                GetTileID(Xpos + 1,Ypos),
                GetTileID(Xpos - 1, Ypos + 1), GetTileID(Xpos, Ypos + 1), GetTileID(Xpos + 1,Ypos + 1)
            };

            foreach (int i in Enumerable.Range(0, 8))
            {
                if (TileIDs[i] >= 0 && TileIDs[i] < Math.Pow(BoardSize, 2) && Board.TheBoard[TileIDs[i]].HasBeenClicked == false)
                {
                    if (Board.TheBoard[TileIDs[i]].MinesSurrounding == 0)
                    {
                        if (Board.TheBoard[TileIDs[i]].flagged == false) // wont reveal flagged tiles
                        {
                            Board.TheBoard[TileIDs[i]].HasBeenClicked = true;
                        }                        
                        Board.TheBoard[TileIDs[i]].RevealTiles(TileIDs[i]); // checks the new tiles 
                        //TilesToCheck.Add(TileIDs[i]);
                    }


                    if (Board.TheBoard[TileIDs[i]].MinesSurrounding > 0 && Board.TheBoard[TileIDs[i]].flagged == false)// wont reveal flagged tiles
                    {
                        Board.TheBoard[TileIDs[i]].HasBeenClicked = true;
                    }

                }
            }

            //SearchTiles(TilesToCheck);

        }

        public void SearchTiles(List<int> TilesToCheck) // dont think this is used
        {
            List<int> NewTilesToCheck = new List<int>();
            foreach (int i in Enumerable.Range(0, TilesToCheck.Count))
            {
                if (TilesToCheck[i] >= 0 && TilesToCheck[i] < Math.Pow(BoardSize, 2))
                {
                    if (Board.TheBoard[TilesToCheck[i]].MinesSurrounding == 0)
                    {
                        Board.TheBoard[TilesToCheck[i]].HasBeenClicked = true;
                        NewTilesToCheck.Add(TilesToCheck[i]);
                    }

                    if (Board.TheBoard[TilesToCheck[i]].MinesSurrounding > 0)
                    {
                        Board.TheBoard[TilesToCheck[i]].HasBeenClicked = true;
                    }

                }
            }
            //if ( NewTilesToCheck.Count != 0)
            //{
            //    foreach (int i in Enumerable.Range(0, NewTilesToCheck.Count))
            //    {
            //        RevealTiles(NewTilesToCheck[i]);
            //    }
            //    //SearchTiles(NewTilesToCheck);
            //}

        }
    }
}
