using MachineFlowers.Models;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using MachineFlowers.Interfaces;

namespace MachineFlowers.ViewModels
{
    // MainViewModel class implementing INotifyPropertyChanged interface to support bindings with automatic UI updates
    public class MainViewModel : INotifyPropertyChanged
    {
        // Event declaration for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IMessageService _messageService;
        // Private field for the ImageModel which holds the image data
        private ImageModel _imageModel;
        public ImageModel ImageModel
        {
            get { return _imageModel; }
            set
            {
                _imageModel = value;
                OnPropertyChanged(nameof(ImageModel));
            }
        }

        // ICommand property for binding UI actions to commands
        public ICommand SelectImageCommand { get; private set; }

        // MainViewModel constructor where command bindings are set up
        public MainViewModel(IMessageService messageService)
        {
            _messageService = messageService;
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        // SelectImageCommand is executed
        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    ImageModel = new ImageModel { ImagePath = openFileDialog.FileName };
                    AnalyzeImageAndUpdateLabel();
                }
                else
                {
                    _messageService.ShowMessage("Error: Only PNG, JPG, and JPEG files are allowed.");
                }
            }
        }

        // Predict image category based on the image path provided
        static async Task<(string Key, double Percentage)> PredictImageAsync(string imagePath)
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            FlowersModel.ModelInput sampleData = new FlowersModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

            // Make a prediction using the ML model and get the scores
            var sortedScoresWithLabel = FlowersModel.PredictAllLabels(sampleData);

            var highestScore = sortedScoresWithLabel.OrderByDescending(score => score.Value).FirstOrDefault();
            if (highestScore.Key != null)   // Ensure there's at least one score
            {
                var scorePercentage = highestScore.Value * 100;
                return (highestScore.Key, scorePercentage);
            }

            return (null, 0);   // Default return value if no prediction is possible
        }

        // Call method to analyze the selected image and update the label accordingly
        private async void AnalyzeImageAndUpdateLabel()
        {
            ImageOutputLbl = "Please wait"; // In reality, it doesn't have time to display this. 
            // await Task.Delay(100);   // For debugging purposes. Test if the previous line works and the application runs asynchronously.

            // Asynchronously wait for the prediction to complete
            (string predictedLabel, double predictedScore) = await PredictImageAsync(ImageModel.ImagePath);

            // Update the label with the prediction result
            if (predictedLabel != null)
            {
                ImageOutputLbl = $"Closest match: {predictedLabel} with a confidence of {predictedScore:0.##}%";
            }
            else
            {
                ImageOutputLbl = "Unable to make a prediction.";
            }
        }

        // Property for the output label's text, with notification on change
        private string _imageOutputLbl;
        public string ImageOutputLbl
        {
            get { return _imageOutputLbl; }
            set
            {
                _imageOutputLbl = value;
                OnPropertyChanged(nameof(ImageOutputLbl));
            }
        }

        // Method to invoke the PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}