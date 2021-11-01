using CognitiveServices.Explorer.Core.Models;
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
            if (ViewModel.ImageInfoService.FilePath != null)
                FlyoutMenu?.ShowAt(ContentArea, new() { Position = new(ContentArea.ActualWidth / 2, 50), ShowMode = FlyoutShowMode.Transient });
        }

        private async void StartFaceDetection_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartFaceDetection();
            
            ImageCanvas.Invalidate();
        }
        
        private async void StartFaceRecognition_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartFaceRecognitionAsync();
            
            ImageCanvas.Invalidate();
        }

        private void ImageCanvas_CreateResources(CanvasVirtualControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.DpiChanged)
                return;

            if (ViewModel.ImageInfoService.FilePath != null)
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
                ViewModel.ImageInfoService.FilePath = file;

                await LoadBitmap();
            }
        }

        public async void OnGetStorageItem(IReadOnlyList<IStorageItem> items)
        {
            ViewModel.ImageInfoService.FilePath = items[0] as StorageFile;

            await LoadBitmap();
        }

        private async Task LoadBitmap()
        {
            if (canvasBitmap != null)
            {
                canvasBitmap.Dispose();
                canvasBitmap = null;
            }
            
            canvasBitmap = await CanvasVirtualBitmap.LoadAsync(ImageCanvas.Device, await ViewModel.ImageInfoService.FilePath.OpenReadAsync());

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

                foreach (PersonInfo personInfo in ViewModel.ImageInfoService.People)
                {
                    drawingSession.DrawRectangle(personInfo.DetectedFace.FaceRectangle.Left, personInfo.DetectedFace.FaceRectangle.Top, personInfo.DetectedFace.FaceRectangle.Width, personInfo.DetectedFace.FaceRectangle.Height, Colors.Blue, 3);

                    if (!string.IsNullOrEmpty(personInfo.Name))
                        drawingSession.DrawText(personInfo.Name, new Vector2(personInfo.DetectedFace.FaceRectangle.Left, personInfo.DetectedFace.FaceRectangle.Top + personInfo.DetectedFace.FaceRectangle.Height), Colors.AliceBlue);
                }
            }
        }

        private void ImageCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.ImageInfoService.People.Count == 0)
                return;

            double mouseX = e.GetCurrentPoint(ImageCanvas).Position.X;
            double mouseY = e.GetCurrentPoint(ImageCanvas).Position.Y;

            bool mouseOverFace = false;

            foreach (PersonInfo personInfo in ViewModel.ImageInfoService.People)
            {
                FaceRectangle detectedFaceRectangle = personInfo.DetectedFace.FaceRectangle;

                if (mouseX >= detectedFaceRectangle.Left && mouseX <= detectedFaceRectangle.Left + detectedFaceRectangle.Width && mouseY >= detectedFaceRectangle.Top && mouseY <= detectedFaceRectangle.Top + detectedFaceRectangle.Height)
                {
                    if (ToolTipService.GetToolTip(ImageCanvas) == null)
                        ToolTipService.SetToolTip(ImageCanvas, new ToolTip());

                    (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).IsOpen = true;
                    (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).Content = PictureAnalyseViewModel.GetPersonInformations(personInfo);
                    (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).Placement = PlacementMode.Bottom;
                    (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).PlacementRect = new(mouseX, mouseY, 0, 0);
                    (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).VerticalOffset = 20;

                    mouseOverFace = true;

                    break;
                }
            }
            
            if (!mouseOverFace && ToolTipService.GetToolTip(ImageCanvas) != null)
                (ToolTipService.GetToolTip(ImageCanvas) as ToolTip).IsOpen = false;
        }
    }
}
