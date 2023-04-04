using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Contracts.Services;

public interface IDialogService
{
    void SetPrimaryButtonState(bool isEnabled);

    Task<ContentDialogResult> ShowAsync(ContentDialog dialog);
}
