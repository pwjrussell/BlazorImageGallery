using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Services
{
    public class UIService
    {
        private readonly ISyncLocalStorageService _localStorage;
        public UIService(ISyncLocalStorageService storage)
        {
            _localStorage = storage;
        }

        public bool NightMode 
        { 
            get { return _localStorage.GetItem<bool>("nightMode"); } 
            set { _localStorage.SetItem("nightMode", value); } 
        }

        public void ToggleNightMode()
        {
            NightMode = !NightMode;
        }
    }
}
