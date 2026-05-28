using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Services;
using FacialEmotionRecognitionHub.Bus.ViewModels;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public enum ModelStatus
    {
        Error,
        DeterminatedProcessing,
        IndeterminatedProcessing,
        Pause,
        Relax,
    }

    public class AIModelM : IDisposable
    {
        public Process Process;

        private string _console = string.Empty;

        public string Console
        {
            get
            {
                return _console;
            }
            set
            {
                if(_console != value)
                {
                    _console = value;
                    if(value != string.Empty)
                    {
                        OnConsoleChanged?.Invoke();
                    }
                }
            }
        }
        public event Action OnConsoleChanged;

        public AIModelM(DateTime id, string dependenceEXEPath, int port, string modelConfigJson, string name)
        {
            //todo
            OnConsoleChanged += () =>
            {
                Debug.Write(Console);
                Console = string.Empty;
            };


            ModelConfigJson = modelConfigJson;
            DependenceEXEPath = dependenceEXEPath;
            Port = port;
            Name = name;
            _id = id;

            ModifiableInstructionSet = [];

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://127.0.0.1:{Port}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            try
            {
                // 构建传入参数
                string arguments = Port.ToString();

                Process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = DependenceEXEPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    }
                };

                // 连接控制台输出
                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;

                //Process.OutputDataReceived += (s, e) =>
                //{
                //    if (!string.IsNullOrEmpty(e.Data))
                //    {
                //        //Console += "[RECEIVED]" + e.Data + "\n";
                //        //todo
                //        //WeakReferenceMessenger.Default.Send(new ConsoleOutputMessage
                //        //{
                //        //    Text = "[RECEIVED]" + e.Data + "\n"
                //        //});


                //        //Console += "[RECEIVED]" + e.Data + "\n";
                //    }
                //};

                // 进程退出事件
                Process.Exited += Process_Exited;
                Process.EnableRaisingEvents = true;

                //todo
                Process.Start();
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();


                //return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"启动失败: {ex.Message}");
                //return false;
                throw;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Debug.WriteLine("模型服务已退出");
            //throw new NotImplementedException();
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($">>>[错误] {e.Data}");
            }
            //throw new NotImplementedException();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {

                //Console += "[RECEIVED]" + e.Data + "\n";
                //todo
                WeakReferenceMessenger.Default.Send(new ConsoleOutputMessage
                {
                    Text = "[RECEIVED]" + e.Data + "\n"
                });


                Console += "[RECEIVED]" + e.Data + "\n";
                

                //Debug.WriteLine($">>>[输出] {e.Data}");
                // 或者更新 UI
            }
            //throw new NotImplementedException();
        }

        public readonly HttpClient httpClient;

        private bool _showError = false;
        public bool ShowError { get { return _showError; } }
        private bool _showPaused = false;
        public bool ShowPaused { get { return _showPaused; } }
        private bool _isIndeterminate = false;
        public bool IsIndeterminate { get { return _isIndeterminate; } }
        private string _status = "No Progress";
        public string Status { get { return _status; } }

        //private ModelStatus _status;
        public ModelStatus modelStatus = ModelStatus.Relax;
        private float _value = 0f;
        public float Value {  get { return _value; } }
        private DateTime _id;
        public DateTime ID { get { return _id; } }

        public string Name = string.Empty;
        public string ModelConfigJson = string.Empty;
        public string DependenceEXEPath = string.Empty;
        public int Port = 0;

        public int RunningPort
        {
            get
            {
                return Port;
            }
        }


        private AIModelConnectionService _aIModelConnectionService;
        public List<ModifiableInstruction> ModifiableInstructionSet;

        //public AIModelM(AIModelConnectionService aIModelConnectionService, string instructionSet)
        //{
        //    //Name = name;
        //    _modificationset = [];
        //    _aIModelConnectionService = aIModelConnectionService;
        //}



        public void SetModelStatus(ModelStatus target, float progress = 0f)
        {
            if (target == ModelStatus.Error)
            {
                _showError = true;
                _showPaused = false;
                _isIndeterminate = true;
                _status = "ERROR";
                _value = 0;
            }
            else if (target == ModelStatus.Pause)
            {
                _showError = false;
                _showPaused = true;
                _isIndeterminate = true;
                _status = "PAUSE";
            }
            else if (target == ModelStatus.IndeterminatedProcessing)
            {
                _showError = false;
                _showPaused = false;
                _isIndeterminate = true;
                _status = "PROCESSING";
            }
            else if(target == ModelStatus.DeterminatedProcessing)
            {
                _showError = false;
                _showPaused = false;
                _isIndeterminate = false;
                _status = "PROCESSING";
                _value = progress;
            }
            else
            {
                _showError = false;
                _showPaused = false;
                _isIndeterminate = false;
                _status = "RELAX";
                _value = 0f;
            }
            modelStatus = target;
        }


        public void AddNewInstruction(ModifiableInstruction newInstruction)
        {
            if (ModifiableInstructionSet is null)
                ModifiableInstructionSet = new();

            if (ModifiableInstructionSet.Any(x => x.InstructionName == newInstruction.InstructionName))
            {
                //_modificationset.First(x=>x.InstructionName == newInstruction.InstructionName) = newInstruction;
                ModifiableInstructionSet[ModifiableInstructionSet.FindIndex(x => x.InstructionName == newInstruction.InstructionName)] = newInstruction;
            }
            else
            {
                ModifiableInstructionSet.Add(newInstruction);
            }
        }

        public ModifiableInstruction GetInstruction(string instructionName)
        {
            if (ModifiableInstructionSet.Any(x => x.InstructionName == instructionName))
            {
                //return _modifiableInstructionSet.First(x => x.InstructionName == instructionName);
                return ModifiableInstructionSet[ModifiableInstructionSet.FindIndex(x => x.InstructionName == instructionName)];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            //Debug.WriteLine("===MODEL D");
            StopProcess();
            GC.SuppressFinalize(this);
        }

        private void StopProcess()
        {
            if (Process != null && !Process.HasExited)
            {
                try
                {
                    //// 尝试友好关闭（发送 Ctrl+C 信号）
                    //if (!Process.CloseMainWindow())
                    //{
                    //    // 强制终止
                    //    Process.Kill();
                    //}
                    //Process.WaitForExit(3000);
                    //Process.Dispose();

                    var killProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = $"/PID {Process.Id} /F /T",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true
                        }
                    };
                    killProcess.Start();
                    killProcess.WaitForExit(5000);

                    // 等待原进程退出
                    Process.WaitForExit(3000);
                    Process.Dispose();
                }
                catch (Exception ex)
                {
                    //todo
                    Debug.WriteLine($"停止进程失败: {ex.Message}");
                }

            }
        }
    }
}
