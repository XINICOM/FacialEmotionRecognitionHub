using System;
using FacialEmotionRecognitionHub.Bus.Services;
using FacialEmotionRecognitionHub.Bus.ViewModels;
using FacialEmotionRecognitionHub.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
namespace FacialEmotionRecognitionHub
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IInterpreter>(x => ActivatorUtilities.CreateInstance<JsonInterpreterService>(x));
            services.AddSingleton<IOService>(x => ActivatorUtilities.CreateInstance<IOService>(x, Windows.Storage.ApplicationData.Current.LocalFolder));
            services.AddSingleton<AIModelsManager>();
            services.AddTransient<TabViewVM>(x => ActivatorUtilities.CreateInstance<TabViewVM>(x, new HomePage()));
            services.AddTransient<HomePageVM>();
            services.AddTransient<AIModelPageVM>();
            return services.BuildServiceProvider();
        }
        private Window? _window;
        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Services.GetService<AIModelsManager>()?.Dispose();
        }
        public new static App Current => (App)Application.Current;
    }
}
