using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchAPI
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
        Task<bool> IsIndexExsitAsync(string index);

        Task<bool> InsertAndUpdateDocumentAsync<T>(T objectDocment) where T : class;
    }
}
