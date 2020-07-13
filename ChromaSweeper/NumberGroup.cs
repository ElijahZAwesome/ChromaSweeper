using Chroma.Graphics;
using System;
using System.Numerics;

namespace ChromaSweeper
{
    class NumberGroup
    {

        private int number;
        private Vector2 position;

        private int firstDigitFrame;
        private Vector2 firstDigitPosition;
        private int secondDigitFrame;
        private Vector2 secondDigitPosition;
        private int thirdDigitFrame;
        private Vector2 thirdDigitPosition;

        public NumberGroup(Vector2 pos)
        {
            position = pos;
            thirdDigitPosition = pos;
            secondDigitPosition = new Vector2(pos.X + SweeperGame.Instance.NumbersSheet.FrameWidth, pos.Y);
            firstDigitPosition = new Vector2(pos.X + (SweeperGame.Instance.NumbersSheet.FrameWidth * 2), pos.Y);
        }

        public void UpdateNumber(int num)
        {
            number = num;

            int lowDigit;
            int highNum;

            if (num >= 0)
            {
                lowDigit = num / 100;
                highNum = num % 100;
            }
            else
            {
                lowDigit = 10;
                highNum = Math.Abs(num);
            }

            thirdDigitFrame = lowDigit;
            secondDigitFrame = highNum / 10;
            firstDigitFrame = highNum % 10;
        }

        public void Draw(RenderContext context)
        {
            SweeperGame.Instance.NumbersSheet.DrawManual(context, thirdDigitFrame, thirdDigitPosition, Vector2.One);
            SweeperGame.Instance.NumbersSheet.DrawManual(context, secondDigitFrame, secondDigitPosition, Vector2.One);
            SweeperGame.Instance.NumbersSheet.DrawManual(context, firstDigitFrame, firstDigitPosition, Vector2.One);
        }

    }
}
