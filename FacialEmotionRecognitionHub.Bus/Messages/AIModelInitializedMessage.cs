using CommunityToolkit.Mvvm.Messaging.Messages;
using FacialEmotionRecognitionHub.Bus.Models;
namespace FacialEmotionRecognitionHub.Bus.Messages
{
    public class AIModelInitializedMessage : ValueChangedMessage<AIModelM>
    {
        public AIModelInitializedMessage(AIModelM value) : base(value) { }
    }
}
