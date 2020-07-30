using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageToolsClassLibrary
{
    public class TileModel
    {
        public int Level { get; set; }
        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public Rectangle TileRect { get; set; }
    }
}
