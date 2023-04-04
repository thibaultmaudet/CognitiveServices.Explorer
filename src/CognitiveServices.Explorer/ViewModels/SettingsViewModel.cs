using System.Reflection;
using System.Windows.Input;

using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Contracts.ViewModels;
using CognitiveServices.Explorer.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace CognitiveServices.Explorer.ViewModels;

public class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private readonly ILocalSettingsService localSettingsService;
    private ElementTheme? elementTheme;

    private string faceEndpoint;
    private string faceKey;
    private string versionDescription;

    public ElementTheme? ElementTheme
    {
        get => elementTheme;
        set => SetProperty(ref elementTheme, value);
    }

    public string FaceEndpoint
    {
        get => faceEndpoint;
        set =>SetProperty(ref faceEndpoint, value);
    }

    public string FaceKey
    {
        get => faceKey;

        set => SetProperty(ref faceKey, value);
    }

    public string VersionDescription
    {
        get => versionDescription;
        set => SetProperty(ref versionDescription, value);
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(ILocalSettingsService localSettingsService, IThemeSelectorService themeSelectorService)
    {
        this.localSettingsService = localSettingsService;
        elementTheme = themeSelectorService.Theme;
        versionDescription = GetVersionDescription();

        faceEndpoint = "";
        faceKey = "";

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    public async void OnNavigatedTo(object parameter)
    {
        FaceEndpoint = await localSettingsService.ReadSettingAsync<string>("faceEndpoint") ?? "";
        FaceKey = await localSettingsService.ReadSettingAsync<string>("faceKey") ?? "";
    }
    
    public void OnNavigatedFrom()
    {
        localSettingsService.SaveSettingAsync("faceEndpoint", FaceEndpoint);
        localSettingsService.SaveSettingAsync("faceKey", FaceKey);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
            version = Assembly.GetExecutingAssembly().GetName().Version!;

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
