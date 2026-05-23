using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.UI;
using FacialEmotionRecognitionHub.Bus.Models;
using Windows.Storage;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FacialEmotionRecognitionHub.Bus.Services
{
    public class AIModelsManager
    {
        private StorageFolder storageFolder;
        private List<AIModelM> _runningAIModels;
        public List<AIModelM> RunningAIModels
        {
            get
            {
                return _runningAIModels;
            }
        }
        public AIModelsManager(StorageFolder storageFolder)
        {
            this.storageFolder = storageFolder;
            _runningAIModels = [];
        }

        //todo
        public void CreatNewAIModel()
        {

        }

        public void CreatNewAIModelInstruction()
        {
            //todo

            string jsonSource = 
            @"{
                ""before"": ""preprocessing"",
                ""name"": ""train_stream"",
                ""prama"":{
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
                },
            }";
            JObject json = JObject.Parse(jsonSource);


        }
    }
}
