using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class HomePageVM : ObservableObject
    {
        
        private AIModelsManager _aIModelsManager;
        [ObservableProperty]
        private ObservableCollection<AIModelM> _aIModelMs;

        public HomePageVM(AIModelsManager aIModelsManager)
        {
            this._aIModelsManager = aIModelsManager ?? throw new ArgumentNullException(nameof(aIModelsManager));
            _aIModelMs = [];
        }

        [RelayCommand]
        public void UpdateAIModelMs()
        {
            Debug.WriteLine("UPDATE");
            AIModelMs.Clear();
            //AIModelMs = _aIModelsManager.RunningAIModels;
            foreach(var i in _aIModelsManager.RunningAIModels)
            {
                AIModelMs.Add(i);
            }
            Debug.WriteLine($"{AIModelMs.Count()}");

        }

        //public void a(ItemsView sender, ItemsViewItemInvokedEventArgs args)
        //{
        //    Debug.WriteLine("INVOKE");
        //    _aIModelsManager.Pause(args.InvokedItem as AIModelM);
        //    updateAIModelMsCommand.Execute(null);
        //}
    }
}
