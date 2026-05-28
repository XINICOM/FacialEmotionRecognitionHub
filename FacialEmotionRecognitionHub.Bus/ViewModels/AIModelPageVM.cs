using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    //todo
    //public class ConsoleOutputMessage
    //{
    //    public string Text { get; set; }
    //}

    public partial class AIModelPageVM : ObservableObject
    {
        [ObservableProperty]
        private string _console = string.Empty;

        [ObservableProperty]
        private bool _showError = false;
        [ObservableProperty]
        private bool _showPaused = false;
        [ObservableProperty]
        private bool _isIndeterminate = false;
        [ObservableProperty]
        private float _value = 0f;
        [ObservableProperty]
        private string _status = string.Empty;
        private string _instructionNameInvokingNow = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PauseOrResumeCommand))]
        [NotifyCanExecuteChangedFor(nameof(TerminateCommand))]
        private bool _changedForView = false;
        [ObservableProperty]
        private string _promptShowed = "Pause";

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

            //todo
            //WeakReferenceMessenger.Default.Register<ConsoleOutputMessage>(this, (r, m) =>
            //{
            //    Console += m.Text;
            //});
        }

        public void UpdateConsole()
        {
            if (model.Console != string.Empty)
            {
                Console += model.Console;
                model.Console = string.Empty;

            }
        }

        public void SetAIModelM(AIModelM model)
        {
            //Debug.WriteLine(this.model is null);
            if (this.model is null)
            {
                //Debug.WriteLine("LOAD MODEL");
                this.model = model;
                ModelName = model.Name;

                //todo
                PivotItemVMs.Clear();
                foreach (var instruction in model.ModifiableInstructionSet)
                {
                    var newPivotItemVM = new PivotItemVM(this, _interpreter);
                    var iName = instruction.InstructionName;
                    //Debug.WriteLine("===" + iName);
                    if (iName.Contains("layer") || iName.Contains("resume") || iName.Contains("pause") || iName.Contains("terminate"))
                        continue;
                    newPivotItemVM.Initialize(iName, instruction);
                    PivotItemVMs.Add(newPivotItemVM);
                }
            }

            //model.OnConsoleChanged += () =>
            //{
                
            //    //Console += model.Console;
            //    //model.Console = string.Empty;
            //    UpdateConsole();
            //};

            //model.Process.OutputDataReceived += (s, e) =>
            //{
            //    if (!string.IsNullOrEmpty(e.Data))
            //    {
            //        //Console += "[RECEIVED]" + e.Data + "\n";
            //        WeakReferenceMessenger.Default.Send(new ConsoleOutputMessage
            //        {
            //            Text = "[RECEIVED]" + e.Data + "\n"
            //        });
            //    }
            //};

            //model.Process.ErrorDataReceived += (s, e) =>
            //{
            //    if (!string.IsNullOrEmpty(e.Data))
            //    {
            //        Console += "[GETERROR]" + e.Data + "\n";
            //    }
            //};

            //model.Process.Start();
            //model.Process.BeginOutputReadLine();
            //model.Process.BeginErrorReadLine();

            UpdateModelStatus();
            //UpdateConsole();
        }

        private void UpdateModelStatus()
        {
            //Debug.WriteLine($">>>>>>>>>>{model.ShowPaused}");
            var s = model.modelStatus;

            ShowError = model.ShowError;
            ShowPaused = model.ShowPaused;
            IsIndeterminate = model.IsIndeterminate;
            Value = model.Value;
            switch (s)
            {
                case ModelStatus.Relax:
                {
                    Status = "Relax";
                    //Changed = !Changed;
                    break;
                }
                case ModelStatus.DeterminatedProcessing:
                {
                    Status = "Executing " + _instructionNameInvokingNow;
                    break;
                }
                case ModelStatus.IndeterminatedProcessing:
                {
                    Status = "Executing " + _instructionNameInvokingNow;
                    break;
                }
                case ModelStatus.Pause:
                {
                    Status = "Paused";
                    break;
                }
                case ModelStatus.Error:
                {
                    Status = "ERROR";
                    break;
                }
            }
            ChangedForView = !ChangedForView;


            //Debug.WriteLine($"ShowError = {ShowError}");
            //Debug.WriteLine($"ShowPaused = {ShowPaused}");
            //Debug.WriteLine($"IsIndeterminate = {IsIndeterminate}");
            //Debug.WriteLine($"Value = {Value}");
            //Debug.WriteLine($"Status = {Status}");
        }

        [RelayCommand(CanExecute = nameof(CanPauseOrResume))]
        private async Task PauseOrResume()
        {
            //todo
            Debug.WriteLine($"===> I {PromptShowed}");
            var i = model.GetInstruction(PromptShowed.ToLower());

            if (i is not null)
            {
                Console = Console + PromptShowed.ToLower() + "\n";
                //Debug.WriteLine($"{PromptShowed}");
                var result = await i.Execute(i, null);
                if (result is not null)
                {
                    foreach (var item in result)
                    {
                        Console += $">>>{item.Key} -> {item.Value} ({item.Value.GetType()})\n";
                    }

                    result.TryGetValue("successful", out object value);
                    if (value is not null && value.ToString() == "0")
                    {
                        if (PromptShowed == "Pause")
                        {
                            Debug.WriteLine("//////PAUSE");
                            model.SetModelStatus(ModelStatus.Pause);
                            ShowPaused = true;

                        }
                        else
                            model.SetModelStatus(ModelStatus.IndeterminatedProcessing);

                        PromptShowed = PromptShowed == "Pause" ? "Resume" : "Pause";
                        //Debug.WriteLine($"successful");

                    }
                    else
                        model.SetModelStatus(ModelStatus.Error);
                }
                UpdateModelStatus();

                //UpdateConsole();
            }
        }
        private bool CanPauseOrResume()
        {
            if (model.GetInstruction("pause") is null || model.GetInstruction("resume") is null)
                return false;
            if (model.modelStatus != ModelStatus.Error)
            {
                if (PromptShowed == "Pause" && model.modelStatus != ModelStatus.Pause)
                    return true;
                if (PromptShowed == "Resume" && model.modelStatus == ModelStatus.Pause)
                    return true;
                return false;
            }
            else
                return false;
        }

        [RelayCommand(CanExecute = nameof(CanTerminate))]
        private async Task TerminateAsync()
        {
            //todo
            Debug.WriteLine("===> I Terminate");
            var i = model.GetInstruction("terminate");

            if (i is not null)
            {
                //Debug.WriteLine($"{PromptShowed}");
                Console = Console + "terminate\n";
                var result = await i.Execute(i, null);
                if (result is not null)
                {
                    foreach (var item in result)
                    {
                        Console += $">>>{item.Key} -> {item.Value} ({item.Value.GetType()})\n";
                    }

                    result.TryGetValue("successful", out object value);
                    if (value is not null && value.ToString() == "0")
                    {
                        //PromptShowed = PromptShowed == "Pause" ? "Resume" : "Pause";
                        //Debug.WriteLine($"successful");
                        PromptShowed = "Pause";
                        model.SetModelStatus(ModelStatus.Relax);

                    }
                    else
                        model.SetModelStatus(ModelStatus.Error);
                }
                UpdateModelStatus();

                //UpdateConsole();
            }
        }
        private bool CanTerminate()
        {
            if (model.GetInstruction("terminate") is null)
                return false;
            if (model.modelStatus != ModelStatus.Error)
                return true;
            else
                return false;
        }

        public async Task ExecuteInstruction(string iName, Dictionary<string, object> param)
        {
            Debug.WriteLine($"===> I {iName}");


            var i = model.GetInstruction(iName);
            if (i is not null)
            {
                Console += iName + "\n";
                if (i.Determinate)
                {
                    model.SetModelStatus(ModelStatus.DeterminatedProcessing);
                    UpdateModelStatus();

                }
                else if (!i.Determinate)
                {
                    model.SetModelStatus(ModelStatus.IndeterminatedProcessing);
                    UpdateModelStatus();

                }

                var result = await i.Execute(i, param);
                if(result is not null)
                {
                    foreach (var item in result)
                    {
                        Console += $">>>{item.Key} -> {item.Value} ({item.Value.GetType()})\n";
                    }

                    result.TryGetValue("successful", out object value);
                    if (value is not null && value.ToString() == "0")
                    {
                        PromptShowed = "Pause";

                        model.SetModelStatus(ModelStatus.Relax);
                    }
                    else
                    {
                        model.SetModelStatus(ModelStatus.Error);
                    }
                }
                UpdateModelStatus();

                //UpdateConsole() ;
            }
        }
    }
}
