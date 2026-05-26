using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public enum ModelStatus
    {
        Error,
        DeterminatedProcessing,
        IndeterminatedProcessing,
        Pause,
    }

    public class AIModelM : IDisposable
    {
        private Process Process;

        public AIModelM(DateTime id, string dependenceEXEPath, int port, string modelConfigJson, string name)
        {
            ModelConfigJson = modelConfigJson;
            DependenceEXEPath = dependenceEXEPath;
            Port = port;
            Name = name;
            _id = id;

            _modifiableInstructionSet = [];

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://127.0.0.1:{Port}");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            //Debug.WriteLine("EXE = " + dependenceEXEPath);
            //Debug.WriteLine("PORT = " + port.ToString());
            //Debug.WriteLine("CONFIG = " + modelConfigJson);
            //Debug.WriteLine("NAME = " + name);

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

                // 进程退出事件
                Process.Exited += Process_Exited;
                Process.EnableRaisingEvents = true;

                Process.Start();
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();

                Debug.WriteLine($"模型服务已启动，端口: {port}");
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
                Debug.WriteLine($">>>[输出] {e.Data}");
                // 或者更新 UI
            }
            //throw new NotImplementedException();
        }

        public readonly HttpClient httpClient;

        private bool _showError = false;
        public bool ShowError { get { return _showError; } }
        private bool _showPaused = false;
        public bool ShowPaused { get { return _showPaused; } }
        private bool _isIndeterminate = true;
        public bool IsIndeterminate { get { return _isIndeterminate; } }
        private string _status = "No Progress";
        public string Status { get { return _status; } }
        private float _value = 0.0f;
        public float Value { get { return _value; } }
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
        private List<ModifiableInstruction> _modifiableInstructionSet;

        //public AIModelM(AIModelConnectionService aIModelConnectionService, string instructionSet)
        //{
        //    //Name = name;
        //    _modificationset = [];
        //    _aIModelConnectionService = aIModelConnectionService;
        //}



        public void SetModelStatus(ModelStatus target, float progress = 0)
        {
            if(target == ModelStatus.Error)
            {
                _showError = true;
                _showPaused = false;
                _status = "ERROR";
                _value = 0;
            }
            else if(target == ModelStatus.Pause)
            {
                _showError = false;
                _showPaused = true;
                _status = "PAUSE";
            }
            else if(target == ModelStatus.IndeterminatedProcessing)
            {
                _showError = false;
                _showPaused = false;
                _isIndeterminate = true;
                _status = "PROCESSING";
            }
            else
            {
                _showError = false;
                _showPaused = false;
                _isIndeterminate = false;
                _status = "PROCESSING";
                _value = 0;
            }
        }


        public void AddNewInstruction(ModifiableInstruction newInstruction)
        {
            if (_modifiableInstructionSet is null)
                _modifiableInstructionSet = new();

            if (_modifiableInstructionSet.Any(x => x.InstructionName == newInstruction.InstructionName))
            {
                //_modificationset.First(x=>x.InstructionName == newInstruction.InstructionName) = newInstruction;
                _modifiableInstructionSet[_modifiableInstructionSet.FindIndex(x => x.InstructionName == newInstruction.InstructionName)] = newInstruction;
            }
            else
            {
                _modifiableInstructionSet.Add(newInstruction);
            }
        }

        public ModifiableInstruction GetInstruction(string instructionName)
        {
            if (_modifiableInstructionSet.Any(x => x.InstructionName == instructionName))
            {
                //return _modifiableInstructionSet.First(x => x.InstructionName == instructionName);
                return _modifiableInstructionSet[_modifiableInstructionSet.FindIndex(x => x.InstructionName == instructionName)];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            Debug.WriteLine("===MODEL D");
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
                    Debug.WriteLine($"停止进程失败: {ex.Message}");
                }

            }
        }
    }
}
