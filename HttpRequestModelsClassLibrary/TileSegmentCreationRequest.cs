using ImageToolsClassLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestModelsClassLibrary
{
    public class TileSegmentCreationRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string FileExtension { get; set; }
        public TileModel[] Tiles { get; set; }
    }
}
