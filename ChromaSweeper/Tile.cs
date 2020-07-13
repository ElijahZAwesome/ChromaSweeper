using Chroma.Graphics;
using System.Numerics;

namespace ChromaSweeper
{
    public class Tile
    {

        public static int TileSize = 16;

        public int Number;
        public bool Bomb;
        public bool Checked;

        public bool BeingHeld;

        public bool Flagged;
        public bool Question;
        /// <summary>
        /// The tile based position on the board
        /// </summary>
        public Vector2 BoardPosition;
        /// <summary>
        /// The scaled, pixel based position of this tile to render
        /// </summary>
        public Vector2 Position;
        public Vector2 Scale;

        private int FrameToDisplay;

        public Tile(Vector2 pos)
        {
            Number = 0;
            BoardPosition = pos;
            Bomb = false;
            Checked = false;
            Flagged = false;

            Scale = new Vector2(TileSize / SweeperGame.Instance.TilesSheet.FrameWidth);
            Position = (BoardPosition * TileSize) + SweeperGame.Instance.BoardPosition;

            DetermineFrame();
        }

        public void Init()
        {
            Number = GetNumberToDisplay();

            DetermineFrame();
        }

        public void Check()
        {
            if (Flagged)
            {
                if (Question)
                {
                    // Unflag
                    Flag();
                    return;
                }

                return;
            }

            if (Checked)
                return;

            Checked = true;
            SweeperGame.Instance.Board.UncheckedTiles--;

            if (SweeperGame.Instance.Board.UncheckedTiles <= 0)
            {
                SweeperGame.Instance.Victory();
            }

            if (Bomb)
            {
                SweeperGame.Instance.BombHit(BoardPosition);
            }

            if (Number == 0 && !Bomb)
            {
                CheckNeighbors();
            }

            DetermineFrame();
        }

        public void Flag()
        {
            if (Checked)
                return;

            if (Flagged)
            {
                if (Question)
                {
                    Flagged = false;
                    Question = false;
                }
                else
                {
                    Question = true;
                    SweeperGame.Instance.FlagsLeft++;
                    SweeperGame.Instance.LeftNumbers.UpdateNumber(SweeperGame.Instance.FlagsLeft);
                }
            }
            else
            {
                Flagged = true;
                SweeperGame.Instance.FlagsLeft--;
                SweeperGame.Instance.LeftNumbers.UpdateNumber(SweeperGame.Instance.FlagsLeft);
            }

            DetermineFrame();
        }

        private void CheckNeighbors()
        {
            SweeperGame.Instance.GetNeighbors(BoardPosition).ForEach(neighbour =>
            {
                if (!neighbour.Checked && !neighbour.Bomb)
                    neighbour.Check();
            });
        }

        public void Draw(RenderContext context)
        {
            if (BeingHeld)
            {
                SweeperGame.Instance.TilesSheet.DrawManual(context, 0, Position, Scale);
            }
            else
            {
                SweeperGame.Instance.TilesSheet.DrawManual(context, FrameToDisplay, Position, Scale);
            }
        }

        public void DetermineFrame()
        {
            if (Checked && Bomb)
            {
                FrameToDisplay = 11;
                return;
            }

            if (Question)
            {
                FrameToDisplay = 14;
                return;
            }

            if (Bomb && SweeperGame.Instance.GameOver)
            {
                FrameToDisplay = 13;
                return;
            }

            if (Flagged && !Bomb && SweeperGame.Instance.GameOver)
            {
                FrameToDisplay = 12;
                return;
            }

            if (Flagged)
            {
                FrameToDisplay = 10;
                return;
            }

            if (!Checked)
            {
                FrameToDisplay = 9;
                return;
            }

            FrameToDisplay = Number;
        }

        private int GetNumberToDisplay()
        {
            int surroundingBombs = 0;
            SweeperGame.Instance.GetNeighbors(BoardPosition).ForEach(neighbour =>
            {
                if (neighbour.Bomb)
                {
                    surroundingBombs++;
                }
            });

            return surroundingBombs;
        }

    }
}
