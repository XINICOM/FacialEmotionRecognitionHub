using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using WinRT.Interop;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public partial class ParameterTemplateSelector : DataTemplateSelector
    {
        // 💡 加上了 DataTemplate? 允许在没跑起来时短暂为 null
        public DataTemplate? IntTemplate { get; set; }
        public DataTemplate? FloatTemplate { get; set; }
        public DataTemplate? BoolTemplate { get; set; }
        public DataTemplate? PathTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IntParameter)
                return IntTemplate;
            if (item is FloatParameter)
                return FloatTemplate;
            if (item is BoolParameter)
                return BoolTemplate;
            if (item is PathParameter)
                return PathTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
    public abstract class BaseParameter : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;

        // 💡 加了 ? 允许事件为空，修复了 Nullability 警告
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class IntParameter : BaseParameter
    {
        private int _value;
        public int Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    }

    public class FloatParameter : BaseParameter
    {
        private double _value;
        public double Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    }

    public class BoolParameter : BaseParameter
    {
        private bool _value;
        public bool Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    }

    public partial class PathParameter : BaseParameter
    {
        public nint sender;
        public IOService IOService;
        public PivotItemVM ItemVM;
        private string _value = string.Empty; // 💡 赋初始空值，防止未初始化警告
        public string Value { get => _value; set { _value = value; OnPropertyChanged(); } }
        public async Task Button_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("ffffffffffffffffffffffffffffffffffff");
            try
            {
                Debug.WriteLine("][[[[[[[[[[[[[[[[[[[[[" + this.sender.ToString());

                if (Name.Contains("save", StringComparison.CurrentCultureIgnoreCase))
                {
                    //var folderPicker = new FolderPicker(this.AppWindow.Id)
                    //{
                    //    // (Optional) Specify the initial location for the picker. 
                    //    //     If the specified location doesn't exist on the user's machine, it falls back to the DocumentsLibrary.
                    //    //     If not set, it defaults to PickerLocationId.Unspecified, and the system will use its default location.
                    //    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,

                    //    // (Optional) specify the text displayed on the commit button. 
                    //    //     If not specified, the system uses a default label of "Open" (suitably translated).
                    //    CommitButtonText = "Select Folder",

                    //    // (Optional) specify the view mode of the picker dialog. If not specified, default to List.
                    //    ViewMode = PickerViewMode.List,
                    //};

                    //var result = await folderPicker.PickSingleFolderAsync();

                    //if (result is not null)
                    //{
                    //    var path = result.Path;
                    //}
                    //else
                    //{
                    //    // Add your error handling here.
                    //}


                    var fileName = Path.GetFileName(Value);
                    // 1. 创建文件夹选择器
                    var windowId = Win32Interop.GetWindowIdFromWindow(this.sender);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    var folderPicker = new FolderPicker(appWindow.Id);

                    // 2. 【重要】获取当前窗口的句柄并初始化选择器
                    // 注意：这里的 "this" 指当前所在的 Window 对象
                    //var hwnd = WindowNative.GetWindowHandle(this);
                    //InitializeWithWindow.Initialize(folderPicker, hwnd);

                    // 3. 配置选择器
                    // 必须添加，否则会引发异常
                    //folderPicker.FileTypeFilter.Add("*");
                    // 设置起始位置为“文档库”
                    //folderPicker.SuggestedStartLocation = IOService.DependencePath;
                    folderPicker.SuggestedStartFolder = IOService.DependencePath;

                    // 4. 显示对话框并获取用户选择的文件夹
                    var result = await folderPicker.PickSingleFolderAsync();

                    // 5. 处理结果
                    if (result is null)
                        return;
                    Value = Path.Combine(result.Path, "fileName");
                }
                else
                {
                    var extention = Path.GetExtension(Value);

                    var windowId = Win32Interop.GetWindowIdFromWindow(this.sender);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    var openPicker = new FileOpenPicker(appWindow.Id)  // 构造函数传入窗口 ID
                    {
                        ViewMode = PickerViewMode.List, // 仍然是枚举
                                                        //FileTypeFilter = { ".exe" },
                    };
                    if (extention != null && extention != string.Empty)
                    {
                        
                            openPicker.FileTypeFilter.Add(extention);
                        
                    }
                    else
                    {
                        // 默认显示所有文件
                        openPicker.FileTypeFilter.Add("*");
                    }
                    //Debug.WriteLine(DependencePath);
                    openPicker.SuggestedStartFolder = IOService.DependencePath;
                    // 注意：新版 API 没有 SuggestedStartFolder 属性
                    // 如果需要指定自定义文件夹，可以使用 SuggestedStartLocation 枚举
                    var result = await openPicker.PickSingleFileAsync();
                    string path = null;
                    if (result != null)
                    {
                        path = result.Path;  // 新版返回的是 PickFileResult，有 Path 属性
                    }

                    //var result = await IOService.OpenFileClick(this.sender, [extention]);
                    if (path is null)
                        return;
                    Value = path;
                }
            }
            catch(Exception ex)
            {
                //todo
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
