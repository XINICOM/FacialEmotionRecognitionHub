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
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public partial class TabViewVM : ObservableObject
    {
        //private readonly TabViewM tabViewM;

        [ObservableProperty]
        private ObservableCollection<TabViewItem> _tabViewItems;
        [ObservableProperty]
        private TabViewItem _selectedTabViewItem;

        private AIModelsManager aIModelsManager;

        public TabViewVM(AIModelsManager aIModelsManager)
        {
            //tabViewM = new TabViewM(aIModelsManager);
            this.aIModelsManager = aIModelsManager;
            _tabViewItems = [];
            InitializeTabViewItems();
        }

        private void InitializeTabViewItems()
        {
            TabViewItems.Clear();
            var homeTab = new TabViewItem
            {
                Header = "Home",
                IconSource = new SymbolIconSource
                {
                    Symbol = Symbol.Home
                },
                IsClosable = false,
                Content = "this is home page" + DateTime.Now.ToString()
            };
            TabViewItems.Add(homeTab);
            SelectedTabViewItem = homeTab;
        }

        [RelayCommand]
        public void AddTabViewItems()
        {
            //tabViewM.LoadTabViewItems();
            //TabViewItems.Clear();
            //foreach (var i in tabViewM.TabViewItems)
            //{
            //    TabViewItems.Add(i);
            //}
            var newTab = new TabViewItem
            {
                Header = "Creating New AI Model",
                IconSource = new SymbolIconSource
                {
                    Symbol = Symbol.Setting,
                },
                IsClosable = true,
                Content = "this is AIM Creating page" + DateTime.Now.ToString(),
            };
            TabViewItems.Add(newTab);
            SelectedTabViewItem = newTab;
        }

        //public void CreatNewAiModel() => tabViewM.CreatNewAiModel();
    }
}
