using CognitiveServices.Explorer.Contracts.Services;
using CognitiveServices.Explorer.Core.Helpers;
using CognitiveServices.Explorer.Core.Models;
using CognitiveServices.Explorer.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CognitiveServices.Explorer.ViewModels.Dialogs
{
    public class AddGroupViewModel : ObservableRecipient
    {
        private bool enablePrimaryButton;

        private FaceProcessorService faceProcessor;

        private string groupDescription;
        private string groupName;
        private string recognictionModelSelected;

        public bool EnablePrimaryButton
        {
            get { return enablePrimaryButton; }
            set { SetProperty(ref enablePrimaryButton, value); }
        }

        public ICommand ValidateCommand
        {
            get { return new RelayCommand(Validate); }
        }

        public Dictionary<string, string> RecognitionModels
        {
            get { return new() { { "recognition_01", "Version 1" }, { "recognition_02", "Version 2" }, { "recognition_03", "Version 3" }, { "recognition_04", "Version 4" } }; }
        }

        public string GroupDescription
        {
            get { return groupDescription; }
            set { SetProperty(ref groupDescription, value); }
        }

        public string GroupName
        {
            get { return groupName; }

            set
            {
                SetProperty(ref groupName, value);

                EnablePrimaryButton = !string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected);
            }
        }

        public string RecognictionModelSelected
        {
            get { return recognictionModelSelected; }

            set
            {
                SetProperty(ref recognictionModelSelected, value);

                EnablePrimaryButton = !string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(recognictionModelSelected);
            }
        }

        public AddGroupViewModel(IFaceClientService faceClientService)
        {
            faceProcessor = new FaceProcessorService(faceClientService.FaceClient);
        }

        private async void Validate()
        {
            if (!string.IsNullOrEmpty(GroupDescription))
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), groupName, recognictionModelSelected, await Json.StringifyAsync(new UserData() { Description = GroupDescription }));
            else
                await faceProcessor.CreatePersonGroupAsync(Guid.NewGuid().ToString().Trim('{').Trim('}'), groupName, recognictionModelSelected);
        }
    }
}
