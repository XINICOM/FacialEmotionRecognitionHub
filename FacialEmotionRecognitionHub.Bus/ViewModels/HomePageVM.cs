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
