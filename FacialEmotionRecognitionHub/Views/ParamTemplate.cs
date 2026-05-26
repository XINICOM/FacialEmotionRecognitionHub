using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Views
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

    public class PathParameter : BaseParameter
    {
        private string _value = string.Empty; // 💡 赋初始空值，防止未初始化警告
        public string Value { get => _value; set { _value = value; OnPropertyChanged(); } }
    }
}
