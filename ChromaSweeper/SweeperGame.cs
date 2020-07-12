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
        public float Time;

        public bool GameStarted;
        public bool GameOver;
        public bool GameWon;


        public int ScorebarHeight = 37;


        public SpriteSheet NumbersSheet;
        public SpriteSheet TilesSheet;
        public SpriteSheet FaceSheet;

        private Texture WindowIcon;
        public readonly Random Rand = new Random();
        private Log Log { get; } = LogManager.GetForCurrentAssembly();

        internal SweeperGame()
        {
            Instance = this;

            Board = new Board();

            BoardPosition = Vector2.Zero;
        }

        public void InitBoard(Vector2? mousePos = null)
        {
            Window.Size = new Size((int)Board.BoardSize.X * Tile.TileSize + 20,
                (int)Board.BoardSize.Y * Tile.TileSize + 27 + ScorebarHeight);

            BoardPosition = new Vector2(Constants.BoardPos.X + Constants.BoardBorderThickness, 
                Constants.BoardPos.Y + Constants.BoardBorderThickness);

            FlagsLeft = Board.BombAmount;
            Time = 0;
            Board.InitBoard(mousePos);
        }

        protected override void LoadContent()
        {
            NumbersSheet = new SpriteSheet(Content.ContentRoot + "/numbers.jpg", 13, 23);
            TilesSheet = new SpriteSheet(Content.ContentRoot + "/tiles.jpg", 16, 16);
            FaceSheet = new SpriteSheet(Content.ContentRoot + "/faces.jpg", 26, 26);
            NumbersSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;
            TilesSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;
            FaceSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;

            WindowIcon = Content.Load<Texture>("icon.png");
            Window.SetIcon(WindowIcon);

            Board.BoardSize = new Vector2(9, 9);
            Board.BombAmount = 10;
            InitBoard();
        }

        protected override void Draw(RenderContext context)
        {
            context.Clear(Constants.BackgroundColor);
            // Frame rectangle
            DrawHUDRectangle(context, new Rectangle(0, 0, Window.Size.Width + 3, Window.Size.Height + 3), 3, true);

            // Board rectangle
            DrawHUDRectangle(context,
                new Rectangle((int) Constants.BoardPos.X, (int) Constants.BoardPos.Y, 
                (int) ((Board.BoardSize.X * Tile.TileSize) + (Constants.BoardBorderThickness * 2)), 
                (int) ((Board.BoardSize.Y * Tile.TileSize) + (Constants.BoardBorderThickness * 2))), 
                Constants.BoardBorderThickness, false);

            // Scoreboard
            DrawHUDRectangle(context,
                new Rectangle((int) Constants.ScoreboardPos.X, (int) Constants.ScoreboardPos.Y, 
                    Window.Size.Width - (int)Constants.ScoreboardPos.X - 5 + Constants.ScoreboardBorderThickness,
                    45), Constants.ScoreboardBorderThickness, false);

            Board.Draw(context);
        }

        /// <summary>
        /// Draws a rectangle used for HUD elements.
        /// </summary>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="lineWidth">How thicc the line should be</param>
        /// <param name="drawRight">Do or don't draw the right part</param>
        private void DrawHUDRectangle(RenderContext context, Rectangle rect, int lineWidth, bool white)
        {
            float oldThickness = Graphics.LineThickness;
            Graphics.LineThickness = 1;

            // Top and left lines
            Color colorToDraw = white ? Constants.RightRectangleColor : Constants.LeftRectangleColor;
            for (int i = 0; i < lineWidth; i++)
            {
                context.Line(new Vector2(rect.X, rect.Y + i), 
                    new Vector2(rect.X + rect.Width - i, rect.Y + i), 
                    colorToDraw);

                context.Line(new Vector2(rect.X + i, rect.Y), 
                    new Vector2(rect.X + i, rect.Y + rect.Height - i), 
                    colorToDraw);
            }

            if (!white)
            {
                // Right and bottom lines
                for (int i = 0; i < lineWidth; i++)
                {
                    context.Line(new Vector2(rect.X + rect.Width - i, rect.Y + i), 
                        new Vector2(rect.X + rect.Width - i, rect.Y + rect.Height - i), 
                        Constants.RightRectangleColor);

                    context.Line(new Vector2(rect.X + rect.Width, rect.Y + rect.Height - i), 
                        new Vector2(rect.X + i, rect.Y + rect.Height - i), 
                        Constants.RightRectangleColor);
                }
            }

            Graphics.LineThickness = oldThickness;
        }

        public void BombHit(Vector2 hitPosition)
        {
            GameOver = true;

            foreach (var tile in Board.BoardArray)
            {
                tile.DetermineFrame();
            }
        }

        public void Victory()
        {
            Log.Info("You won dipshit");
            GameWon = true;

            foreach (var tile in Board.BoardArray)
            {
                if ((!tile.Flagged || tile.Question) && tile.Bomb)
                {
                    tile.Flagged = true;
                    tile.Question = false;
                    tile.DetermineFrame();
                }
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
            if (GameOver || GameWon)
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
