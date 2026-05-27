using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class PivotItemVM : ObservableObject
    {
        private IInterpreter _interpreter;

        //[ObservableProperty]
        //private string _buttonText = "Pause";

        //todo
        [ObservableProperty]
        private AIModelPageVM _pageVM;
        //public AIModelPageVM pageVM;

        [ObservableProperty]
        private string _name = "D PIVOTITEM";

        [ObservableProperty]
        private ObservableCollection<BaseParameter> _parameters = new();

        private ModifiableInstruction _instruction;

        public PivotItemVM(AIModelPageVM pageVM, IInterpreter interpreter)
        {
            this._pageVM = pageVM;
            //this.pageVM = pageVM;
            _interpreter = interpreter;

            //_pageVM.PropertyChanged += (s, e) =>
            //{
            //    if (e.PropertyName == nameof(_pageVM.PromptShowed))
            //    {
            //        ButtonText = _pageVM.PromptShowed;
            //    }
            //};
            //ButtonText = _pageVM.PromptShowed;
        }

        public void Initialize(string name, ModifiableInstruction instruction)
        {
            Name = name;
            _instruction = instruction;

            List<BaseParameter> @params = [];
            foreach (var param in _instruction.TemplateParameters)
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
                else if (param.Value is float f)
                {
                    @params.Add(new FloatParameter
                    {
                        Name = param.Key,
                        Value = f,
                        Header = _interpreter.FormatInstructionName(param.Key),
                    });
                }
                else if (param.Value is bool b)
                {
                    @params.Add(new BoolParameter
                    {
                        Name = param.Key,
                        Value = b,
                        Header = _interpreter.FormatInstructionName(param.Key),
                    });
                }
                else if (param.Value is string s && s.Contains('.'))
                {
                    @params.Add(new PathParameter
                    {
                        Name = param.Key,
                        Value = s,
                        Header = _interpreter.FormatInstructionName(param.Key),
                    });
                }
            }
            Parameters.Clear();
            foreach (var param in @params)
            {
                Parameters.Add(param);
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecute))]
        private async Task Execute()
        {
            var newDict = new Dictionary<string, object>();
            foreach (var param in Parameters)
            {
                if (param is IntParameter i)
                {
                    newDict.Add(param.Name, i.Value);
                }
                else if (param is FloatParameter f)
                {
                    newDict.Add(param.Name, f.Value);
                }
                else if (param is BoolParameter b)
                {
                    newDict.Add(param.Name, b.Value);

                }
                else if (param is PathParameter path)
                {
                    newDict.Add(param.Name, path.Value);

                }
            }
            await PageVM.ExecuteInstruction(Name, newDict);
        }
        private bool CanExecute()
        {
            //todo
            return true;
        }
    }
}
