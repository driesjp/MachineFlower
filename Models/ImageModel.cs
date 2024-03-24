namespace MachineFlowers.Models
{
    // Represents the data model for an image within the application.
    public class ImageModel
    {
        // This property gets or sets the file path of the image.
        // It allows for data binding in an MVVM architecture, facilitating the display of the image in the UI based on its file path.
        public string ImagePath { get; set; }
    }
}