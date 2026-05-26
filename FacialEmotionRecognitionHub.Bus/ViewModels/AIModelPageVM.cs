using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void SetAIModelM(AIModelM model)
        {
            //Debug.WriteLine(this.model is null);
            if(this.model is null)
            {
                //Debug.WriteLine("LOAD MODEL");
                this.model = model;
                ModelName = model.Name;
            }
        }
    }
}
