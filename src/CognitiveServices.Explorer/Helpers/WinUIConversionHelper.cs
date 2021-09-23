using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
using WinRT;

namespace CognitiveServices.Explorer.Helpers
{
    public static class WinUIConversionHelper
    {
        public static void InitFileOpenPicker(FileOpenPicker picker)
        {
            if (Window.Current == null)
            {
                IInitializeWithWindow initializeWithWindowWrapper = picker.As<IInitializeWithWindow>();
                IntPtr hwnd = GetActiveWindow();
                initializeWithWindowWrapper.Initialize(hwnd);
            }
        }

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();
    }
}
