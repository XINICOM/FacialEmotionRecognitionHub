using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //public ModifiableInstruction? InstructionBefore;
        private Dictionary<string, object>? _templateParameters;
        public Dictionary<string, object>? TemplateParameters
        {
            get { return _templateParameters; }
        }
        private Dictionary<string, object>? _templateReturn;
        public Dictionary<string, object>? TemplateReturn
        {
            get { return _templateReturn; }
        }

        //private bool _executable;
        private Command command = command;

        public bool determinate = false;

        //public bool ExecutabilityUpdate(Func<ModifiableInstruction, bool>? predicate = null)
        //{
        //    if (InstructionBefore is null)
        //        return true;
        //    else
        //        return predicate(InstructionBefore);
        //}

        public async Task<Dictionary<string, object>?> Execute(object? invoker = null, Dictionary<string, object>? parameters = null)
        {
            Debug.WriteLine("EXE2");
            var result = await ExecuteWithoutCheckingResult(invoker, parameters);
            if (result is not null && _templateReturn is not null)
            {
                if (result.Keys.ToHashSet().SetEquals(_templateReturn.Keys))
                    return result;
                else
                    throw new InvalidOperationException($"new arguments beyond {_templateReturn.Keys.ToHashSet().ToString()} occured");
            }
            else if (_templateReturn is null)
            {
                if (result is not null)
                    throw new InvalidOperationException($"The return should be null");
                else
                    return null;
            }
            else
                throw new InvalidOperationException("The return is missing required parameters");
        }


        public async Task<Dictionary<string, object>?> ExecuteWithoutCheckingResult(object? invoker = null, Dictionary<string, object>? parameters = null)
        {
            if (command is null)
            {
                throw new InvalidOperationException(nameof(command) + " should not be null");
            }
            object _invoker = invoker is null ? this : invoker;
            Dictionary<string, object>? result;
            if (parameters is null)
            {
                if (TemplateParameters is null)
                {
                    //await command(invoker, null);
                    result = await command(_invoker, null);
                }
                else
                {
                    //await command(invoker, TemplateParameters);
                    result = await command(_invoker, TemplateParameters);
                }
            }
            else
            {
                ArgumentNullException.ThrowIfNull(TemplateParameters, $"Because the inner template parameters exists, the {nameof(parameters)} cannot be null");
                if (parameters.Keys.ToHashSet().SetEquals(TemplateParameters.Keys))
                {
                    //await command(invoker, parameters);
                    result = await command(_invoker, parameters);
                }
                else
                {
                    //Debug.WriteLine(parameters.Keys.Except(TemplateParameters.Keys).Count());
                    if (parameters.Keys.Except(TemplateParameters.Keys).Any())
                        throw new InvalidOperationException($"{nameof(parameters)} has new key which was not determinated in the template parameters");
                    else
                    {
                        var fullParameters = TemplateParameters.Concat(parameters).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
                        result = await command(_invoker, fullParameters);
                    }
                }
                
            }
            return result;
        }

        public void AddParameter(string parameterName, object templateValue)
        {
            if (_templateParameters is null)
                _templateParameters = new();
            if (_templateParameters.Any(x => x.Key != parameterName))
            {
                Debug.WriteLine($"{parameterName} = {templateValue} ({templateValue.GetType()})");
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
