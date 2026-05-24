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

        //private bool _executable;
        private Command command = command;

        //public bool ExecutabilityUpdate(Func<ModifiableInstruction, bool>? predicate = null)
        //{
        //    if (InstructionBefore is null)
        //        return true;
        //    else
        //        return predicate(InstructionBefore);
        //}


        //todo 验证返回是否满足模板样式
        public async Task<Dictionary<string, object>?> Execute(object? invoker = null, Dictionary<string, object>? parameters = null)
        {
            Debug.WriteLine("try to execute");
            if (command is null)//!_executable || 
            {
                //Debug.WriteLine("0");
                throw new InvalidOperationException(nameof(command) + " should not be null");
                //return null;

            }
            object _invoker = invoker is null ? this : invoker;
            if (parameters is null)
            {
                if (TemplateParameters is null)
                {
                    //await command(invoker, null);
                    return await command(_invoker, null);
                }
                else
                {
                    //await command(invoker, TemplateParameters);
                    return await command(_invoker, TemplateParameters);
                }
            }
            else
            {
                if (TemplateParameters is null)
                {
                    ArgumentNullException.ThrowIfNull(parameters, $"Because the inner template parameters exists, the {nameof(parameters)} cannot be null");
                    return null;
                }
                else
                {
                    if (parameters.Keys.ToHashSet().SetEquals(TemplateParameters.Keys))
                    {
                        //await command(invoker, parameters);
                        return await command(_invoker, parameters);
                    }
                    else
                    {
                        if (parameters.Keys.Except(TemplateParameters.Keys) is not null)
                            throw new InvalidOperationException($"{nameof(parameters)} has new key which was not determinated in the template parameters");
                        else
                        {
                            var fullParameters = TemplateParameters.Concat(parameters).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
                            return await command(_invoker, fullParameters);
                        }
                    }
                }
            }
        }

        public void AddParameter(string parameterName, object templateValue)
        {
            if (_templateParameters is null)
                _templateParameters = new();
            if (_templateParameters.Any(x => x.Key != parameterName))
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
