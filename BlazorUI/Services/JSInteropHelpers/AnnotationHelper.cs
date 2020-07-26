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
        public delegate Task OnPageChanged(int newPage);

        public AnnotationHelper(
            OnAnnotationChanged onAnnotationChangedCallback, 
            OnPageChanged onPageChangedCallback)
        {
            OnAnnotationChangedCallback = onAnnotationChangedCallback;
            OnPageChangedCallback = onPageChangedCallback;
        }

        public OnAnnotationChanged OnAnnotationChangedCallback { get; set; }
        public OnPageChanged OnPageChangedCallback { get; set; }

        [JSInvokable]
        public async Task NotifyAnnotationsChanged(W3CWebAnnotationModel[] annotations)
        {
            await OnAnnotationChangedCallback(annotations);
        }
        [JSInvokable]
        public async Task NotifyPageChangedTo(int newPage)
        {
            await OnPageChangedCallback(newPage);
        }
    }
}
