using System;
using System.Threading.Tasks;
using System.Windows.Input;

using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace CognitiveServices.Explorer.ViewModels
{
    public class SettingsViewModel : ObservableRecipient
    {
        private ElementTheme elementTheme;

        private ICommand switchThemeCommand;

        private readonly IThemeSelectorService themeSelectorService;

        private string faceEndpoint;
        private string faceKey;
        private string versionDescription;

        public ElementTheme ElementTheme
        {
            get => elementTheme;

            set => SetProperty(ref elementTheme, value);
        }

        public string FaceEndpoint
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values["FaceEndpoint"] != null)
                    faceEndpoint = ApplicationData.Current.LocalSettings.Values["FaceEndpoint"].ToString();

                return faceEndpoint;
            }

            set
            {
                ApplicationData.Current.LocalSettings.Values["FaceEndpoint"] = value;
                SetProperty(ref faceEndpoint, value);
            }
        }
        
        public string FaceKey
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values["FaceKey"] != null)
                    faceKey = ApplicationData.Current.LocalSettings.Values["FaceKey"].ToString();

                return faceKey;
            }

            set
            {
                ApplicationData.Current.LocalSettings.SaveString(nameof(FaceKey), value);
                SetProperty(ref faceKey, value);
            }
        }

        public string VersionDescription
        {
            get => versionDescription;

            set => SetProperty(ref versionDescription, value);
        }

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (switchThemeCommand == null)
                {
                    switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            if (ElementTheme != param)
                            {
                                ElementTheme = param;
                                await themeSelectorService.SetThemeAsync(param);
                            }
                        });
                }

                return switchThemeCommand;
            }
        }

        public SettingsViewModel(IThemeSelectorService themeSelectorService)
        {
            this.themeSelectorService = themeSelectorService;
            elementTheme = themeSelectorService.Theme;
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
