/*
 * This file is part of FacialEmotionRecognitionHub.
 * Copyright (C) 2026 周诣成（Yicheng Zhou）XINICOM@outlook.com
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
namespace FacialEmotionRecognitionHub.Bus.Models
{
    public partial class ParameterTemplateSelector : DataTemplateSelector
    {
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
        private string _value = string.Empty;
        public string Value { get => _value; set { _value = value; OnPropertyChanged(); } }
        public async Task Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Name.Contains("save", StringComparison.CurrentCultureIgnoreCase))
                {
                    var fileName = Path.GetFileName(Value);
                    var windowId = Win32Interop.GetWindowIdFromWindow(this.sender);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    var folderPicker = new FolderPicker(appWindow.Id);
                    folderPicker.SuggestedStartFolder = IOService.DependencePath;
                    var result = await folderPicker.PickSingleFolderAsync();
                    if (result is null)
                        return;
                    Value = Path.Combine(result.Path, "fileName");
                }
                else
                {
                    var extention = Path.GetExtension(Value);
                    var windowId = Win32Interop.GetWindowIdFromWindow(this.sender);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    var openPicker = new FileOpenPicker(appWindow.Id)
                    {
                        ViewMode = PickerViewMode.List,
                    };
                    if (extention != null && extention != string.Empty)
                    {
                        openPicker.FileTypeFilter.Add(extention);
                    }
                    else
                    {
                        openPicker.FileTypeFilter.Add("*");
                    }
                    openPicker.SuggestedStartFolder = IOService.DependencePath;
                    var result = await openPicker.PickSingleFileAsync();
                    string path = null;
                    if (result != null)
                    {
                        path = result.Path;
                    }
                    if (path is null)
                        return;
                    Value = path;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
