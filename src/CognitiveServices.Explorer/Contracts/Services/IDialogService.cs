using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Contracts.Services
{
    public interface IDialogService
    {
        void SetPrimaryButtonState(bool isEnabled);

        Task ShowAsync(ContentDialog dialog);
    }
}
