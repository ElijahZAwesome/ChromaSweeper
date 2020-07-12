using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Chroma.Graphics;

namespace ChromaSweeper
{
    public static class Constants
    {

        // Colors
        public static Color BackgroundColor = new Color(192, 192, 192);
        public static Color LeftRectangleColor = new Color(136, 136, 136);
        public static Color RightRectangleColor = Color.White;

        // UI
        public static Vector2 BoardPos = new Vector2(9, 52);
        public static int BoardBorderThickness = 3;
        public static Vector2 ScoreboardPos = new Vector2(9, 9);
        public static int ScoreboardBorderThickness = 2;

    }
}
