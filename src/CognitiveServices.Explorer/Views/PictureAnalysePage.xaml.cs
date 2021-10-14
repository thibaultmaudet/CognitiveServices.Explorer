using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.ViewModels;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CognitiveServices.Explorer.Views
{
    public sealed partial class PictureAnalysePage : Page
    {
        private CanvasVirtualBitmap canvasBitmap;

        public PictureAnalyseViewModel ViewModel { get; }

        public Action<IReadOnlyList<IStorageItem>> GetStorageItem => ((items) => OnGetStorageItem(items));
        
        public PictureAnalysePage()
        {
            ViewModel = Ioc.Default.GetService<PictureAnalyseViewModel>();
            InitializeComponent();
        }

        private void ImageCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.OpenedImage != null)
                FlyoutMenu?.ShowAt(ContentArea, new() { Position = new(ContentArea.ActualWidth / 2, 50), ShowMode = FlyoutShowMode.Transient });
        }

        private async void StartFaceDetection_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartFaceDetection();
            
            ImageCanvas.Invalidate();
        }

        private void ImageCanvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.DpiChanged)
                return;

            if (ViewModel.OpenedImage != null)
                args.TrackAsyncAction(LoadBitmap().AsAsyncAction());
        }

        private async void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new() { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            WinUIConversionHelper.InitFileOpenPicker(openPicker);

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                ViewModel.OpenedImage = file;

                await LoadBitmap();
            }
        }

        public async void OnGetStorageItem(IReadOnlyList<IStorageItem> items)
        {
            ViewModel.OpenedImage = items[0] as StorageFile;

            await LoadBitmap();
        }

        private async Task LoadBitmap()
        {
            if (canvasBitmap != null)
            {
                canvasBitmap.Dispose();
                canvasBitmap = null;
            }
            
            canvasBitmap = await CanvasVirtualBitmap.LoadAsync(ImageCanvas.Device, await ViewModel.OpenedImage.OpenReadAsync());

            if (ImageCanvas == null)
                return;

            Size size = canvasBitmap.Size;
            ImageCanvas.Width = size.Width;
            ImageCanvas.Height =  size.Height;

            ImageCanvas.Invalidate();
        }

        private void ImageVirtualControl_RegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            foreach (var region in args.InvalidatedRegions)
            {
                using CanvasDrawingSession drawingSession = ImageCanvas.CreateDrawingSession(region);

                if (canvasBitmap != null)
                    drawingSession.DrawImage(canvasBitmap, region, region);

                if (ViewModel.DetectedFaces != null)
                    foreach (DetectedFace face in ViewModel.DetectedFaces)
                        drawingSession.DrawRectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height, Colors.Blue, 3);
            }
        }
    }
}
