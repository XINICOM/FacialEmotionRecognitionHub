using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class PivotItemVM : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        
        public PivotItemVM()
        {
            _name = "D PIVOTITEM";
        }

        public void Initialize(string name)
        {
            Name = name;
        }

    }
}
