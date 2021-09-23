using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CognitiveServices.Explorer.Helpers
{
    public class StorageFileToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return default;

            IRandomAccessStream stream = null;

            Task task = Task.Run(async () =>
            {
                stream = await (value as StorageFile).OpenAsync(FileAccessMode.Read);
            });

            task.Wait();

            BitmapImage bitmapImage = new();

            bitmapImage.SetSource(stream);

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
