/*
 * This file is part of FacialEmotionRecognitionHub.
 * Copyright (C) 2026 周诣成（Yicheng Zhou）XINICOM@outlook.com
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private Command command = command;
        public bool Determinate = false;
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
                    result = await command(_invoker, null);
                }
                else
                {
                    result = await command(_invoker, TemplateParameters);
                }
            }
            else
            {
                ArgumentNullException.ThrowIfNull(TemplateParameters, $"Because the inner template parameters exists, the {nameof(parameters)} cannot be null");
                if (parameters.Keys.ToHashSet().SetEquals(TemplateParameters.Keys))
                {
                    result = await command(_invoker, parameters);
                }
                else
                {
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
