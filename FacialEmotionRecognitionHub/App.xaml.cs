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
