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
        public delegate Task OnMouseDown(double viewportX, double viewportY);
        public delegate Task OnMouseUp(double viewportX, double viewportY);

        public AnnotationHelper(
            OnAnnotationChanged onAnnotationChangedCallback = null, 
            OnPageChanged onPageChangedCallback = null,
            OnMouseDown onMouseDownCallback = null,
            OnMouseUp onMouseUpCallback = null)
        {
            OnAnnotationChangedCallback = onAnnotationChangedCallback ?? (async(W3CWebAnnotationModel[] annotations) => { });
            OnPageChangedCallback = onPageChangedCallback ?? (async (int newPage) => { });
            OnMouseDownCallback = onMouseDownCallback ?? (async (double viewportX, double viewportY) => { });
            OnMouseUpCallback = onMouseUpCallback ?? (async (double viewportX, double viewportY) => { });
        }

        public OnAnnotationChanged OnAnnotationChangedCallback { get; set; }
        public OnPageChanged OnPageChangedCallback { get; set; }
        public OnMouseDown OnMouseDownCallback { get; set; }
        public OnMouseUp OnMouseUpCallback { get; set; }

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
        [JSInvokable]
        public async Task NotifyMouseDown(double viewportX, double viewportY)
        {
            await OnMouseDownCallback(viewportX, viewportY);
        }
        [JSInvokable]
        public async Task NotifyMouseUp(double viewportX, double viewportY)
        {
            await OnMouseUpCallback(viewportX, viewportY);
        }
    }
}
