using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Services
{
    public class QuillClient
    {
        private readonly IJSRuntime _JSRuntime;

        public QuillClient(IJSRuntime jSRuntime)
        {
            _JSRuntime = jSRuntime;
        }

        public async Task InitAsync(ElementReference toolbarReference, ElementReference editorReference)
        {
            await _JSRuntime.InvokeVoidAsync("QuillClient.initQuill", toolbarReference, editorReference);
        }
        public async Task SetHTMLAsync(ElementReference editorReference, MarkupString markup)
        {
            await _JSRuntime.InvokeVoidAsync("QuillClient.setHTML", editorReference, markup.ToString());
        }
        public async Task<MarkupString> GetHTMLAsync(ElementReference editorReference)
        {
            return (MarkupString)await _JSRuntime.InvokeAsync<string>("QuillClient.getHTML", editorReference);
        }
    }
}
