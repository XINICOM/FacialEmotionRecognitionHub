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
