using Windows.Storage;

namespace CognitiveServices.Explorer.Contracts.Services;

public interface IPickDialogService
{
    Task<StorageFile> PickSingleFolderDialog();
}
