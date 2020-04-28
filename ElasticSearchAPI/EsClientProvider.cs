using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchAPI.Dto;
using Microsoft.Extensions.Configuration;
using Nest;

namespace ElasticSearchAPI
{
    public class EsClientProvider : IEsClientProvider
    {
        private readonly IConfiguration _configuration;
        private ElasticClient _client;
        public EsClientProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ElasticClient GetClient()
        {
            if (_client != null)
                return _client;

            InitClient();
            return _client;
        }

        private void InitClient()
        {
            var node = new Uri(_configuration["EsUrl"]);
            var connectionSettings = new ConnectionSettings(node)
            .DefaultMappingFor<DocModel>(i => i
                .IndexName("my-projects")
                .IdProperty(p => p.Id)
            );
            _client = new ElasticClient(connectionSettings);
        }

        #region  index

        /// <summary>
        /// 检查索引是否存在
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<bool> IsIndexExsitAsync(string indexName)
        {
            var index = indexName.ToLower();
            bool flag = false;
            var result = await _client.Indices.ExistsAsync(index);
            if (result != null && result.Exists)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<bool> CreateIndexAsync(string indexName, int shards = 5)
        {
            bool flag = false;
            if (await IsIndexExsitAsync(indexName))
            {
                //_client.c
            }


            return flag;
        }
        #endregion

        #region
        public async Task<bool> InsertAndUpdateDocumentAsync<T>( T objectDocment) where T : class
        {
            bool flag = false;
            var result = await _client.IndexDocumentAsync<T>(objectDocment);
            if (result.Result ==Result.Created|| result.Result == Result.Updated)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<bool> InsertAndUpdateDocumentsAsync<T>(List<T> objectDocments) where T : class
        {
            bool flag = false;
            var result = await _client.IndexManyAsync<T>(objectDocments);
            if (!result.Errors)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<bool> DeleteDocumentAsync<T>(T objectDocment) where T : class
        {
            bool flag = false;
            var result = await _client.DeleteAsync<T>(objectDocment);
            if (result.Result == Result.Deleted)
            {
                flag = true;
            }
            return flag;
        }

        #endregion
    }
}
