using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class AIModelPageVM : ObservableObject
    {
        [ObservableProperty]
        private bool _showError = false;
        [ObservableProperty]
        private bool _showPaused = false;
        [ObservableProperty]
        private bool _isIndeterminate = false;
        [ObservableProperty]
        private float _value = 0f;
        [ObservableProperty]
        private string _status = "DEFAULT STATUS";

        //todo
        //private ObservableCollection<PivotItem>
        [ObservableProperty]
        private ObservableCollection<PivotItemVM> _pivotItemVMs;

        private AIModelM model;
        private IInterpreter _interpreter;

        [ObservableProperty]
        private string _modelName;

        public AIModelPageVM(IInterpreter interpreter)
        {
            _pivotItemVMs = [];
            _modelName = "Default Model Name";
            _interpreter = interpreter;
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
                    var newPivotItemVM = new PivotItemVM(this);
                    var iName = instruction.InstructionName;
                    Debug.WriteLine("==="+ iName);
                    if (iName.Contains("layer") || iName.Contains("resume") || iName.Contains("pause") || iName.Contains("terminate"))
                        continue;

                    List<BaseParameter> @params = [];
                    foreach(var param in instruction.TemplateParameters)
                    {
                        if (param.Value is int i)
                        {
                            @params.Add(new IntParameter
                            {
                                Name = param.Key,
                                Value = i,
                                Header = _interpreter.FormatInstructionName(param.Key),
                            });
                        }
                        else if(param.Value is float f)
                        {
                            @params.Add(new FloatParameter
                            {
                                Name = param.Key,
                                Value = f,
                                Header = _interpreter.FormatInstructionName(param.Key),
                            });
                        }
                        else if(param.Value is bool b)
                        {
                            @params.Add(new BoolParameter
                            {
                                Name = param.Key,
                                Value = b,
                                Header = _interpreter.FormatInstructionName(param.Key),
                            });
                        }
                        else if( param.Value is string s && s.Contains('.'))
                        {
                            @params.Add(new PathParameter
                            {
                                Name = param.Key,
                                Value = s,
                                Header = _interpreter.FormatInstructionName(param.Key),
                            });
                        }
                    }

                    newPivotItemVM.Initialize(iName, @params);
                    PivotItemVMs.Add(newPivotItemVM);
                }
            }
        }
    }
}
