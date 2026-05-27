using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FacialEmotionRecognitionHub.Bus.Models;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class PivotItemVM : ObservableObject
    {
        private AIModelPageVM pageVM;

        [ObservableProperty]
        private string _name = "D PIVOTITEM";

        [ObservableProperty]
        private ObservableCollection<BaseParameter> _parameters = new();

        public PivotItemVM(AIModelPageVM pageVM)
        {
            this.pageVM = pageVM;
        }

        public void Initialize(string name, List<BaseParameter> @params)
        {
            Name = name;

            Parameters.Clear();
            foreach (var param in @params)
            {
                Parameters.Add(param);
            }
        }

    }
}
