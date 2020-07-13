using Chroma.Graphics;
using System.Collections.Generic;
using System.Numerics;

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

            BoardArray = new Tile[(int)BoardSize.X, (int)BoardSize.Y];
            for (var x = 0; x < BoardArray.GetLength(0); x++)
            {
                for (var y = 0; y < BoardArray.GetLength(1); y++)
                {
                    BoardArray[x, y] = new Tile(new Vector2(x, y));
                }
            }

            bool[,] BombArray = new bool[(int)BoardSize.X, (int)BoardSize.Y];
            int bombsLeft = BombAmount;
            while (bombsLeft > 0)
            {
                if (PlaceBombOnBoard(mousePos))
                {
                    bombsLeft--;
                }
            }
        }

        private bool PlaceBombOnBoard(Vector2? avoidPos)
        {
            var randomPos = new Vector2(SweeperGame.Instance.Rand.Next((int)BoardSize.X),
                        SweeperGame.Instance.Rand.Next((int)BoardSize.Y));

            // Bomb on where user pressed
            if (avoidPos != null && randomPos == avoidPos)
            {
                return false;
            }

            // Bomb already placed here
            if (BoardArray[(int)randomPos.X, (int)randomPos.Y].Bomb)
            {
                return false;
            }
            else
            {
                BoardArray[(int)randomPos.X, (int)randomPos.Y].Bomb = true;
                BoardArray[(int)randomPos.X, (int)randomPos.Y].Init();
                return true;
            }
        }

        public void MoveBombAtPos(Vector2? position)
        {
            if (position.HasValue)
            {
                var tile = BoardArray[(int)position.Value.X, (int)position.Value.Y];
                if (tile.Bomb)
                {
                    tile.Bomb = false;
                    while (!PlaceBombOnBoard(position)) { }
                }
            }
        }

        public void InitTiles()
        {
            foreach (var tile in BoardArray)
            {
                tile.Init();
            }
        }

        public void OpenRegion(Vector2 clickPosition)
        {
            var tile = BoardArray[(int)clickPosition.X, (int)clickPosition.Y];
            if (tile.Number == 0 && !tile.Bomb)
            {
                var q = new Queue<Tile>();

                q.Enqueue(tile);

                while (q.Count > 0)
                {
                    var queuedTile = q.Dequeue();
                    queuedTile.Check();
                    SweeperGame.Instance.GetNeighbors(queuedTile.BoardPosition).ForEach(neighbour =>
                    {
                        if (!neighbour.Checked && neighbour.Number == 0 && !neighbour.Bomb && !neighbour.RegionVisited)
                        {
                            neighbour.RegionVisited = true;
                            q.Enqueue(neighbour);
                        }
                        else if (!neighbour.Checked && !neighbour.Bomb && !neighbour.RegionVisited)
                        {
                            neighbour.Check();
                            neighbour.RegionVisited = true;
                        }
                    });
                    //queuedTile.RegionVisited = false;
                }
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
