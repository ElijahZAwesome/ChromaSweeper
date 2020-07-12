using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Chroma;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Input;
using Chroma.Input.EventArgs;
using Chroma.UI.Controls;
using Color = Chroma.Graphics.Color;

namespace ChromaSweeper
{
    internal class SweeperGame : Game
    {
        public static SweeperGame Instance;

        public Board Board;
        public Vector2 BoardPosition;

        public int FlagsLeft;
        public int Score;

        public bool GameStarted = false;
        public bool GameOver = false;


        public int ScorebarHeight = 37;

        // UI
        public Panel BoardPanel;


        public SpriteSheet NumbersSheet;
        public SpriteSheet TilesSheet;
        public SpriteSheet FaceSheet;

        private Texture WindowIcon;
        public readonly Random Rand = new Random();
        private Log Log { get; } = LogManager.GetForCurrentAssembly();

        internal SweeperGame()
        {
            Instance = this;

            BoardPanel = new Panel(BoardPosition, );

            Board = new Board();

            BoardPosition = Vector2.Zero;
        }

        public void InitBoard(Vector2? mousePos = null)
        {
            Window.Size = new Size((int)Board.BoardSize.X * Tile.TileSize + 20,
                (int)Board.BoardSize.Y * Tile.TileSize + 27 + ScorebarHeight);

            BoardPosition = new Vector2(12, 19 + ScorebarHeight);

            Board.InitBoard(mousePos);
        }

        protected override void LoadContent()
        {
            NumbersSheet = new SpriteSheet(Content.ContentRoot + "/numbers.jpg", 13, 23);
            TilesSheet = new SpriteSheet(Content.ContentRoot + "/tiles.jpg", 16, 16);
            FaceSheet = new SpriteSheet(Content.ContentRoot + "/faces.jpg", 26, 26);

            WindowIcon = Content.Load<Texture>("icon.png");
            Window.SetIcon(WindowIcon);

            Board.BoardSize = new Vector2(9, 9);
            Board.BombAmount = 10;
            InitBoard();
        }

        protected override void Draw(RenderContext context)
        {
            context.Clear(new Color(192, 192, 192));

            Board.Draw(context);
        }

        public void BombHit(Vector2 hitPosition)
        {
            GameOver = true;

            foreach (var tile in Board.BoardArray)
            {
                tile.DetermineFrame();
            }
        }

        public List<Tile> GetNeighbours(Vector2 pos)
        {
            List<Tile> final = new List<Tile>();
            for (int dx = (pos.X > 0 ? -1 : 0); dx <= (pos.X < Board.BoardSize.X - 1 ? 1 : 0); ++dx)
            {
                for (int dy = (pos.Y > 0 ? -1 : 0); dy <= (pos.Y < Board.BoardSize.Y - 1 ? 1 : 0); ++dy)
                {
                    if (dx != 0 || dy != 0)
                    {
                        var neighbor = Board.BoardArray[(int) (pos.X + dx), (int) (pos.Y + dy)];
                        final.Add(neighbor);
                    }
                }
            }

            return final;
        }

        protected override void MouseReleased(MouseButtonEventArgs e)
        {
            Vector2 relPosition = e.Position - BoardPosition;
            Vector2 tilePosition = new Vector2((float)Math.Floor(relPosition.X / 16), (float)Math.Floor(relPosition.Y / 16));
            if (!GameStarted)
            {
                GameStarted = true;
                InitBoard(tilePosition);
            }
            if (GameOver)
                return;
            if (tilePosition.X >= 0 && tilePosition.Y >= 0)
            {
                if (tilePosition.X < Board.BoardSize.X && tilePosition.Y < Board.BoardSize.Y)
                {
                    Tile clickedTile = Board.BoardArray[(int)tilePosition.X, (int)tilePosition.Y];
                    switch (e.Button)
                    {
                        case MouseButton.Left:
                            clickedTile.Check();
                            break;
                        case MouseButton.Right:
                            clickedTile.Flag();
                            break;
                    }
                }
            }
        }
    }
}
