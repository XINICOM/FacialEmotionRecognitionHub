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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class HomePageVM : ObservableObject
    {
        private AIModelsManager _aIModelsManager;
        [ObservableProperty]
        private ObservableCollection<AIModelM> _aIModelMs;
        public HomePageVM(AIModelsManager aIModelsManager)
        {
            _aIModelsManager = aIModelsManager ?? throw new ArgumentNullException(nameof(aIModelsManager));
            _aIModelMs = [];
        }
        [RelayCommand]
        public void UpdateAIModelMs()
        {
            Debug.WriteLine("UPDATE");
            AIModelMs.Clear();
            foreach (var i in _aIModelsManager.RunningAIModels)
            {
                AIModelMs.Add(i);
            }
            Debug.WriteLine($"{AIModelMs.Count()}");
        }
    }
}
