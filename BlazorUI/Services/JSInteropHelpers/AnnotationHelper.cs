using HttpRequestModelsClassLibrary;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Services.JSInteropHelpers
{
    public class AnnotationHelper
    {
        public delegate Task OnAnnotationChanged(W3CWebAnnotationModel[] annotations);

        public AnnotationHelper(OnAnnotationChanged onAnnotationChangedCallback)
        {
            OnAnnotationChangedCallback = onAnnotationChangedCallback;
        }

        public OnAnnotationChanged OnAnnotationChangedCallback { get; set; }

        [JSInvokable]
        public async Task NotifyAnnotationsChanged(W3CWebAnnotationModel[] annotations)
        {
            await OnAnnotationChangedCallback(annotations);
        }
    }
}
