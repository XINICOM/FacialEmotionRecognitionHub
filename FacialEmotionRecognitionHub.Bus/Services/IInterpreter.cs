using System.Collections.Generic;
using FacialEmotionRecognitionHub.Bus.Models;
namespace FacialEmotionRecognitionHub.Bus.Services
{
    public interface IInterpreter
    {
        public ModifiableInstruction InstructionGenerator(object source, Command command);
        public Dictionary<string, object> InterpretArgument(object source);
        public string FormatInstructionName(string input);
    }
}
