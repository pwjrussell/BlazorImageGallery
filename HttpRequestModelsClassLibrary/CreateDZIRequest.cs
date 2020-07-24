using System;
using System.IO;

namespace HttpRequestModelsClassLibrary
{
    public class CreateDZIRequest
    {
        public string ImageName { get; set; }
        public Stream ImageStream { get; set; }
        public int TileSize { get; set; }
        public int Overlap { get; set; }
    }
}
