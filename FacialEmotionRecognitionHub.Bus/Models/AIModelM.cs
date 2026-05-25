using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;
using System.Dynamic;
using System.IO;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public enum ModelStatus
    {
        Error,
        DeterminatedProcessing,
        IndeterminatedProcessing,
        Pause,
    }
    public class AIModelM
    {
        public AIModelM(string dependenceEXEPath, int port, string modelConfigFolder, string name)
        {
            ModelConfigFolder = dependenceEXEPath;
            DependenceEXEPath = dependenceEXEPath;
            Port = port;
            Name = name;
        }

        private bool _showError = false;
        public bool ShowError { get { return _showError; } }
        private bool _showPaused = false;
        public bool ShowPaused { get { return _showPaused; } }
        private bool _isIndeterminate = true;
        public bool IsIndeterminate { get { return _isIndeterminate; } }
        private string _status = "Default Status";
        public string Status { get { return _status; } }
        private float _value = 0.0f;
        public float Value { get { return _value; } }

        public string Name = string.Empty;
        public string ModelConfigFolder = string.Empty;
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
        private List<ModifiableInstruction> _modificationset = [];

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
            if (_modificationset is null)
                _modificationset = new();

            if (_modificationset.Any(x => x.InstructionName == newInstruction.InstructionName))
            {
                //_modificationset.First(x=>x.InstructionName == newInstruction.InstructionName) = newInstruction;
                _modificationset[_modificationset.FindIndex(x => x.InstructionName == newInstruction.InstructionName)] = newInstruction;
            }
            else
            {
                _modificationset.Add(newInstruction);
            }
        }

        public ModifiableInstruction GetInstruction(string instructionName)
        {
            if (_modificationset.Any(x => x.InstructionName == instructionName))
            {
                return _modificationset.First(x => x.InstructionName == instructionName);
            }
            else
            {
                return null;
            }
        }
    }
}
