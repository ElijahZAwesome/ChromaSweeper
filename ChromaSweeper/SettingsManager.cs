﻿using Chroma.Graphics;
using Chroma.Input.EventArgs;
using Chroma.UI.Controls;
using System;
using System.Drawing;
using System.Numerics;
using Color = Chroma.Graphics.Color;

namespace ChromaSweeper
{
    class SettingsManager
    {

        public bool Shown;

        private Panel bgPanel;
        private InputField boardWidthField;
        private InputField boardHeightField;
        private InputField bombAmountField;
        private InputField seedField;
        private Button cancelBtn;
        private Button acceptBtn;

        public SettingsManager(Size windowSize)
        {
            Init(windowSize);
        }

        public void Init(Size windowSize)
        {
            Dispose();
            bgPanel = new Panel(new Vector2(Constants.SettingsPanelOffset),
                new Vector2(windowSize.Width - Constants.SettingsPanelOffset * 2, windowSize.Height - Constants.SettingsPanelOffset * 2),
                Constants.SettingsPanelColor)
            {
                BorderColor = Color.Black,
                BorderThickness = 1
            };

            cancelBtn = new Button(bgPanel.CalculatedPosition + new Vector2(bgPanel.CalculatedSize.X / 2, bgPanel.CalculatedSize.Y - 44))
            {
                Text = "Cancel",
                Origin = new Vector2(40, 12),
            };
            cancelBtn.ButtonPressed += Cancel;
            acceptBtn = new Button(bgPanel.CalculatedPosition + new Vector2(bgPanel.CalculatedSize.X / 2, bgPanel.CalculatedSize.Y - 16))
            {
                Text = "OK",
                Origin = new Vector2(40, 12),
            };
            acceptBtn.ButtonPressed += Accept;

            boardWidthField = new InputField(new Vector2(windowSize.Width / 2 - 60,
                bgPanel.CalculatedPosition.Y + bgPanel.CalculatedSize.Y / 2 - 25 - cancelBtn.CalculatedSize.Y * 2))
            {
                AllowOverflow = false,
                Filter = "^[0-9]*$",
                SizeLimit = 3,
                Placeholder = "Width: " + SweeperGame.Instance.Board.BoardSize.X
            };

            boardHeightField = new InputField(new Vector2(windowSize.Width / 2 - 60,
                bgPanel.CalculatedPosition.Y + bgPanel.CalculatedSize.Y / 2 - cancelBtn.CalculatedSize.Y * 2))
            {
                AllowOverflow = false,
                Filter = "^[0-9]*$",
                SizeLimit = 3,
                Placeholder = "Height: " + SweeperGame.Instance.Board.BoardSize.Y
            };

            bombAmountField = new InputField(new Vector2(windowSize.Width / 2 - 60, boardHeightField.CalculatedPosition.Y + 25))
            {
                AllowOverflow = false,
                Filter = "^[0-9]*$",
                SizeLimit = 3,
                Placeholder = "Bombs: " + SweeperGame.Instance.Board.BombAmount
            };

            seedField = new InputField(new Vector2(windowSize.Width / 2 - 60, bombAmountField.CalculatedPosition.Y + 25))
            {
                AllowOverflow = false,
                Filter = "^[0-9]*$",
                SizeLimit = 9,
                Placeholder = "RNG Seed"
            };
        }

        public void Show()
        {
            Shown = true;
        }

        public void Hide()
        {
            Shown = false;
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private void Cancel(object? sender, EventArgs e)
        {
            Hide();
        }

        private void Accept(object? sender, EventArgs e)
        {
            Hide();

            if (string.IsNullOrEmpty(boardWidthField.Text))
            {
                boardWidthField.Text = SweeperGame.Instance.Board.BoardSize.X.ToString();
            }
            if (string.IsNullOrEmpty(boardHeightField.Text))
            {
                boardHeightField.Text = SweeperGame.Instance.Board.BoardSize.Y.ToString();
            }
            if (string.IsNullOrEmpty(bombAmountField.Text))
            {
                bombAmountField.Text = SweeperGame.Instance.Board.BombAmount.ToString();
            }

            int newWidth = int.Parse(boardWidthField.Text);
            if (newWidth < 9)
                newWidth = 9;

            int newHeight = int.Parse(boardHeightField.Text);
            if (newHeight < 9)
                newHeight = 9;

            int newBombAmount = int.Parse(bombAmountField.Text);
            if (newBombAmount > (newWidth * newHeight) - 1)
                newBombAmount = (newWidth * newHeight) - 1;

            SweeperGame.Instance.Board.BoardSize = new Vector2(newWidth, newHeight);
            SweeperGame.Instance.Board.BombAmount = newBombAmount;

            int newSeed;
            if (!int.TryParse(seedField.Text, out newSeed))
            {
                newSeed = new Random().Next();
            }
            SweeperGame.Instance.Rand = new Random(newSeed);
            SweeperGame.Instance.InitBoard();
        }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public void Draw(RenderContext context, GraphicsManager gfx)
        {
            if (Shown)
            {
                bgPanel.Draw(context, gfx);
                boardWidthField.Draw(context, gfx);
                boardHeightField.Draw(context, gfx);
                bombAmountField.Draw(context, gfx);
                seedField.Draw(context, gfx);
                cancelBtn.Draw(context, gfx);
                acceptBtn.Draw(context, gfx);
            }
        }

        public void Update(float delta)
        {
            if (Shown)
            {
                bgPanel.Update(delta);
                boardWidthField.Update(delta);
                boardHeightField.Update(delta);
                bombAmountField.Update(delta);
                seedField.Update(delta);
                cancelBtn.Update(delta);
                acceptBtn.Update(delta);
            }
        }

        public void KeyPressed(KeyEventArgs e)
        {
            if (Shown)
            {
                boardWidthField.KeyPressed(e);
                boardHeightField.KeyPressed(e);
                bombAmountField.KeyPressed(e);
                seedField.KeyPressed(e);
            }
        }

        public void TextInput(TextInputEventArgs e)
        {
            if (Shown)
            {
                boardWidthField.TextInput(e);
                boardHeightField.TextInput(e);
                bombAmountField.TextInput(e);
                seedField.TextInput(e);
            }
        }

        public void Dispose()
        {
            if (bgPanel != null)
                bgPanel.Dispose();
            if (boardWidthField != null)
                boardWidthField.Dispose();
            if (boardHeightField != null)
                boardHeightField.Dispose();
            if (bombAmountField != null)
                bombAmountField.Dispose();
            if (seedField != null)
                seedField.Dispose();
            if (cancelBtn != null)
                cancelBtn.Dispose();
            if (acceptBtn != null)
                acceptBtn.Dispose();
        }

    }
}
