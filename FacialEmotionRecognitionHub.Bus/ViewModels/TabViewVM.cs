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
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
namespace FacialEmotionRecognitionHub.Bus.ViewModels
{
    public enum TabViewItemTag
    {
        HomePage,
        ALModelPage,
    }
    public partial class TabViewVM : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TabViewItem> _tabViewItems;
        [ObservableProperty]
        private TabViewItem _selectedTabViewItem;
        private AIModelsManager aIModelsManager;
        private IOService iOService;
        public TabViewVM(AIModelsManager aIModelsManager, IOService iOService, Page homePage)
        {
            this.aIModelsManager = aIModelsManager;
            this.iOService = iOService;
            _tabViewItems = new();
            InitializeTabViewItems(homePage);
        }
        private void InitializeTabViewItems(Page homePage)
        {
            var homeTab = new TabViewItem
            {
                Header = "Home",
                IconSource = new SymbolIconSource
                {
                    Symbol = Symbol.Home,
                },
                IsClosable = false,
                Content = homePage,
                Tag = TabViewItemTag.HomePage,
            };
            homeTab.Resources = new()
            {
                { "TabViewItemHeaderBackground", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundSelected", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundPointerOver", new SolidColorBrush(Color.FromArgb(51, 255, 255, 255)) },
          };
            TabViewItems.Add(homeTab);
            SelectedTabViewItem = homeTab;
        }
        public async Task AddTabViewItem(nint sender, Page page, DateTime id)
        {
            var exeFile = await iOService.OpenFileClick(sender, [".exe"]);
            if (exeFile is null)
                return;
            var newModel = aIModelsManager.CreatNewAIModel(exeFile, id);
            WeakReferenceMessenger.Default.Send(new AIModelInitializedMessage(newModel));
            var newTab = new TabViewItem
            {
                Header = newModel.Name,
                IconSource = new SymbolIconSource
                {
                    Symbol = Symbol.Library,
                },
                IsClosable = true,
                Content = page,
                Tag = newModel.ID,
            };
            newTab.Resources = new()
            {
                { "TabViewItemHeaderBackground", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundSelected", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundPointerOver", new SolidColorBrush(Color.FromArgb(51, 255, 255, 255)) },
            };
            TabViewItems.Add(newTab);
            SelectedTabViewItem = newTab;
        }
        public void CloseTabViewItem(TabViewItem tvi)
        {
            if (tvi is null || tvi.IsClosable == false)
                return;
            if (tvi.Tag is DateTime id)
            {
                aIModelsManager.DisposeModel(id);
            }
            TabViewItems.Remove(tvi);
        }
    }
}
