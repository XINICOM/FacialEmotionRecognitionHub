using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

        public string SearchAndInitializeConfigJson(string exePath)
        {
            string name = Path.GetFileNameWithoutExtension(exePath);
            string configFolder = Path.Combine(ModelsPath, name);
            InitializeConfigFolder(configFolder);
            string primaryConfigJson = Path.Combine(configFolder, $"{name}.json");
            if (!File.Exists(primaryConfigJson))
            {
                string exeFloder = Path.GetDirectoryName(exePath);
                string secondaryConfigJson = Path.Combine(exeFloder, $"{name}.json");
                Debug.WriteLine(secondaryConfigJson);
                if (!File.Exists(secondaryConfigJson))
                    throw new InvalidOperationException("Miss the critical config json file !");
                File.Copy(secondaryConfigJson, primaryConfigJson, true);
            }
            return primaryConfigJson;
        }

        public void InitializeConfigFolder(string folderPath)
        {
            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            string saveFloder = Path.Combine(folderPath, "save");
            if(!Directory.Exists(saveFloder))
                Directory.CreateDirectory(saveFloder);
            string cacheFloder = Path.Combine(folderPath, "cache");
            if(!Directory.Exists(cacheFloder))
                Directory.CreateDirectory(cacheFloder);
        }

        public int GetAvailablePort(IPAddress ip)
        {
            using var listener = new TcpListener(ip, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public async Task<string> OpenFileClick(nint sender, IList<string> extentions = null)
        {
            // 获取当前窗口的 ID（新版 API 需要）
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(sender);
            var windowId = Win32Interop.GetWindowIdFromWindow(sender);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var openPicker = new FileOpenPicker(appWindow.Id)  // 构造函数传入窗口 ID
            {
                ViewMode = PickerViewMode.List, // 仍然是枚举
                //FileTypeFilter = { ".exe" },
            };
            if (extentions != null && extentions.Count > 0)
            {
                foreach (var filter in extentions)
                {
                    openPicker.FileTypeFilter.Add(filter);
                }
            }
            else
            {
                // 默认显示所有文件
                openPicker.FileTypeFilter.Add("*");
            }
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
