using System;
using System.Drawing;

namespace ImageToolsClassLibrary
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Returns a new Bitmap object consisting of the source bitmap scaled by the factor specified.
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to be scaled.</param>
        /// <param name="factor">The factor by which the source bitmap will be scaled.</param>
        /// <param name="midpointRounding">The rounding options to use when determining the new width and height of the bitmap.</param>
        /// <returns>The source bitmap, scaled by the factor specified.</returns>
        public static Bitmap ScaleBy(
            this Bitmap sourceBitmap, double factor, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            int newWidth = (int)Math.Round(factor * sourceBitmap.Width, midpointRounding);
            int newHeight = (int)Math.Round(factor * sourceBitmap.Height, midpointRounding);

            return new Bitmap(sourceBitmap, newWidth, newHeight);
        }
    }
}
