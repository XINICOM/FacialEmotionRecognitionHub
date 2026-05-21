using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class TabViewVM : ObservableObject
    {
        private readonly TabViewM tabViewM;

        [ObservableProperty]
        private ObservableCollection<TabViewItem> _tabViewItems;

        public TabViewVM(AIModelsManager aIModelsManager)
        {
            tabViewM = new TabViewM(aIModelsManager);
            _tabViewItems = [];
        }
        /// <summary>
        /// 在打开 View 的时候更新数据
        /// </summary>
        public void LoadTabViewItems()
        {
            tabViewM.LoadTabViewItems();
            TabViewItems.Clear();
            foreach (var i in tabViewM.TabViewItems)
            {
                TabViewItems.Add(i);
            }
        }

        public void CreatNewAiModel()=> tabViewM.CreatNewAiModel();
    }
}
