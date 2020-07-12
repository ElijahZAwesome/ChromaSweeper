using System;
using Chroma.Graphics;

namespace ChromaSweeper
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            GraphicsManager.MultiSamplingPrecision = 0;

            new SweeperGame().Run();
        }
    }
}
