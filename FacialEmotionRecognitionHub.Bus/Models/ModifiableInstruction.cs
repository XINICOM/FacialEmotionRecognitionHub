using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace FacialEmotionRecognitionHub.Bus.Models
{
    public delegate Task<Dictionary<string, object>?> Command(object invoker, Dictionary<string, object>? parameters);

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
        private Dictionary<string, object>? _templateReturn;

        private bool _executable = false;
        private Command command = command;
        public async Task<Dictionary<string, object>?> Execute(object invoker, Dictionary<string, object>? parameters = null)
        {
            if (!_executable || command is null)
                return null;
            if(parameters is null)
            {
                if(TemplateParameters is null)
                {
                    //await command(invoker, null);
                    return await command(invoker, null);
                }
                else
                {
                    //await command(invoker, TemplateParameters);
                    return await command(invoker, TemplateParameters);
                }
            }
            else
            {
                if(TemplateParameters is null)
                {
                    return null;
                }
                else
                {
                    if (parameters.Keys.ToHashSet().SetEquals(TemplateParameters.Keys))
                    {
                        //await command(invoker, parameters);
                        return await command(invoker, parameters);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void AddParameter(string parameterName, object templateValue)
        {
            if (_templateParameters is null)
                _templateParameters = new();
            if(_templateParameters.Any(x=>x.Key != parameterName))
            {
                _templateParameters.Add(parameterName, templateValue);
            }
            else
            {
                _templateParameters[parameterName] = templateValue;
            }
        }

        public void AddReturn(string parameterName, object templateValue)
        {
            if (_templateReturn is null)
                _templateReturn = new();
            //_templateReturn.Add(parameterName, templateValue);
            if (_templateReturn.Any(x => x.Key != parameterName))
            {
                _templateReturn.Add(parameterName, templateValue);
            }
            else
            {
                _templateReturn[parameterName] = templateValue;
            }
        }
    }
}
