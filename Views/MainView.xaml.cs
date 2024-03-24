using System.Windows;
using MachineFlowers.Interfaces;
using MachineFlowers.Services;
using MachineFlowers.ViewModels;

namespace MachineFlowers.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            // Set the DataContext of the view to a new instance of MainViewModel
            IMessageService messageService = new MessageBoxService();
            DataContext = new MainViewModel(messageService);
        }
    }
}