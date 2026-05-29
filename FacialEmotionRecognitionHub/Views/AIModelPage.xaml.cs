using System;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.Models;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace FacialEmotionRecognitionHub.Views
{
    public sealed partial class AIModelPage : Page
    {
        private DateTime _id;
        private AIModelPageVM? VM;
        public AIModelPage(DateTime id, nint n)
        {
            InitializeComponent();
            _id = id;
            VM = App.Current.Services.GetService<AIModelPageVM>();
            VM?.GivePageCite(n);
            WeakReferenceMessenger.Default.Register<AIModelInitializedMessage>(this, (o, m) =>
            {
                if (m.Value is AIModelM model)
                {
                    VM?.SetAIModelM(model);
                }
                WeakReferenceMessenger.Default.Unregister<AIModelInitializedMessage>(this);
            });
            WeakReferenceMessenger.Default.Register<ConsoleOutputMessage>(this, (o, m) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (m.ID == _id && VM is not null)
                    {
                        VM.UpdateConsole();
                    }
                });
            });
        }
        private event Action? TerminalBlockSizeChanged;
        private void TerminalBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TerminalBlockSizeChanged?.Invoke();
        }
        private void ConsoleScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            TerminalBlockSizeChanged += () =>
            {
                if (sender is ScrollViewer scrollViewer)
                {
                    scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null);
                }
            };
        }
    }
}
