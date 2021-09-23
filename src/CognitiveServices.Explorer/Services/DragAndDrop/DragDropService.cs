using CognitiveServices.Explorer.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;

namespace CognitiveServices.Explorer.Services.DragAndDrop
{
    public class DragDropService : DependencyObject
    {
        private static DependencyProperty configurationProperty = DependencyProperty.RegisterAttached("Configuration", typeof(DropConfiguration), typeof(DragDropService), new PropertyMetadata(null, OnConfigurationPropertyChanged));

        private static DependencyProperty visualConfigurationProperty = DependencyProperty.RegisterAttached("VisualConfiguration", typeof(VisualDropConfiguration), typeof(DragDropService), new PropertyMetadata(null, OnVisualConfigurationPropertyChanged));

        public static void SetConfiguration(DependencyObject dependencyObject, DropConfiguration value)
        {
            if (dependencyObject != null)
                dependencyObject.SetValue(configurationProperty, value);
        }

        public static DropConfiguration GetConfiguration(DependencyObject dependencyObject)
        {
            return (DropConfiguration)dependencyObject.GetValue(configurationProperty);
        }

        public static void SetVisualConfiguration(DependencyObject dependencyObject, VisualDropConfiguration value)
        {
            if (dependencyObject != null)
                dependencyObject.SetValue(visualConfigurationProperty, value);
        }

        public static VisualDropConfiguration GetVisualConfiguration(DependencyObject dependencyObject)
        {
            return (VisualDropConfiguration)dependencyObject.GetValue(visualConfigurationProperty);
        }

        private static void OnConfigurationPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dependencyObject as UIElement;
            DropConfiguration configuration = GetConfiguration(element);
            ConfigureUIElement(element, configuration);
        }

        private static void ConfigureUIElement(UIElement element, DropConfiguration configuration)
        {
            element.DragEnter += (sender, args) =>
            {
                args.AcceptedOperation = DataPackageOperation.Copy;

                DragDropData data = new() { AcceptedOperation = args.AcceptedOperation, DataView = args.DataView };
                configuration.DragEnterCommand?.Execute(data);
                args.AcceptedOperation = data.AcceptedOperation;
            };

            element.DragOver += (sender, args) =>
            {
                DragDropData data = new() { AcceptedOperation = args.AcceptedOperation, DataView = args.DataView };
                configuration.DragOverCommand?.Execute(data);
                args.AcceptedOperation = data.AcceptedOperation;
            };

            element.DragLeave += (sender, args) =>
            {
                DragDropData data = new() { AcceptedOperation = args.AcceptedOperation, DataView = args.DataView };
                configuration.DragLeaveCommand?.Execute(data);
            };

            element.Drop += async (sender, args) =>
            {
                await configuration.ProcessComandsAsync(args.DataView);
            };
        }

        private static void OnVisualConfigurationPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dependencyObject as UIElement;
            VisualDropConfiguration configuration = GetVisualConfiguration(element);

            element.DragStarting += (sender, args) =>
            {
                if (configuration.DropOverImage != null)
                    args.DragUI.SetContentFromBitmapImage(configuration.DragStartingImage as BitmapImage);
            };

            element.DragOver += (sender, args) =>
            {
                args.DragUIOverride.Caption = configuration.Caption;
                args.DragUIOverride.IsCaptionVisible = configuration.IsCaptionVisible;
                args.DragUIOverride.IsContentVisible = configuration.IsContentVisible;
                args.DragUIOverride.IsGlyphVisible = configuration.IsGlyphVisible;

                if (configuration.DropOverImage != null)
                    args.DragUIOverride.SetContentFromBitmapImage(configuration.DropOverImage as BitmapImage);
            };
        }
    }
}
