using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.Windows.Storage.Pickers;
using Windows.Storage;
namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class IOService
    {
        private StorageFolder _storageFolder;
        public string DependencePath = string.Empty;
        public string ModelsPath = string.Empty;
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
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            string saveFloder = Path.Combine(folderPath, "save");
            if (!Directory.Exists(saveFloder))
                Directory.CreateDirectory(saveFloder);
            string cacheFloder = Path.Combine(folderPath, "cache");
            if (!Directory.Exists(cacheFloder))
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
            var windowId = Win32Interop.GetWindowIdFromWindow(sender);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var openPicker = new FileOpenPicker(appWindow.Id)
            {
                ViewMode = PickerViewMode.List,
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
                openPicker.FileTypeFilter.Add("*");
            }
            Debug.WriteLine(DependencePath);
            openPicker.SuggestedStartFolder = DependencePath;
            var result = await openPicker.PickSingleFileAsync();
            string path = null;
            if (result != null)
            {
                path = result.Path;
            }
            return path;
        }
    }
}
