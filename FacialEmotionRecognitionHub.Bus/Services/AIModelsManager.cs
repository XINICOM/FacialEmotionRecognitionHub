using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.UI;
using FacialEmotionRecognitionHub.Bus.Models;
using Windows.Storage;

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
            //todo
            //_runningAIModels.Add(new AIModelM("model4"));
        }
    }
}
