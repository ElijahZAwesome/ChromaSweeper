using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Chroma.Graphics;

namespace ChromaSweeper
{
    class NumberGroup
    {

        private int number;
        private Vector2 position;

        private SpriteSheet sheetNumOne;
        private SpriteSheet sheetNumTwo;
        private SpriteSheet sheetNumThree;

        public NumberGroup(Vector2 pos, SpriteSheet spriteSheet)
        {
            position = pos;
            sheetNumOne = spriteSheet.Copy();
            sheetNumTwo = spriteSheet.Copy();
            sheetNumThree = spriteSheet.Copy();
            sheetNumOne.Position = pos;
            sheetNumTwo.Position = new Vector2(pos.X + sheetNumTwo.FrameWidth, pos.Y);
            sheetNumThree.Position = new Vector2(pos.X + (sheetNumTwo.FrameWidth * 2), pos.Y);
        }

        public void UpdateNumber(int num)
        {
            number = num;

            int lowDigit;
            int highNum;

            if (num >= 0) {
                lowDigit = num / 100;
                highNum = num % 100;
            }
            else {
                lowDigit = 10;
                highNum = Math.Abs(num);
            }

            sheetNumOne.CurrentFrame = lowDigit;
            sheetNumTwo.CurrentFrame = highNum / 10;
            sheetNumThree.CurrentFrame = highNum % 10;
        }

        public void Draw(RenderContext context)
        {
            sheetNumOne.Draw(context);
            sheetNumTwo.Draw(context);
            sheetNumThree.Draw(context);
        }

    }
}
