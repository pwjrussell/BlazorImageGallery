using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestModelsClassLibrary
{
    public class SetAnnotationRequestModel
    {
        public PinOverlayModel[] Pins { get; set; }
        public W3CWebAnnotationModel[] Annotations { get; set; }
    }
}
