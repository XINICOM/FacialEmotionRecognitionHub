using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.Messaging;
//using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.Models;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FacialEmotionRecognitionHub.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private HomePageVM? VM;
        public HomePage()
        {
            InitializeComponent();
            VM = App.Current.Services.GetService<HomePageVM>();
            DataContext = VM;
            Loading += OnPageLoaded;
        }
        private void OnPageLoaded(object sender, object e)
        {
            //Debug.WriteLine("HOME PAGE LOAD");
            VM?.UpdateAIModelMsCommand.Execute(null);

            //todo
            //VM?.
        }

        //private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args) => VM?.a(sender, args);
    }
}
