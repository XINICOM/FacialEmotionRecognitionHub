using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacialEmotionRecognitionHub.Bus.Services;
using System.Dynamic;

namespace FacialEmotionRecognitionHub.Bus.Models
{
    public class AIModelM
    {
        public string Name;
        private AIModelConnectionService _aIModelConnectionService;
        private dynamic _instructionSet = new ExpandoObject();

        public AIModelM(AIModelConnectionService aIModelConnectionService, string instructionSet)
        {
            //Name = name;
            _aIModelConnectionService = aIModelConnectionService;
        }
    }
}
