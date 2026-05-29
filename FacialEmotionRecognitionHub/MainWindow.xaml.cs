using System;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using FacialEmotionRecognitionHub.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace FacialEmotionRecognitionHub
{
    public sealed partial class MainWindow : Window
    {
        private TabViewVM? VM;
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            VM = App.Current.Services.GetService<TabViewVM>();
        }
        void MainTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (VM is not null)
                VM.CloseTabViewItem(args.Tab);
        }
        private async void MainTabView_AddTabButtonClickAsync(TabView sender, object args)
        {
            if (VM is not null)
            {
                var id = DateTime.Now;
                var wh = WinRT.Interop.WindowNative.GetWindowHandle(this);
                await VM.AddTabViewItem(wh, new AIModelPage(id, wh), id);
            }
        }
    }
}