using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace CognitiveServices.Explorer.Services.DragAndDrop
{
    public class DropConfiguration : DependencyObject
    {
        public static readonly DependencyProperty DropStorageItemsCommandProperty = DependencyProperty.Register("DropStorageItemsCommand", typeof(ICommand), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DropDataViewCommandProperty = DependencyProperty.Register("DropDataViewCommand", typeof(ICommand), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragEnterCommandProperty = DependencyProperty.Register("DragEnterCommand", typeof(ICommand), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragOverCommandProperty = DependencyProperty.Register("DragOverCommand", typeof(ICommand), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragLeaveCommandProperty = DependencyProperty.Register("DragLeaveCommand", typeof(ICommand), typeof(DropConfiguration), new PropertyMetadata(null));

        public ICommand DropStorageItemsCommand
        {
            get { return (ICommand)GetValue(DropStorageItemsCommandProperty); }
            set { SetValue(DropStorageItemsCommandProperty, value); }
        }

        public ICommand DropDataViewCommand
        {
            get { return (ICommand)GetValue(DropDataViewCommandProperty); }
            set { SetValue(DropDataViewCommandProperty, value); }
        }

        public ICommand DragEnterCommand
        {
            get { return (ICommand)GetValue(DragEnterCommandProperty); }
            set { SetValue(DragEnterCommandProperty, value); }
        }

        public ICommand DragOverCommand
        {
            get { return (ICommand)GetValue(DragOverCommandProperty); }
            set { SetValue(DragOverCommandProperty, value); }
        }

        public ICommand DragLeaveCommand
        {
            get { return (ICommand)GetValue(DragLeaveCommandProperty); }
            set { SetValue(DragLeaveCommandProperty, value); }
        }

        public async Task ProcessComandsAsync(DataPackageView dataview)
        {
            if (DropDataViewCommand != null)
                DropDataViewCommand.Execute(dataview);

            if (dataview.Contains(StandardDataFormats.StorageItems) && DropStorageItemsCommand != null)
            {
                IReadOnlyList<IStorageItem> storageItems = await dataview.GetStorageItemsAsync();
                DropStorageItemsCommand.Execute(storageItems);
            }
        }
    }
}
