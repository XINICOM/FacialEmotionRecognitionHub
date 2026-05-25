using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FacialEmotionRecognitionHub.Bus.Models;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class AIModelPageVM : ObservableObject
    {
        private AIModelM model;
        [ObservableProperty]
        private string _modelName;

        public AIModelPageVM()
        {
            ModelName = "Default Model Name";
        }
    }
}
