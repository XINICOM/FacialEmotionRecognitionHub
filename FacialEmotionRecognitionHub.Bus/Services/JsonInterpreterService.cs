using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Models;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;

namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class JsonInterpreterService : IInterpreter
    {
        public Dictionary<string, object> InterpretArgument(object source)
        {
            Dictionary<string, object> result = new();
            if (source is not null)
            {
                foreach (var p in ((JObject)source).Properties())
                {
                    string key = p.Name.ToString();
                    string value = p.Value.ToString();
                    if (int.TryParse(value, out int i))
                    {
                        result.Add(key, i);
                    }
                    else if (float.TryParse(value, out float f))
                    {
                        result.Add(key, f);
                    }
                    else if (IsValidFilePathFormat(value))
                    {
                        result.Add(key, Path.GetExtension(value));
                    }
                    else if (value == "True")
                    {
                        result.Add(key, true);
                    }
                    else if (value == "False")
                    {
                        result.Add(key, false);
                    }
                    else if (key == "successful")
                    {
                        result.Add(key, "0");
                    }
                    else
                    {
                        result.Add(key, value);
                    }
                }
            }
            return result;
        }
        public ModifiableInstruction InstructionGenerator(object source, Command command)
        {
            ArgumentNullException.ThrowIfNull(source);
            JObject json = JObject.Parse(source.ToString());
            string instructionName = json["name"]?.ToString();
            if (instructionName is null)
                throw new ArgumentNullException(nameof(instructionName));
            ArgumentNullException.ThrowIfNull(command);
            ModifiableInstruction instruction = new ModifiableInstruction(instructionName, command);
            JObject prama = json["param"] as JObject;
            if(prama is not null)
                foreach(var p in InterpretArgument(prama))
                {
                    instruction.AddParameter(p.Key, p.Value);
                }
            //if (prama is not null)
            //{
            //    foreach (var p in prama.Properties())
            //    {
            //        string key = p.Name.ToString();
            //        string value = p.Value.ToString();
            //        if (int.TryParse(value, out int i))
            //        {
            //            instruction.AddParameter(key, i);
            //        }
            //        else if (float.TryParse(value, out float f))
            //        {
            //            instruction.AddParameter(key, f);
            //        }
            //        else if (IsValidFilePathFormat(value))
            //        {
            //            instruction.AddParameter(key, Path.GetExtension(value));
            //        }
            //        else if (value == "True")
            //        {
            //            instruction.AddParameter(key, true);
            //        }
            //        else if (value == "False")
            //        {
            //            instruction.AddParameter(key, false);
            //        }
            //        else
            //        {
            //            instruction.AddParameter(key, value);
            //        }
            //    }
            //}
            JObject rparam = json["return"] as JObject;
            if(rparam is not null)
                foreach (var p in InterpretArgument(rparam))
                {
                    instruction.AddReturn(p.Key, p.Value);
                }
            //if (r is not null)
            //{
            //    foreach (var p in r.Properties())
            //    {
            //        string key = p.Name.ToString();
            //        string value = p.Value.ToString();
            //        if (int.TryParse(value, out int i))
            //        {
            //            instruction.AddReturn(key, i);
            //        }
            //        else if (float.TryParse(value, out float f))
            //        {
            //            instruction.AddReturn(key, f);
            //        }
            //        else if (IsValidFilePathFormat(value))
            //        {
            //            instruction.AddReturn(key, Path.GetExtension(value));
            //        }
            //        else if (value == "true")
            //        {
            //            instruction.AddReturn(key, true);
            //        }
            //        else if (value == "false")
            //        {
            //            instruction.AddReturn(key, false);
            //        }
            //        else
            //        {
            //            instruction.AddReturn(key, value);
            //        }
            //    }
            //}
            return instruction;
        }
        public static bool IsValidFilePathFormat(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            try
            {
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                    path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    return false;
                string fileName = Path.GetFileName(path);
                if (string.IsNullOrWhiteSpace(fileName) || !fileName.Contains('.'))
                    return false;
                if (fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                    return false;
                string fullPath = Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
