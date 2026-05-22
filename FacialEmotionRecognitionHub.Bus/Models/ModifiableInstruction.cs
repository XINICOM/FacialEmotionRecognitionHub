using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace FacialEmotionRecognitionHub.Bus.Models
{
    public delegate Task Command(Dictionary<string, object>? parameters);

    public class ModifiableInstruction(string instructionName, Command command)
    {
        private string _instructionName = instructionName;
        public string InstructionName
        {
            get { return _instructionName; }
        }
        public ModifiableInstruction? InstructionBefore;
        private Dictionary<string, object>? _templateParameters;
        public Dictionary<string, object>? TemplateParameters
        {
            get
            {
                return _templateParameters;
            }
        }
        private bool _executable = false;
        private Command command = command;
        public async void Execute(Dictionary<string, object>? parameters = null)
        {
            if (!_executable || command is null)
                return;
            if(parameters is null)
            {
                if(TemplateParameters is null)
                {
                    await command(null);
                    return;
                }
                else
                {
                    await command(TemplateParameters);
                    return;
                }
            }
            else
            {
                if(TemplateParameters is null)
                {
                    return;
                }
                else
                {
                    if (parameters.Keys.ToHashSet().SetEquals(TemplateParameters.Keys))
                    {
                        await command(parameters);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public void AddParameter(string parameterName, object value)
        {
            if (_templateParameters is null)
                _templateParameters = new();
            _templateParameters.Add(parameterName, value);
        }
    }
}
