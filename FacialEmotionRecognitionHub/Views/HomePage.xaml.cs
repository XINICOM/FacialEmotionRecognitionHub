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
