using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Messages;
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
    public sealed partial class AIModelPage : Page
    {
        //public ObservableCollection<BaseParameter> LoadDataParameters { get; set; }
        //public ObservableCollection<BaseParameter> PreprocessParameters { get; set; }
        //public ObservableCollection<BaseParameter> TrainParameters { get; set; }
        //public ObservableCollection<BaseParameter> PredictParameters { get; set; }




        private DateTime _id;

        private AIModelPageVM? VM;

        public AIModelPage(DateTime id)
        {
            InitializeComponent();
            _id = id;
            VM = App.Current.Services.GetService<AIModelPageVM>();

            WeakReferenceMessenger.Default.Register<AIModelInitializedMessage>(this, (o, m) =>
            {
                //Debug.WriteLine("TRY MODEL UPDATE");
                //Debug.WriteLine($"{m.Value.GetType()}");

                if(m.Value is AIModelM model)
                {
                    VM?.SetAIModelM(model);
                }

                WeakReferenceMessenger.Default.Unregister<AIModelInitializedMessage>(this);
            });

            //Loaded += AIModelPage_Loaded;

            
        }

        //private void AIModelPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if(VM is not null)
        //    {
        //        Debug.WriteLine("///" + VM.PivotItemVMs.Count());
        //        foreach(var i in VM.PivotItemVMs)
        //        {
        //            Debug.WriteLine("///" + i.Name);
        //        }

        //    }

        //}

        //public void InitializeWithAIModelM(AIModelM model) => VM?.SetAIModelM(model);


        //// ==========================================
        //// TAB 1: DATA LOADING EVENTS
        //// ==========================================
        //private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // 💡 前端同学读取控件值示范：string path = LoadPathTextBox.Text;
        //}

        //// ==========================================
        //// TAB 2: PREPROCESSING EVENTS
        //// ==========================================
        //private void PreprocessButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // 💡 前端同学读取控件值示范：double bSize = BatchSizeNumberBox.Value;
        //}

        //// ==========================================
        //// TAB 3: TRAINING MANAGEMENT EVENTS
        //// ==========================================
        //private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TrainingStatusTxt.Text = "Status: Training...";
        //    TrainingProgressBar.IsIndeterminate = true;
        //    PauseTrainingBtn.IsEnabled = true; // 激活暂停按钮
        //    TerminalBlock.Text += "\n[EXEC] python train.py --epochs 20 --lr 0.001";
        //}

        ///// <summary>
        ///// 💡 核心改动：Pause 按钮与 Resume 按钮的就地转换逻辑
        ///// </summary>
        //private void PauseTrainingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (PauseTrainingBtn.Content.ToString() == "Pause")
        //    {
        //        // 切换为暂停状态
        //        PauseTrainingBtn.Content = "Resume";
        //        TrainingStatusTxt.Text = "Status: Paused";
        //        TrainingProgressBar.IsIndeterminate = false; // 暂停进度流动
        //        TerminalBlock.Text += "\n[SUSPEND] Training process paused by user.";
        //    }
        //    else
        //    {
        //        // 切换回恢复状态
        //        PauseTrainingBtn.Content = "Pause";
        //        TrainingStatusTxt.Text = "Status: Training...";
        //        TrainingProgressBar.IsIndeterminate = true; // 恢复进度流动
        //        TerminalBlock.Text += "\n[RESUME] Training process resumed.";
        //    }
        //}

        //private void TerminateTrainingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TrainingStatusTxt.Text = "Status: Terminated";
        //    TrainingProgressBar.IsIndeterminate = false;
        //    PauseTrainingBtn.Content = "Pause";
        //    PauseTrainingBtn.IsEnabled = false; // 禁用暂停按钮
        //    TerminalBlock.Text += "\n[CRITICAL] Process terminated explicitly.";
        //}

        //// ==========================================
        //// TAB 4: REAL-TIME INFERENCE EVENTS
        //// ==========================================
        //private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        //{
        //}

        //private void PredictButton_Click(object sender, RoutedEventArgs e)
        //{
        //}
    }
}
