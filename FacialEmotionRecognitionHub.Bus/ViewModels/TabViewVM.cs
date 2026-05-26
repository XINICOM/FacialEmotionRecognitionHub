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
using FacialEmotionRecognitionHub.Bus.Messages;

//using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
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
        //private readonly TabViewM tabViewM;

        [ObservableProperty]
        private ObservableCollection<TabViewItem> _tabViewItems;
        [ObservableProperty]
        private TabViewItem _selectedTabViewItem;

        private AIModelsManager aIModelsManager;
        private IOService iOService;

        public TabViewVM(AIModelsManager aIModelsManager,IOService iOService, object homePage)
        {
            //tabViewM = new TabViewM(aIModelsManager);
            this.aIModelsManager = aIModelsManager;
            this.iOService = iOService;
            _tabViewItems = new();
            InitializeTabViewItems(homePage);

            //TabViewItems.CollectionChanged += TabViewItems_CollectionChanged;
        }

        //private void TabViewItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    Debug.WriteLine("TabViewItems_CollectionChanged");
        //}

        private void InitializeTabViewItems(object homePage)
        {
            //TabViewItems.Clear();
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
                //Content = "this is home page" + DateTime.Now.ToString(),
            };
            homeTab.Resources = new()
            {
                { "TabViewItemHeaderBackground", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundSelected", new SolidColorBrush(Colors.Transparent) },
                { "TabViewItemHeaderBackgroundPointerOver", new SolidColorBrush(Color.FromArgb(51, 255, 255, 255)) },
                //{ "TabViewItemHeaderBackgroundPressed", new SolidColorBrush(Color.FromArgb(77, 255, 255, 255)) },
                ////{ "TabViewItemHeaderBorderBrushSelected", new SolidColorBrush(Colors.Blue) },//(Brush)Application.Current.Resources["SystemControlForegroundAccentBrush"]
                ////{ "TabViewItemHeaderBorderThicknessSelected", new Thickness(0,0,0,5) }
                //{ "TabViewSelectionIndicatorBrush", new SolidColorBrush(Colors.Blue) },//(Brush)Application.Current.Resources["SystemControlForegroundAccentBrush"]
                //{ "TabViewSelectionIndicatorThickness", new Thickness(0,0,0,5) }

            };
            TabViewItems.Add(homeTab);
            SelectedTabViewItem = homeTab;
        }

        //todo
        public async Task AddTabViewItem(nint sender, object page, DateTime id)
        {
            var exeFile = await iOService.OpenFileClick(sender);
            if (exeFile is null)
                return;
            Debug.WriteLine(exeFile);


            //todo
            var newModel = aIModelsManager.CreatNewAIModel(exeFile, id);

            WeakReferenceMessenger.Default.Send(new AIModelInitializedMessage(newModel));

            //tabViewM.LoadTabViewItems();
            //TabViewItems.Clear();
            //foreach (var i in tabViewM.TabViewItems)
            //{
            //    TabViewItems.Add(i);
            //}
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
                //Content = "this is AIM Creating page" + DateTime.Now.ToString(),
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

        [RelayCommand]
        private void CloseTabViewItem(TabViewItem tvi)
        {
            if (tvi is null || tvi.IsClosable == false)
                return;

            //var index = TabViewItems.IndexOf(tvi);
            //TabViewItems.Remove(tvi);
            //if (SelectedTabViewItem == tvi)
            //{
            //    if (TabViewItems.Count > 0)
            //    {
            //        SelectedTabViewItem = index > 0 ? TabViewItems[index - 1] : TabViewItems[0];
            //    }
            //}
        }

        
        //public void a()
        //{
        //    aIModelsManager.CreatNewAIModelInstruction();
        //}

        //[RelayCommand]
        //public void Navigation(TabViewItem tvi)
        //{

        //    if (TabViewItems.Any(x => x.Tag == tvi.Tag))
        //    {
        //        Debug.WriteLine("aaa");
        //        WeakReferenceMessenger.Default.Send(new SelectedTabChangedMessage(tvi));
        //    }
        //}

        //public void CreatNewAiModel() => tabViewM.CreatNewAiModel();
    }
}
