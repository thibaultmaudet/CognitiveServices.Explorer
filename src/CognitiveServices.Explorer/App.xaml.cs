﻿using CognitiveServices.Explorer.Activation;
using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Helpers;
using CognitiveServices.Explorer.Services;
using CognitiveServices.Explorer.ViewModels;
using CognitiveServices.Explorer.ViewModels.Dialogs;
using CognitiveServices.Explorer.Views;
using CognitiveServices.Explorer.Views.Dialogs;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace CognitiveServices.Explorer
{
    public partial class App : Application
    {
        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }

        private System.IServiceProvider ConfigureServices()
        {
            // TODO WTS: Register your services, viewmodels and pages here
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IFaceClientService, FaceClientService>();

            // Views and ViewModels
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<FacePeopleViewModel>();
            services.AddTransient<FacePeoplePage>();
            services.AddTransient<AddFaceViewModel>();
            services.AddTransient<AddFaceDialog>();
            services.AddTransient<AddGroupViewModel>();
            services.AddTransient<AddGroupDialog>();
            services.AddTransient<AddEditPersonViewModel>();
            services.AddTransient<AddEditPersonDialog>();
            services.AddTransient<PictureAnalyseViewModel>();
            services.AddTransient<PictureAnalysePage>();
            return services.BuildServiceProvider();
        }
    }
}
