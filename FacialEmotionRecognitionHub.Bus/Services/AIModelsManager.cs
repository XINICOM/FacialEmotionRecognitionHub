using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using FacialEmotionRecognitionHub.Bus.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class AIModelsManager : IDisposable
    {
        public List<AIModelM> RunningAIModels;
        public IInterpreter Interpreter;
        private IOService _iOService;
        public AIModelsManager(IInterpreter interpreter, IOService iOService)
        {
            Interpreter = interpreter;
            RunningAIModels = [];
            _iOService = iOService;
        }
        public AIModelM CreatNewAIModel(string dependenceEXEPath, DateTime id)
        {
            string configJson = _iOService.SearchAndInitializeConfigJson(dependenceEXEPath);
            int port = _iOService.GetAvailablePort(IPAddress.Loopback);
            string name = Path.GetFileNameWithoutExtension(dependenceEXEPath);
            var newModel = new AIModelM(this, id, dependenceEXEPath, port, configJson, name);
            string jsonContent = File.ReadAllText(configJson);
            JObject root = JObject.Parse(jsonContent);
            if (root["instructionset"] is JArray jArray)
            {
                foreach (var i in jArray)
                {
                    newModel.AddNewInstruction(Interpreter.InstructionGenerator(i.ToString(), async (invoker, parameters) =>
                     {
                         if (invoker is ModifiableInstruction instruction)
                         {
                             try
                             {
                                 var httpPath = $"/{instruction.InstructionName}";
                                 if (instruction.TemplateParameters is not null)
                                 {
                                     var p = JsonConvert.SerializeObject(parameters);
                                     p = p.Replace('\\', ',').Replace('/', ',');
                                     httpPath += "/" + p;
                                     Debug.WriteLine(httpPath);
                                 }
                                 var request = new HttpRequestMessage(HttpMethod.Get, httpPath);
                                 var response = await newModel.httpClient.SendAsync(request);
                                 string content = await response.Content.ReadAsStringAsync();
                                 return Interpreter.InterpretArgument(JObject.Parse(content));
                             }
                             catch (Exception e)
                             {
                                 throw;
                             }
                         }
                         else
                         {
                         }
                         return null;
                     }));
                }
            }
            RunningAIModels.Add(newModel);
            return newModel;
        }
        public void Dispose()
        {
            foreach (var model in RunningAIModels)
            {
                model.Dispose();
            }
        }
        public void DisposeModel(DateTime id)
        {
            if (RunningAIModels.Any(x => x.ID == id))
            {
                var index = RunningAIModels.FindIndex(x => x.ID == id);
                var target = RunningAIModels[index];
                RunningAIModels.Remove(target);
                target.Dispose();
            }
        }
    }
}
