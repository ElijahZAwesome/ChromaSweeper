using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Chroma.Graphics;
using Chroma.Windowing;

namespace ChromaSweeper
{
    class Board
    {

        public Vector2 BoardSize;
        public int BombAmount;

        public Tile[,] BoardArray;

        public int UncheckedTiles;

        public Board()
        {

        }

        public void InitBoard(Vector2? mousePos = null)
        {
            UncheckedTiles = (int)(BoardSize.X * BoardSize.Y) - BombAmount;
            bool[,] BombArray = new bool[(int)BoardSize.X,(int)BoardSize.Y];
            int bombsLeft = BombAmount;
            while (bombsLeft > 0)
            {
                var randomPos = new Vector2(SweeperGame.Instance.Rand.Next((int)BoardSize.X),
                    SweeperGame.Instance.Rand.Next((int)BoardSize.Y));

                // Bomb on where user pressed
                if (mousePos != null && randomPos == mousePos)
                {
                    continue;
                }

                // Bomb already placed here
                if (BombArray[(int) randomPos.X, (int) randomPos.Y])
                {
                    continue;
                }
                else
                {
                    BombArray[(int) randomPos.X, (int) randomPos.Y] = true;
                    bombsLeft--;
                }
            }

            BoardArray = new Tile[(int)BoardSize.X, (int)BoardSize.Y];
            for (var x = 0; x < BoardArray.GetLength(0); x++)
            {
                for (var y = 0; y < BoardArray.GetLength(1); y++)
                {
                    BoardArray[x, y] = new Tile(BombArray[x, y], new Vector2(x, y), SweeperGame.Instance.TilesSheet.Copy());
                }
            }

            foreach (var tile in BoardArray)
            {
                tile.Init();
            }
        }

        public void Draw(RenderContext context)
        {
            foreach (var tile in BoardArray)
            {
                tile.Draw(context);
            }
        }
    }
}
