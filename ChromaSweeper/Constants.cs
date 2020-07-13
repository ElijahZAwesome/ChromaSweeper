using Chroma.Graphics;
using System.Numerics;

namespace ChromaSweeper
{
    public static class Constants
    {

        // Strings
        public static string WindowTitle = "Minesweeper";

        // Colors
        public static Color BackgroundColor = new Color(192, 192, 192);
        public static Color LeftRectangleColor = new Color(136, 136, 136);
        public static Color RightRectangleColor = Color.White;

        // UI
        public static Vector2 BoardPos = new Vector2(9, 53);
        public static int BoardBorderThickness = 3;
        public static Vector2 ScoreboardPos = new Vector2(9, 9);
        public static int ScoreboardHeight = 37;
        public static int ScoreboardBorderThickness = 2;
        public static int SmileyY = (int)ScoreboardPos.Y + ScoreboardBorderThickness + 4;
        public static int SmileySize = 24;
        public static int NumbersY = SmileyY;
        public static int NumbersHeight = 24;
        public static int LeftNumberOffset = ScoreboardBorderThickness + 5;
        public static int RightNumberOffset = ScoreboardBorderThickness + 7;

        // Settings
        public static int SettingsPanelOffset = 20;
        public static Color SettingsPanelColor = Color.White;

    }
}
