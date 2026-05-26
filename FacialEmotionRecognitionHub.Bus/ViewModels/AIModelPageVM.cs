using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FacialEmotionRecognitionHub.Bus.Models;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class AIModelPageVM : ObservableObject
    {
        //todo
        //private ObservableCollection<PivotItem>
        [ObservableProperty]
        private ObservableCollection<PivotItemVM> _pivotItemVMs;

        private AIModelM model;

        [ObservableProperty]
        private string _modelName;

        public AIModelPageVM()
        {
            _pivotItemVMs = [];
            _modelName = "Default Model Name";
        }

        public void SetAIModelM(AIModelM model)
        {
            //Debug.WriteLine(this.model is null);
            if(this.model is null)
            {
                //Debug.WriteLine("LOAD MODEL");
                this.model = model;
                ModelName = model.Name;

                //todo
                PivotItemVMs.Clear();
                foreach(var instruction in model.ModifiableInstructionSet)
                {
                    var newPivotItemVM = new PivotItemVM();
                    var iName = instruction.InstructionName;
                    Debug.WriteLine("==="+ iName);
                    if (iName.Contains("layer") || iName.Contains("resume") || iName.Contains("pause") || iName.Contains("terminate"))
                        continue;
                    newPivotItemVM.Initialize(iName);
                    PivotItemVMs.Add(newPivotItemVM);
                }
            }
        }
    }
}
