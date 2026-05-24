using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls;

namespace FacialEmotionRecognitionHub.Bus.Messages
{
    public class SelectedTabChangedMessage : ValueChangedMessage<TabViewItem>
    {
        public SelectedTabChangedMessage(TabViewItem value) : base(value) { }
    }
}
