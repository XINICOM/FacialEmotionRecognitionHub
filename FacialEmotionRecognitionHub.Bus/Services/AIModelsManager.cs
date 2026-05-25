using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<AIModelM> _runningAIModels;
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

            //todo
            //_runningAIModels.Add(new AIModelM("C:\\Users\\XINIC\\AppData\\Local\\Packages\\fe185a02-4a03-45e3-8bb8-99f77cd78caf_6jqbvxqyaede0\\LocalState\\dependence\\SAMPLE.exe",5000));
        }

        //public void Pause(AIModelM target)
        //{
        //    _runningAIModels[_runningAIModels.IndexOf(target)].SetModelStatus(ModelStatus.Pause);
        //}

        //todo
        public void CreatNewAIModel()
        {
            //todo
            //_runningAIModels.Add(new AIModelM("C:\\Users\\XINIC\\AppData\\Local\\Packages\\fe185a02-4a03-45e3-8bb8-99f77cd78caf_6jqbvxqyaede0\\LocalState\\dependence\\SAMPLE.exe", 5000));
            //_runningAIModels[0].Status = "CHANGED";
        }

        //todo
        public async void CreatNewAIModelInstruction()
        {
            Debug.WriteLine($"START");
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
            Debug.WriteLine($"SETTING");
            var instruction = _interpreter.InstructionGenerator(jsonSource, async (invoker, parameters) =>
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
                foreach (var rx in r)
                {
                    Debug.WriteLine($"{rx.Key} = {rx.Value} (TYPE: {rx.Value.GetType()})");
                }
                return r;
            });

            Debug.WriteLine($"Invoke");
            ////var JsonInterpreterService = new JsonInterpreterService();
            //var instruction = _interpreter.InstructionGenerator(jsonSource,  async (invoker, parameters) =>
            //{

            //    Debug.WriteLine($"INVOKER: {invoker.ToString()}");
            //    Debug.WriteLine($"USER: {_interpreter}");
            //    Debug.WriteLine($"PARAMETERS:");
            //    foreach (var parameter in parameters)
            //    {
            //        Debug.WriteLine($"{parameter.Key} = {parameter.Value}(TYPE: {parameter.Value.GetType()})");
            //    }
            //    var result =
            //    @"{
            //        ""successful"": ""error"",
            //    }";
            //    var r = _interpreter.InterpretArgument(JObject.Parse(result));
            //    foreach(var rx in r)
            //    {
            //        Debug.WriteLine($"{rx.Key} = {rx.Value} (TYPE: {rx.Value.GetType()})");
            //    }
            //    return r;
            //});
            Dictionary<string,object> newDict = new()
            {
                //newDict["model_type"] = "newModelType";
                { "epochs", 10 }
            };
            //await instruction.Execute();
            try
            {

                //await instruction.Execute();
                await instruction.Execute(this, newDict);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

    }
}
