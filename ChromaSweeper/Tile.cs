using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using Chroma.Graphics;

namespace ChromaSweeper
{
    public class Tile
    {

        public static int TileSize = 16;

        public int Number;
        public bool Bomb;
        public bool Checked;
        public bool Flagged;
        public bool Question;
        public Vector2 Position;

        private SpriteSheet TileSheet;

        public Tile(bool isBomb, Vector2 pos, SpriteSheet sheet)
        {
            Number = 0;
            Bomb = isBomb;
            Position = pos;
            TileSheet = sheet;
            Checked = false;
            Flagged = false;

            TileSheet.Scale = new Vector2(TileSize / TileSheet.FrameWidth);
            TileSheet.Position = (Position * TileSize) + SweeperGame.Instance.BoardPosition;
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
                }
            }

            Checked = true;

            if (Bomb)
            {
                SweeperGame.Instance.BombHit(Position);
            }

            if (Number == 0 && !Bomb)
            {
                CheckNeighbors();
            }

            DetermineFrame();
        }

        public void Flag()
        {
            if (Flagged)
            {
                if (Question)
                {
                    Flagged = false;
                    Question = false;
                }
                else
                    Question = true;
            }
            else
            {
                Flagged = true;
            }
            
            DetermineFrame();
        }

        private void CheckNeighbors()
        {
            SweeperGame.Instance.GetNeighbours(Position).ForEach(neighbour =>
            {
                if(!neighbour.Checked && !neighbour.Bomb)
                    neighbour.Check();
            });
        }

        public void Draw(RenderContext context)
        {
            TileSheet.Draw(context);
        }

        public void DetermineFrame()
        {
            if (Checked && Bomb)
            {
                TileSheet.CurrentFrame = 11;
                return;
            }

            if (Question)
            {
                TileSheet.CurrentFrame = 14;
                return;
            }

            if (Bomb && SweeperGame.Instance.GameOver)
            {
                TileSheet.CurrentFrame = 13;
                return;
            }

            if (Flagged && !Bomb && SweeperGame.Instance.GameOver)
            {
                TileSheet.CurrentFrame = 12;
                return;
            }

            if (Flagged)
            {
                TileSheet.CurrentFrame = 10;
                return;
            }

            if (!Checked)
            {
                TileSheet.CurrentFrame = 9;
                return;
            }

            TileSheet.CurrentFrame = Number;
        }

        private int GetNumberToDisplay()
        {
            int surroundingBombs = 0;
            SweeperGame.Instance.GetNeighbours(Position).ForEach(neighbour =>
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
