using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Models;

namespace FacialEmotionRecognitionHub.Bus.Services
{
    public interface IInterpreter
    {
        public ModifiableInstruction InstructionGenerator(object source, Command command);
        public Dictionary<string, object> InterpretArgument(object source);
    }
}
