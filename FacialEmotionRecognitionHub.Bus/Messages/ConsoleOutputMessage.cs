using System;
namespace FacialEmotionRecognitionHub.Bus.Messages
{
    public class ConsoleOutputMessage(DateTime id)
    {
        public DateTime ID { get; set; } = id;
    }
}
