using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.IntegrationServices
{
    public interface IBaseIntegrationService
    {
        string ExternalSystemDescriptor { get; }
        Task GetActualDataAsync();
        Task SaveChangesAsync();
    }
}
