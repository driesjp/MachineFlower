using MachineFlowers.Interfaces;
using System.Windows;

namespace MachineFlowers.Services
{
    public class MessageBoxService : IMessageService
    {
        public void ShowMessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}