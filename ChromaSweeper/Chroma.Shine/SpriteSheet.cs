using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Chroma.Graphics
{
    public class SpriteSheet : Sprite
    {
        private List<Rectangle> _sourceRectangles;
        private int _currentFrame;

        public int FrameWidth { get; }
        public int FrameHeight { get; }

        public int TotalFrames => _sourceRectangles.Count;

        public int CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (WrapFrameIndexOnOverflow)
                    _currentFrame = value % _sourceRectangles.Count;
                else
                    _currentFrame = value;
            }
        }

        public bool WrapFrameIndexOnOverflow { get; set; } = true;

        public SpriteSheet(string filePath, int frameWidth, int frameHeight) : base(filePath)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;

            CalculateFrameRectangles();
        }

        public SpriteSheet(Texture texture, int frameWidth, int frameHeight) : base(texture)
        {
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;

            CalculateFrameRectangles();
        }

        public SpriteSheet Copy()
        {
            var newSheet = new SpriteSheet(Texture, FrameWidth, FrameHeight)
            {
                CurrentFrame = CurrentFrame,
                WrapFrameIndexOnOverflow = WrapFrameIndexOnOverflow,
                Shearing = Shearing,
                Position = Position,
                Scale = Scale,
                Origin = Origin,
                Rotation = Rotation
            };
            return newSheet;
        }

        public override void Draw(RenderContext context)
        {
            if (Shearing != Vector2.Zero)
            {
                context.Transform.Push();
                context.Transform.Shear(Shearing);
            }

            context.DrawTexture(Texture, Position, Scale, Origin, Rotation, _sourceRectangles[CurrentFrame]);

            if (Shearing != Vector2.Zero)
            {
                context.Transform.Pop();
            }
        }

        public void DrawSpecificFrame(RenderContext context, int frame)
        {
            int oldFrame = CurrentFrame;
            CurrentFrame = frame;
            Draw(context);
            CurrentFrame = oldFrame;
        }

        public void DrawManual(RenderContext context, int frame, Vector2 position, Vector2 scale)
        {
            var oldPos = Position;
            var oldScale = Scale;
            Position = position;
            Scale = scale;
            DrawSpecificFrame(context, frame);
            Position = oldPos;
            Scale = oldScale;
        }

        private void CalculateFrameRectangles()
        {
            _sourceRectangles = new List<Rectangle>();

            var totalFramesH = Math.Ceiling((float)Texture.Width / FrameWidth);
            var totalFramesV = Math.Ceiling((float)Texture.Height / FrameHeight);

            for (var y = 0; y < totalFramesV; y++)
            {
                for (var x = 0; x < totalFramesH; x++)
                {
                    _sourceRectangles.Add(
                        new Rectangle(
                            x * FrameWidth,
                            y * FrameHeight,
                            FrameWidth,
                            FrameHeight
                        )
                    );
                }
            }
        }
    }
}