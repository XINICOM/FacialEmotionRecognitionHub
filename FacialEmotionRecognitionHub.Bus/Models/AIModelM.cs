using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;
using System.Dynamic;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public class AIModelM(string name)
    {
        private int _runningPort = 5000;
        public int RunningPort
        {
            get
            {
                return _runningPort;
            }
        }
        public string Name = name;
        private AIModelConnectionService _aIModelConnectionService;
        private List<ModifiableInstruction> _modificationset = [];

        //public AIModelM(AIModelConnectionService aIModelConnectionService, string instructionSet)
        //{
        //    //Name = name;
        //    _modificationset = [];
        //    _aIModelConnectionService = aIModelConnectionService;
        //}

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
