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