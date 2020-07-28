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

        public event AnnotationHelper.OnPageChanged PageChanged;
        public event AnnotationHelper.OnAnnotationChanged AnnotationsChanged;
        public OpenSeadragonClient(IJSRuntime jSRuntime)
        {
            _JsRuntime = jSRuntime;
        }

        public async Task InitAsync(
            ElementReference viewerReference, string[] tileSourcePaths, string[] annotationPaths, bool isReadonly)
        {
            objRef = DotNetObjectReference.Create(new AnnotationHelper(OnAnnotationsChangedCallback, OnPageChangedCallback));
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.initDZI", 
                viewerReference, tileSourcePaths, annotationPaths, isReadonly, objRef);
        }
        public async Task<bool> GetIfViewerExists()
        {
            return await _JsRuntime.InvokeAsync<bool>("OpenSeadragonClient.getIfViewerExists");
        }
        public async Task DestroyAsync()
        {
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.destroy");
        }

        public async Task PanTo(double x, double y)
        {
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.panTo", x, y);
        }
        public async Task PanTo(string id)
        {
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.annoPanTo", id);
        }
        public async Task GoToPage(int index)
        {
            await _JsRuntime.InvokeVoidAsync("OpenSeadragonClient.goToPage", index);
        }

        private async Task OnAnnotationsChangedCallback(W3CWebAnnotationModel[] annotations)
        {
            await AnnotationsChanged?.Invoke(annotations ?? new W3CWebAnnotationModel[0]);
        }
        private async Task OnPageChangedCallback(int newPage)
        {
            await PageChanged(newPage);
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
