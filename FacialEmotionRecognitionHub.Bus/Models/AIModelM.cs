using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using FacialEmotionRecognitionHub.Bus.Messages;
using FacialEmotionRecognitionHub.Bus.Services;
using Newtonsoft.Json.Linq;
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
        public bool needReturnJSON = false;
        private AIModelsManager _aIModelsManager;
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
                if (_console != value)
                {
                    _console = value;
                    if (value != string.Empty)
                    {
                        WeakReferenceMessenger.Default.Send(new AIModelInitializedMessage(this));
                        WeakReferenceMessenger.Default.Send(new ConsoleOutputMessage(ID));
                    }
                }
            }
        }
        public AIModelM(AIModelsManager manager, DateTime id, string dependenceEXEPath, int port, string modelConfigJson, string name)
        {
            _aIModelsManager = manager;
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
                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
                Process.Exited += Process_Exited;
                Process.EnableRaisingEvents = true;
                Process.Start();
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"启动失败: {ex.Message}");
                throw;
            }
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            Debug.WriteLine("模型服务已退出");
        }
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($">>>[错误] {e.Data}");
                var s = e.Data.ToLower();
                if (s.Contains("development server") || s.Contains("127.0.0.1 - - ") || s.Contains(" * running on http://127.0.0.1:") || s.Contains("press ctrl+c to quit"))
                    return;
                Console += $"[GETERROR]\n>>>>>>>>>>{e.Data}\n";
            }
        }
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                var result = e.Data;
                if (result[0] == '\uFEFF')
                {
                    Debug.WriteLine("TRUE");
                    result = result.Substring(1);
                }
                Debug.WriteLine(">>>>>>>>>>>>" + result);
                if (needReturnJSON)
                {
                    try
                    {
                        var json = JObject.Parse(result);
                        var returns = _aIModelsManager.Interpreter.InterpretArgument(json);
                        float t = 0f, c = 0f;
                        foreach (var item in returns)
                        {
                            if (item.Key.Contains("total", StringComparison.CurrentCultureIgnoreCase))
                                t = Convert.ToSingle(item.Value);
                            if (item.Key.Contains("current", StringComparison.CurrentCultureIgnoreCase))
                                c = Convert.ToSingle(item.Value);
                            if (item.Key.Contains("val", StringComparison.CurrentCultureIgnoreCase) && item.Key.Contains("acc", StringComparison.CurrentCultureIgnoreCase))
                                _accuracy = (Convert.ToSingle(item.Value) * 100).ToString("F1") + " %";
                        }
                        SetModelStatus(ModelStatus.DeterminatedProcessing, t != 0 ? c / t * 100 : 0);
                    }
                    catch
                    {
                        Debug.WriteLine("NO JSON");
                    }
                }
                Console += $"[RECEIVED]{e.Data}\n";
                Debug.WriteLine($">>>[输出] {e.Data}");
            }
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
        private string _accuracy = "--.-%";
        public string Accuracy { get { return _accuracy; } }
        public ModelStatus modelStatus = ModelStatus.Relax;
        private float _value = 0f;
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
        public List<ModifiableInstruction> ModifiableInstructionSet;
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
            else if (target == ModelStatus.DeterminatedProcessing)
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
            WeakReferenceMessenger.Default.Send(new ModelStatusMessage());
        }
        public void AddNewInstruction(ModifiableInstruction newInstruction)
        {
            if (ModifiableInstructionSet is null)
                ModifiableInstructionSet = new();
            if (ModifiableInstructionSet.Any(x => x.InstructionName == newInstruction.InstructionName))
            {
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
                return ModifiableInstructionSet[ModifiableInstructionSet.FindIndex(x => x.InstructionName == instructionName)];
            }
            else
            {
                return null;
            }
        }
        public void Dispose()
        {
            StopProcess();
            GC.SuppressFinalize(this);
        }
        private void StopProcess()
        {
            if (Process != null && !Process.HasExited)
            {
                try
                {
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
