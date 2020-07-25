using BlazorUI.Services.JSInteropHelpers;
using HttpRequestModelsClassLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Services
{
    public class OpenSeadragonClient : IDisposable
    {
        private readonly IJSRuntime _JsRuntime;

        private DotNetObjectReference<AnnotationHelper> objRef;

        public delegate Task OnAnnotationsChanged(W3CWebAnnotationModel[] annotations);
        public event OnAnnotationsChanged AnnotationsChanged;
        public OpenSeadragonClient(IJSRuntime jSRuntime)
        {
            _JsRuntime = jSRuntime;
        }

        public async Task InitAsync(ElementReference viewerReference, string[] tileSourcePaths, string[] annotationPaths)
        {
            objRef = DotNetObjectReference.Create(new AnnotationHelper(OnAnnotationsChangedCallback));
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.initDZI", 
                viewerReference, tileSourcePaths, annotationPaths, objRef);
        }

        private async Task OnAnnotationsChangedCallback(W3CWebAnnotationModel[] annotations)
        {
            await AnnotationsChanged(annotations);
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
