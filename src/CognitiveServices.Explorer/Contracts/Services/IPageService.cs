using System;

namespace CognitiveServices.Explorer.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
