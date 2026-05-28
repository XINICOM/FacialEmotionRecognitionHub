using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FacialEmotionRecognitionHub.Bus;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using FacialEmotionRecognitionHub.Views;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FacialEmotionRecognitionHub
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private TabViewVM? VM;
        public MainWindow()
        {
            InitializeComponent();
            //自定义 titlebar
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            VM = App.Current.Services.GetService<TabViewVM>();
            //if (tabViewVM is not null)
            //    tabViewVM.LoadTabViewItems();

        }

        /// <summary>
        /// 关闭 tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void MainTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            //Debug.WriteLine("a");
            //sender.TabItems.Remove(args.Tab);
            if (VM is not null)
                VM.CloseTabViewItemCommand.Execute(args.Tab);
        }

        private async void MainTabView_AddTabButtonClickAsync(TabView sender, object args)
        {
            //if (VM is not null)
            //    VM.AddTabViewItemCommand.Execute(new ALModelCreatingPage());
            if (VM is not null)
            {
                var id = DateTime.Now;

                await VM.AddTabViewItem(WinRT.Interop.WindowNative.GetWindowHandle(this), new AIModelPage(id, WinRT.Interop.WindowNative.GetWindowHandle(this)), id);
                
                //VM.a();
            }

        }

        //private void MainTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    var newSelectedTab = e.AddedItems.FirstOrDefault() as TabViewItem;

        //    if (VM is not null)
        //    {

        //        //Debug.WriteLine("a");
        //        //VM.Navigation(newSelectedTab);
        //    }
        //}
    }
}
