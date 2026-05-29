using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
namespace FacialEmotionRecognitionHub.Views
{
    public sealed partial class HomePage : Page
    {
        private HomePageVM? VM;
        public HomePage()
        {
            InitializeComponent();
            VM = App.Current.Services.GetService<HomePageVM>();
            DataContext = VM;
            Loading += OnPageLoaded;
            WeakReferenceMessenger.Default.Register<ModelStatusMessage>(this, (o, m) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    VM?.UpdateAIModelMsCommand.Execute(null);
                });
            });
        }
        private void OnPageLoaded(object sender, object e)
        {
            VM?.UpdateAIModelMsCommand.Execute(null);
        }
    }
}
