using System.Threading.Tasks;

namespace CognitiveServices.Explorer.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
