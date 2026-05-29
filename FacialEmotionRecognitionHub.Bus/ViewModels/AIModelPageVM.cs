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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class AIModelPageVM : ObservableObject
    {
        private nint _windowNint;
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
        [ObservableProperty]
        private ObservableCollection<PivotItemVM> _pivotItemVMs;
        private AIModelM model;
        private IInterpreter _interpreter;
        private IOService _ioService;
        [ObservableProperty]
        private string _modelName;
        public AIModelPageVM(IInterpreter interpreter, IOService iOService)
        {
            _pivotItemVMs = [];
            _modelName = "Default Model Name";
            _interpreter = interpreter;
            _ioService = iOService;
        }
        public void GivePageCite(nint n)
        {
            _windowNint = n;
        }
        public void UpdateConsole()
        {
            if (model.Console != string.Empty)
            {
                Console += model.Console;
                UpdateModelStatus();
                model.Console = string.Empty;
            }
        }
        public void SetAIModelM(AIModelM model)
        {
            if (this.model is null)
            {
                this.model = model;
                ModelName = model.Name;
                PivotItemVMs.Clear();
                foreach (var instruction in model.ModifiableInstructionSet)
                {
                    var newPivotItemVM = new PivotItemVM(this, _interpreter, _ioService, _windowNint);
                    var iName = instruction.InstructionName;
                    if (iName.Contains("layer") || iName.Contains("resume") || iName.Contains("pause") || iName.Contains("terminate"))
                        continue;
                    newPivotItemVM.Initialize(iName, instruction);
                    PivotItemVMs.Add(newPivotItemVM);
                }
            }
            UpdateModelStatus();
        }
        private void UpdateModelStatus()
        {
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
        }
        [RelayCommand(CanExecute = nameof(CanPauseOrResume))]
        private async Task PauseOrResume()
        {
            Debug.WriteLine($"===> I {PromptShowed}");
            var i = model.GetInstruction(PromptShowed.ToLower());
            if (i is not null)
            {
                Console = Console + PromptShowed.ToLower() + "\n";
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
                            model.SetModelStatus(ModelStatus.Pause);
                        }
                        else
                            model.SetModelStatus(ModelStatus.IndeterminatedProcessing);
                        PromptShowed = PromptShowed == "Pause" ? "Resume" : "Pause";
                    }
                    else
                        model.SetModelStatus(ModelStatus.Error);
                }
                UpdateModelStatus();
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
            Debug.WriteLine("===> I Terminate");
            var i = model.GetInstruction("terminate");
            if (i is not null)
            {
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
                        PromptShowed = "Pause";
                        model.SetModelStatus(ModelStatus.Relax);
                    }
                    else
                        model.SetModelStatus(ModelStatus.Error);
                }
                UpdateModelStatus();
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
                _instructionNameInvokingNow = iName;
                if (i.Determinate)
                {
                    model.needReturnJSON = true;
                    model.SetModelStatus(ModelStatus.DeterminatedProcessing);
                    UpdateModelStatus();
                }
                else if (!i.Determinate)
                {
                    model.SetModelStatus(ModelStatus.IndeterminatedProcessing);
                    UpdateModelStatus();
                }
                var result = await i.Execute(i, param);
                if (result is not null)
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
                    model.needReturnJSON = false;
                    _instructionNameInvokingNow = string.Empty;
                }
                UpdateModelStatus();
            }
        }
    }
}
