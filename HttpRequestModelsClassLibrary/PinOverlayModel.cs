using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestModelsClassLibrary
{
    public class PinOverlayModel
    {
        public PinOverlayModel()
        {

        }
        public PinOverlayModel(string text, double x, double y, ColorEnum color = ColorEnum.Black)
        {
            Text = text;
            X = x;
            Y = y;
            Color = color;
        }
        public enum ColorEnum
        {
            Black,
            Red,
            Yellow,
            Cyan,
            Blue,
            Magenta
        }

        /// <summary>
        /// The text of the overlay.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The x-coordinate (in viewport coordinates) of the overlay.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// The y-ccoridante (in viewport coordinates) of the overlay.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// The color of the pin.
        /// </summary>
        public ColorEnum Color { get; set; }
    }
}
