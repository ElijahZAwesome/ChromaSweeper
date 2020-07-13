using Chroma;
using Chroma.Graphics;
using Chroma.Input;
using Chroma.Input.EventArgs;
using Chroma.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Color = Chroma.Graphics.Color;

namespace ChromaSweeper
{
    internal class SweeperGame : Game
    {
        public static SweeperGame Instance;
        public readonly Random Rand = new Random();

        public Board Board;
        public Vector2 BoardPosition;

        public int FlagsLeft;
        public float Time;

        public bool GameStarted;
        public bool GameOver;
        public bool GameWon;

        public SpriteSheet NumbersSheet;
        public SpriteSheet TilesSheet;
        public SpriteSheet FaceSheet;
        public NumberGroup LeftNumbers;
        public NumberGroup RightNumbers;

        private SettingsManager settingsManager;

        private Texture WindowIcon;

        private Tile previouslyHeldTile;

        internal SweeperGame()
        {
            Instance = this;

            Board = new Board();

            BoardPosition = Vector2.Zero;
        }

        public void InitBoard()
        {
            Window.Size = new Size((int)Board.BoardSize.X * Tile.TileSize + 20,
                (int)Board.BoardSize.Y * Tile.TileSize + 27 + Constants.ScoreboardHeight);
            if(settingsManager != null)
                settingsManager.Init(Window.Size);
            else
                settingsManager = new SettingsManager(Window.Size);

            BoardPosition = new Vector2(Constants.BoardPos.X + Constants.BoardBorderThickness,
                Constants.BoardPos.Y + Constants.BoardBorderThickness);

            FaceSheet.Position = new Vector2(Window.Center.X - (Constants.SmileySize / 2), Constants.SmileyY);
            LeftNumbers = new NumberGroup(new Vector2(Constants.ScoreboardPos.X + Constants.LeftNumberOffset + 1, Constants.NumbersY + 1));
            int numbersWidth = NumbersSheet.FrameWidth * 3 + 2;
            RightNumbers = new NumberGroup(new Vector2(
                (int)(Constants.ScoreboardPos.X + (Window.Size.Width - (int)Constants.ScoreboardPos.X - 5) - numbersWidth - Constants.RightNumberOffset) + 1,
                Constants.NumbersY + 1));

            FaceSheet.CurrentFrame = (int)SmileyFaces.Smile;

            GameOver = false;
            GameWon = false;
            GameStarted = false;
            FlagsLeft = Board.BombAmount;
            Time = 0;
            LeftNumbers.UpdateNumber(FlagsLeft);
            RightNumbers.UpdateNumber((int)Math.Floor(Time));
            Board.InitBoard();
        }

        protected override void LoadContent()
        {
            new UiContentLoader(Content).LoadUiContent();

            NumbersSheet = new SpriteSheet(Content.ContentRoot + "/numbers.jpg", 13, 23);
            TilesSheet = new SpriteSheet(Content.ContentRoot + "/tiles.jpg", 16, 16);
            FaceSheet = new SpriteSheet(Content.ContentRoot + "/faces.jpg", 26, 26);
            NumbersSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;
            TilesSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;
            FaceSheet.TextureFilteringMode = TextureFilteringMode.NearestNeighbor;

            WindowIcon = Content.Load<Texture>("Minesweeper.ico");
            Window.SetIcon(WindowIcon);
            Window.Title = Constants.WindowTitle;

            Board.BoardSize = new Vector2(9, 9);
            Board.BombAmount = 10;
            InitBoard();
        }

        protected override void Draw(RenderContext context)
        {
            context.Clear(Constants.BackgroundColor);
            // Frame rectangle
            DrawHudRectangle(context, new Rectangle(0, 0, Window.Size.Width + 3, Window.Size.Height + 3), 3, 1);

            // Board rectangle
            DrawHudRectangle(context,
                new Rectangle((int)Constants.BoardPos.X, (int)Constants.BoardPos.Y,
                    Window.Size.Width - (int)Constants.BoardPos.X - 5,
                (int)((Board.BoardSize.Y * Tile.TileSize) + (Constants.BoardBorderThickness * 2))),
                Constants.BoardBorderThickness, 0);

            // Scoreboard rectangle
            DrawHudRectangle(context,
                new Rectangle((int)Constants.ScoreboardPos.X, (int)Constants.ScoreboardPos.Y,
                    Window.Size.Width - (int)Constants.ScoreboardPos.X - 5,
                    Constants.ScoreboardHeight), Constants.ScoreboardBorderThickness, 0);

            // Smiley Rectangle
            DrawHudRectangle(context,
                new Rectangle((int)Window.Center.X - ((Constants.SmileySize + 1) / 2), Constants.SmileyY, Constants.SmileySize + 1, Constants.SmileySize + 1),
                1, 2);

            int numbersWidth = NumbersSheet.FrameWidth * 3 + 2;

            // Left Numbers rectangle
            DrawHudRectangle(context,
                new Rectangle((int)(Constants.ScoreboardPos.X + Constants.LeftNumberOffset), Constants.NumbersY,
                    numbersWidth, NumbersSheet.FrameHeight + 2), 1, 0);

            // Right Numbers rectangle
            DrawHudRectangle(context,
                new Rectangle((int)(Constants.ScoreboardPos.X + (Window.Size.Width - (int)Constants.ScoreboardPos.X - 5) - numbersWidth - Constants.RightNumberOffset),
                    Constants.NumbersY,
                    numbersWidth, NumbersSheet.FrameHeight + 2), 1, 0);

            FaceSheet.Draw(context);
            LeftNumbers.Draw(context);
            RightNumbers.Draw(context);

            Board.Draw(context);

            if (settingsManager.Shown)
            {
                settingsManager.Draw(context, Graphics);
            }
        }

        /// <summary>
        /// Draws a rectangle used for HUD elements.
        /// </summary>
        /// <param name="context">RenderContext to draw onto</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="lineWidth">How thicc the line should be</param>
        /// <param name="whiteOrCopyPen">0 = normal colors, 1 = white and right half isn't drawn, 2 = both halves are dark</param>
        private void DrawHudRectangle(RenderContext context, Rectangle rect, int lineWidth, int whiteOrCopyPen)
        {
            float oldThickness = Graphics.LineThickness;
            Graphics.LineThickness = 1;

            rect.X++;
            rect.Width--;

            // Top and left lines
            Color colorToDraw = whiteOrCopyPen == 1 ? Constants.RightRectangleColor : Constants.LeftRectangleColor;
            for (int i = 0; i < lineWidth; i++)
            {
                context.Line(new Vector2(rect.X, rect.Y + i),
                    new Vector2(rect.X + rect.Width - i - 1, rect.Y + i),
                    colorToDraw);

                context.Line(new Vector2(rect.X + i, rect.Y),
                    new Vector2(rect.X + i, rect.Y + rect.Height - i - 1),
                    colorToDraw);
            }

            if (whiteOrCopyPen != 1)
            {
                // Right and bottom lines
                Color newColorToDraw = whiteOrCopyPen < 2 ? Constants.RightRectangleColor : Constants.LeftRectangleColor;
                for (int i = 0; i < lineWidth; i++)
                {
                    context.Line(new Vector2(rect.X + rect.Width - i, rect.Y + i + 1),
                        new Vector2(rect.X + rect.Width - i, rect.Y + rect.Height),
                        newColorToDraw);

                    context.Line(new Vector2(rect.X + rect.Width, rect.Y + rect.Height - i - 1),
                        new Vector2(rect.X + i, rect.Y + rect.Height - i),
                        newColorToDraw);
                }
            }

            Graphics.LineThickness = oldThickness;
        }

        public void BombHit(Vector2 hitPosition)
        {
            GameOver = true;
            FaceSheet.CurrentFrame = (int)SmileyFaces.Dead;

            foreach (var tile in Board.BoardArray)
            {
                tile.DetermineFrame();
            }
        }

        public void Victory()
        {
            GameWon = true;
            FaceSheet.CurrentFrame = (int)SmileyFaces.Victory;

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

        public List<Tile> GetNeighbors(Vector2 pos)
        {
            List<Tile> final = new List<Tile>();
            for (int dx = (pos.X > 0 ? -1 : 0); dx <= (pos.X < Board.BoardSize.X - 1 ? 1 : 0); ++dx)
            {
                for (int dy = (pos.Y > 0 ? -1 : 0); dy <= (pos.Y < Board.BoardSize.Y - 1 ? 1 : 0); ++dy)
                {
                    if (dx != 0 || dy != 0)
                    {
                        var neighbor = Board.BoardArray[(int)(pos.X + dx), (int)(pos.Y + dy)];
                        final.Add(neighbor);
                    }
                }
            }

            return final;
        }

        protected override void Update(float delta)
        {
            if (settingsManager.Shown)
            {
                settingsManager.Update(delta);
                return;
            }

            if (!GameOver && !GameWon && GameStarted)
            {
                Time += delta;
                RightNumbers.UpdateNumber((int)Math.Floor(Time));
            }

            if (previouslyHeldTile != null)
            {
                previouslyHeldTile.BeingHeld = false;
                previouslyHeldTile = null;
            }

            if (Mouse.IsButtonDown(MouseButton.Left) && !GameOver && !GameWon)
            {
                FaceSheet.CurrentFrame = (int)SmileyFaces.Anticipation;
                var heldTile = GetBoardPosFromMousePos(Mouse.GetPosition());
                if (heldTile.HasValue)
                {
                    previouslyHeldTile = Board.BoardArray[(int)heldTile.Value.X, (int)heldTile.Value.Y];
                    if (!previouslyHeldTile.Checked)
                        previouslyHeldTile.BeingHeld = true;
                }
            }

            if (Mouse.IsButtonDown(MouseButton.Left))
            {
                if (IsMouseOverlappingSmiley())
                {
                    FaceSheet.CurrentFrame = (int)SmileyFaces.PressedSmile;
                }
                else
                {
                    if (GameOver)
                    {
                        FaceSheet.CurrentFrame = (int)SmileyFaces.Dead;
                    }
                    else if (GameWon)
                    {
                        FaceSheet.CurrentFrame = (int)SmileyFaces.Victory;
                    }
                }
            }
        }

        protected override void MouseReleased(MouseButtonEventArgs e)
        {
            if (settingsManager.Shown)
                return;

            // Smiley Logic
            if (!GameWon && !GameOver)
                FaceSheet.CurrentFrame = (int)SmileyFaces.Smile;

            if (IsMouseOverlappingSmiley())
            {
                InitBoard();
                return;
            }

            // Board logic
            Vector2? tilePosition = GetBoardPosFromMousePos(e.Position);
            if (!GameStarted)
            {
                Board.MoveBombAtPos(tilePosition);
                Board.InitTiles();
                GameStarted = true;
            }
            if (GameOver || GameWon)
                return;
            if (tilePosition.HasValue)
            {
                Tile clickedTile = Board.BoardArray[(int)tilePosition.Value.X, (int)tilePosition.Value.Y];
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

        private Vector2? GetBoardPosFromMousePos(Vector2 mousePos)
        {
            Vector2 relPosition = mousePos - BoardPosition;
            Vector2 tilePosition = new Vector2((float)Math.Floor(relPosition.X / 16), (float)Math.Floor(relPosition.Y / 16));
            if (tilePosition.X < 0 || tilePosition.X >= Board.BoardSize.X ||
                tilePosition.Y < 0 || tilePosition.Y >= Board.BoardSize.Y)
            {
                return null;
            }
            return tilePosition;
        }

        private bool IsMouseOverlappingSmiley()
        {
            var mousePos = Mouse.GetPosition();
            if (mousePos.X > FaceSheet.Position.X && mousePos.X < FaceSheet.Position.X + FaceSheet.FrameWidth &&
                mousePos.Y > FaceSheet.Position.Y && mousePos.Y < FaceSheet.Position.Y + FaceSheet.FrameHeight)
            {
                return true;
            }
            return false;
        }

        protected override void KeyPressed(KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.Escape)
            {
                if (!settingsManager.Shown)
                {
                    settingsManager.Show();
                }
                else
                {
                    settingsManager.Hide();
                }
            }

            settingsManager.KeyPressed(e);
        }

        protected override void TextInput(TextInputEventArgs e)
        {
            settingsManager.TextInput(e);
        }
    }
}
