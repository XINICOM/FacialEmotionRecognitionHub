using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using FacialEmotionRecognitionHub.Bus.Services;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public class TabViewM
    {
        private AIModelsManager _AIModelsManager;
        public ObservableCollection<TabViewItem> TabViewItems { get; set; } = [];
        public TabViewM(AIModelsManager aIModelsManager)
        {
            this._AIModelsManager = aIModelsManager;
        }

        public void LoadTabViewItems()
        {
            List<AIModelM> aIModelMs = _AIModelsManager.RunningAIModels;
            TabViewItems.Clear();
            foreach (var i in aIModelMs)
            {
                var tvi = new TabViewItem();
                tvi.Header = i.Name;
                tvi.Content = i.Name;
                TabViewItems.Add(tvi);
            }
        }

        public void CreatNewAiModel()
        {
            _AIModelsManager.CreatNewAIModel();
        }
    }
}
