using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class IOService
    {
        private StorageFolder _storageFolder;

        string DependencePath = string.Empty;
        string ModelsPath = string.Empty;

        public IOService(StorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
            DependencePath = Path.Combine(_storageFolder.Path, "dependence");
            ModelsPath = Path.Combine(_storageFolder.Path, "models");
            if (!Directory.Exists(DependencePath))
                Directory.CreateDirectory(DependencePath);
            if (!Directory.Exists(ModelsPath))
                Directory.CreateDirectory(ModelsPath);
        }

        public async Task<string> OpenFileClick(nint sender)
        {
            // 获取当前窗口的 ID（新版 API 需要）
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(sender);
            var windowId = Win32Interop.GetWindowIdFromWindow(sender);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var openPicker = new FileOpenPicker(appWindow.Id)  // 构造函数传入窗口 ID
            {
                ViewMode = PickerViewMode.List, // 仍然是枚举
                FileTypeFilter = { ".exe" }
            };
            openPicker.SuggestedStartFolder = DependencePath;
            // 注意：新版 API 没有 SuggestedStartFolder 属性
            // 如果需要指定自定义文件夹，可以使用 SuggestedStartLocation 枚举
            var result = await openPicker.PickSingleFileAsync();
            string path = null;
            if (result != null)
            {
                path = result.Path;  // 新版返回的是 PickFileResult，有 Path 属性
            }
            return path;
        }
    }
}
