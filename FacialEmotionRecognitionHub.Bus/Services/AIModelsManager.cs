using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using Windows.Web.UI;

namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class AIModelsManager
    {
        private StorageFolder _storageFolder;
        private List<AIModelM> _runningAIModels;
        private IInterpreter _interpreter;
        public List<AIModelM> RunningAIModels
        {
            get
            {
                return _runningAIModels;
            }
        }
        public AIModelsManager(StorageFolder storageFolder, IInterpreter interpreter)
        {
            this._storageFolder = storageFolder;
            this._interpreter = interpreter;
            _runningAIModels = [];
        }

        //todo
        public void CreatNewAIModel()
        {

        }

        public async void CreatNewAIModelInstruction()
        {
            //todo

            string jsonSource =
            @"{
                ""before"": ""preprocessing"",
                ""name"": ""train_stream"",
                ""param"":{
                    ""model_type"": ""CNN"",
                    ""epochs"": 20,
                    ""lr"": 0.001,
                    ""weight_decay"": 1e-4,
                    ""load_model_path"": ""path"",
                    ""save_path"": ""Back_end/model_save/model_CNN.pth"",
                    ""verbose"": true,
                },
                ""return"":{
                    ""successful"": ""error"",
                },
                ""determinate"": {
                    ""total_epoch"": 20,
                    ""current_epoch"": 1,
                    ""train_loss"": 0.7966,
                    ""train_acc"": 0.7064,
                    ""val_loss"": 1.1592,
                    ""val_acc"": 0.5915,
                }
            }";
            //var JsonInterpreterService = new JsonInterpreterService();
            var instruction = _interpreter.InstructionGenerator(jsonSource,  async (invoker, parameters) =>
            {

                Debug.WriteLine($"INVOKER: {invoker.ToString()}");
                Debug.WriteLine($"USER: {_interpreter}");
                Debug.WriteLine($"PARAMETERS:");
                foreach (var parameter in parameters)
                {
                    Debug.WriteLine($"{parameter.Key} = {parameter.Value}(TYPE: {parameter.Value.GetType()})");
                }
                var result =
                @"{
                    ""successful"": ""error"",
                }";
                var r = _interpreter.InterpretArgument(JObject.Parse(result));
                foreach(var rx in r)
                {
                    Debug.WriteLine($"{rx.Key} = {rx.Value} (TYPE: {rx.Value.GetType()})");
                }
                return r;
            });
            //Dictionary<string,object> newDict = new();
            //newDict["model_type"] = "newModelType";
            await instruction.Execute();
            //await instruction.Execute(newDict);

            //JObject json = JObject.Parse(jsonSource);

            //string iName = json["name"]?.ToString();
            //if (iName is null)
            //    return;
            //ModifiableInstruction instruction = new ModifiableInstruction(iName, async (invoker, parameters) =>
            //{
            //    Debug.WriteLine($"INVOKER: {invoker.ToString()}");
            //    Debug.WriteLine($"PARAMETERS:");
            //    foreach(var parameter in parameters)
            //    {
            //        Debug.WriteLine($"{parameter.Key} = {parameter.Value}（TYPE: {parameter.Value.GetType()}）");
            //    }
            //    return null;
            //});
            //JObject prama = json["param"] as JObject;
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
            //        else if(float.TryParse(value, out float f))
            //        {
            //            instruction.AddParameter(key, f);
            //        }
            //        else if(IsValidFilePathFormat(value))
            //        {
            //            instruction.AddParameter(key, Path.GetExtension(value));
            //        }
            //        else if(value == "True")
            //        {
            //            instruction.AddParameter(key, true);
            //        }
            //        else if(value == "False")
            //        {
            //            instruction.AddParameter(key, false);
            //        }
            //        else
            //        {
            //            instruction.AddParameter(key, value);
            //        }
            //    }
            //}
            //JObject r = json["return"] as JObject;
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
            //        else if(float.TryParse(value, out float f))
            //        {
            //            instruction.AddReturn(key, f);
            //        }
            //        else if(IsValidFilePathFormat(value))
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
        }

        //public static bool IsValidFilePathFormat(string path)
        //{
        //    if (string.IsNullOrWhiteSpace(path))
        //        return false;
        //    try
        //    {
        //        if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
        //            path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        //            return false;
        //        string fileName = Path.GetFileName(path);
        //        if (string.IsNullOrWhiteSpace(fileName) || !fileName.Contains('.'))
        //            return false;
        //        if (fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        //            return false;
        //        string fullPath = Path.GetFullPath(path);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
