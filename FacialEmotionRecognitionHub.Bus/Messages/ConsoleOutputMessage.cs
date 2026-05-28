using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialEmotionRecognitionHub.Bus.Messages
{
    public class ConsoleOutputMessage(DateTime id)
    {
        public DateTime ID { get; set; } = id;

    }
}
