using System.Numerics;

using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.ViewModels;

using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;

namespace CognitiveServices.Explorer.Views;

public sealed partial class PictureAnalysePage : Page
{
    private CanvasVirtualBitmap? canvasBitmap;

    public PictureAnalyseViewModel ViewModel { get; }

    public PictureAnalysePage()
    {
        ViewModel = App.GetService<PictureAnalyseViewModel>();
        InitializeComponent();
    }

    private void ImageCanvas_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ViewModel.ImageInfoService.File != null)
            ImageCommandBar?.ShowAt(ContentArea, new() { Position = new(ContentArea.ActualWidth / 2, 50), ShowMode = FlyoutShowMode.Transient });
    }

    private void ImageCanvas_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (ViewModel.ImageInfoService.File != null)
            ImageMenu?.ShowAt(ContentArea, e.GetPosition(ContentArea));
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

        if (ViewModel.ImageInfoService.File != null)
            args.TrackAsyncAction(LoadBitmap().AsAsyncAction());
    }

    private async void OpenImage_Click(object sender, RoutedEventArgs e)
    {
        StorageFile? file = await ViewModel.OpenImageAsync();

        if (file != null)
            await LoadBitmap();
    }

    private async Task LoadBitmap()
    {
        if (canvasBitmap != null)
        {
            canvasBitmap.Dispose();
            canvasBitmap = null;
        }

        canvasBitmap = await CanvasVirtualBitmap.LoadAsync(ImageCanvas.Device, await ViewModel.ImageInfoService.File?.OpenReadAsync());

        if (ImageCanvas == null)
            return;

        Size size = canvasBitmap.Size;
        ImageCanvas.Width = size.Width;
        ImageCanvas.Height = size.Height;

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

        var mouseX = e.GetCurrentPoint(ImageCanvas).Position.X;
        var mouseY = e.GetCurrentPoint(ImageCanvas).Position.Y;

        var mouseOverFace = false;

        foreach (PersonInfo personInfo in ViewModel.ImageInfoService.People)
        {
            FaceRectangle detectedFaceRectangle = personInfo.DetectedFace.FaceRectangle;

            if (mouseX >= detectedFaceRectangle.Left && mouseX <= detectedFaceRectangle.Left + detectedFaceRectangle.Width && mouseY >= detectedFaceRectangle.Top && mouseY <= detectedFaceRectangle.Top + detectedFaceRectangle.Height)
            {
                if (ToolTipService.GetToolTip(ImageCanvas) == null)
                    ToolTipService.SetToolTip(ImageCanvas, new ToolTip());

                ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).IsOpen = true;
                ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).Content = PictureAnalyseViewModel.GetPersonInformations(personInfo);
                ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).Placement = PlacementMode.Bottom;
                ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).PlacementRect = new(mouseX, mouseY, 0, 0);
                ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).VerticalOffset = 20;

                mouseOverFace = true;

                break;
            }
        }

        if (!mouseOverFace && ToolTipService.GetToolTip(ImageCanvas) != null)
        {
            ((ToolTip)ToolTipService.GetToolTip(ImageCanvas)).IsOpen = false;
            ToolTipService.SetToolTip(ImageCanvas, null);
        }
    }

    private void ContentArea_DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }

    private async void ContentArea_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1 && items[0] is StorageFile file && file.ContentType.StartsWith("image/"))
            {
                ViewModel.ImageInfoService.File = file;

                await LoadBitmap();
            }
        }
    }
}
