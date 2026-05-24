using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private List<AIModelM> _aIModelMs;

        public HomePageVM(AIModelsManager aIModelsManager)
        {
            this._aIModelsManager = aIModelsManager ?? throw new ArgumentNullException(nameof(aIModelsManager));
        }

        [RelayCommand]
        private void UpdateAIModelMs()
        {
            AIModelMs = _aIModelsManager.RunningAIModels;
        }
    }
}
