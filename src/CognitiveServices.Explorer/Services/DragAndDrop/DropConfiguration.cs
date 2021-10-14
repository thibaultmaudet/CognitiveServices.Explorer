using CognitiveServices.Explorer.Models;
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
        public static readonly DependencyProperty DropStorageItemsActionProperty =
            DependencyProperty.Register("DropStorageItemsAction", typeof(Action<IReadOnlyList<IStorageItem>>), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragEnterActionProperty =
            DependencyProperty.Register("DragEnterAction", typeof(Action<DragDropData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragOverActionProperty =
            DependencyProperty.Register("DragOverAction", typeof(Action<DragDropData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragLeaveActionProperty =
            DependencyProperty.Register("DragLeaveAction", typeof(Action<DragDropData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public Action<IReadOnlyList<IStorageItem>> DropStorageItemsAction
        {
            get { return (Action<IReadOnlyList<IStorageItem>>)GetValue(DropStorageItemsActionProperty); }
            set { SetValue(DropStorageItemsActionProperty, value); }
        }

        public Action<DragDropData> DragEnterAction
        {
            get { return (Action<DragDropData>)GetValue(DragEnterActionProperty); }
            set { SetValue(DragEnterActionProperty, value); }
        }

        public Action<DragDropData> DragOverAction
        {
            get { return (Action<DragDropData>)GetValue(DragOverActionProperty); }
            set { SetValue(DragOverActionProperty, value); }
        }

        public Action<DragDropData> DragLeaveAction
        {
            get { return (Action<DragDropData>)GetValue(DragLeaveActionProperty); }
            set { SetValue(DragLeaveActionProperty, value); }
        }

        public async Task ProcessComandsAsync(DataPackageView dataview)
        {
            if (dataview.Contains(StandardDataFormats.StorageItems) && DropStorageItemsAction != null)
            {
                IReadOnlyList<IStorageItem> storageItems = await dataview.GetStorageItemsAsync();
                DropStorageItemsAction.Invoke(storageItems);
            }
        }
    }
}
