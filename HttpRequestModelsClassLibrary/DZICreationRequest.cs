using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpRequestModelsClassLibrary
{
    public class DZICreationRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int TileSize { get; set; }
        public int Overlap { get; set; }
        public Stream ImageStream { get; set; }
    }
}
