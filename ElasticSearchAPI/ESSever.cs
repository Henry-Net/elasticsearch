using Elasticsearch.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchAPI
{
    public class ESSever
    {
        /// <summary>
        /// Linq查询的官方Client
        /// </summary>
        public IElasticClient ElasticLinqClient { get; set; }
        /// <summary>
        /// Js查询的官方Client
        /// </summary>
        public IElasticLowLevelClient ElasticJsonClient { get; set; }

        public IMemoryCache memoryCache { get; set; }


        public ESSever(IConfiguration configuration, IMemoryCache memoryCache_arg)
        {
            memoryCache = memoryCache_arg;
            var uris = configuration["ElasticSearchContext:Url"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(x => new Uri(x));
            var connectionPool = new StaticConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool).RequestTimeout(TimeSpan.FromSeconds(30));//.BasicAuthentication("elastic", "n@@W#RJQ$z1#")
            this.ElasticJsonClient = new ElasticLowLevelClient(settings);
            this.ElasticLinqClient = new ElasticClient(settings);
        }



        /// <summary>
        /// 封装后的linq的查询方式
        /// </summary>
        /// <typeparam name="T">要查询和返回的Json</typeparam>
        /// <param name="indexName">index的名称</param>
        /// <param name="typeName">type的名称</param>
        /// <param name="selector">linq内容</param>
        /// <returns></returns>
        public async Task<List<T>> SearchAsync<T>(string indexName, string typeName, Func<QueryContainerDescriptor<T>, QueryContainer> selector = null) where T : class
        {
            var list = await ElasticLinqClient.SearchAsync<T>(option => option.Index(indexName.ToLower()).Query(selector));
            return list.Documents.ToList();
        }


        /// <summary>
        /// 封装后的Json的查询方式
        /// </summary>
        /// <param name="indexName">index的名称</param>
        /// <param name="typeName">type的名称</param>
        /// <param name="jsonString">json字符串</param>
        /// <returns>返回Jobject的内容</returns>
        //public async Task<JToken> SearchAsync(string indexName, string typeName, string jsonString)
        //{
        //    var stringRespones = await ElasticJsonClient.SearchAsync<StringResponse>(indexName.ToLower(), typeName, jsonString);
        //    var jobject = JObject.Parse(stringRespones.Body);
        //    var total = Convert.ToInt32(jobject["hits"]["total"].ToString());
        //    if (total > 0)
        //    {
        //        string json = string.Empty;
        //        var sourceArg = jobject["hits"]["hits"];
        //        foreach (var source in sourceArg)
        //        {
        //            string sourceJson = source["_source"].ToString().Substring(1, source["_source"].ToString().Length - 1);
        //            sourceJson = "{ \"_id\":\"" + source["_id"] + "\"," + sourceJson;
        //            if (json.Length <= 0)
        //                json += sourceJson;
        //            else
        //                json += "," + sourceJson;


        //        }

        //        return JToken.Parse("[" + json + "]");
        //    }
        //    return null;
        //}


        /// <summary>
        /// 封装后的创建index
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="shards">分片数量，即数据块最小单元</param>
        /// <returns></returns>
        public async Task<bool> CreateIndexAsync(string indexName, int shards = 5)
        {
            var isHaveIndex = await IsIndexExsit(indexName.ToLower());
            if (!isHaveIndex)
            {
                //var stringResponse = await ElasticJsonClient.IndicesCreateAsync<StringResponse>(indexName.ToLower(),
                //        PostData.String($"{{\"settings\" : {{\"index\" : {{\"number_of_replicas\" : 0, \"number_of_shards\":\"{shards}\",\"refresh_interval\":\"-1\"}}}}}}"));
                //var resObj = JObject.Parse(stringResponse.Body);
                //if ((bool)resObj["acknowledged"])
                //{
                //    return true;
                //}
            }
            else
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 检测索引是否已经存在
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<bool> IsIndexExsit(string index)
        {
            bool flag = false;
            ExistsResponse resStr = null;
            try
            {
                resStr = await ElasticLinqClient.Indices.ExistsAsync(index);
                if (resStr.Exists)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
            }

            return flag;
        }

        /// <summary>
        /// 封装后的删除index
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteIndexAsync(string indexName)
        {
            //var stringRespones = await ElasticJsonClient.IndicesDeleteAsync<StringResponse>(indexName.ToLower());
            //var resObj = JObject.Parse(stringRespones.Body);
            //if ((bool)resObj["acknowledged"])
            //{
            //    return true;
            //}
            return false;
        }
        /// <summary>
        /// 插入单个文档
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName">文档名称</param>
        /// <param name="objectDocment">文档内容</param>
        /// <param name="_id">自定义_id</param>
        /// <returns></returns>
        public async Task<bool> InsertDocumentAsync(string indexName, string typeName, object objectDocment, string _id = "")
        {
            //var stringRespones = new StringResponse();
            //if (_id.Length > 0)
            //    stringRespones = await ElasticJsonClient.IndexAsync<StringResponse>(indexName.ToLower(), typeName, _id, PostData.String(JsonConvert.SerializeObject(objectDocment)));
            //else
            //    stringRespones = await ElasticJsonClient.IndexAsync<StringResponse>(indexName.ToLower(), typeName, PostData.String(JsonConvert.SerializeObject(objectDocment)));
            //var resObj = JObject.Parse(stringRespones.Body);
            //if ((int)resObj["_shards"]["successful"] > 0)
            //{
            //    return true;
            //}
            return false;
        }


        /// <summary>
        /// 优化写入性能
        /// </summary>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        /// <param name="replia"></param>
        /// <returns></returns>
        public async Task<bool> SetIndexRefreshAndReplia(string index, string refresh = "30s", int replia = 1)
        {
            bool flag = false;
            StringResponse resStr = null;
            //try
            //{
            //    if (memoryCache.TryGetValue("isRefreshAndReplia", out bool isrefresh))
            //    {
            //        if (!isrefresh)
            //        {
            //            resStr = await ElasticJsonClient.IndicesPutSettingsAsync<StringResponse>(index.ToLower(),
            //         PostData.String($"{{\"index\" : {{\"number_of_replicas\" : {replia},\"refresh_interval\":\"{refresh}\"}}}}"));
            //            var resObj = JObject.Parse(resStr.Body);
            //            if ((bool)resObj["acknowledged"])
            //            {
            //                flag = true;
            //                memoryCache.Set("isRefreshAndReplia", true);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        resStr = await ElasticJsonClient.IndicesPutSettingsAsync<StringResponse>(index.ToLower(),
            //        PostData.String($"{{\"index\" : {{\"number_of_replicas\" : {replia},\"refresh_interval\":\"{refresh}\"}}}}"));
            //        var resObj = JObject.Parse(resStr.Body);
            //        if ((bool)resObj["acknowledged"])
            //        {
            //            flag = true;
            //            memoryCache.Set("isRefreshAndReplia", true);
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //}
            return flag;
        }

        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName"></param>
        /// <param name="listDocment">数据集合</param>
        /// <returns></returns>
        public async Task<bool> InsertListDocumentAsync(string indexName, string typeName, List<object> listDocment)
        {
            //var isRefresh = await SetIndexRefreshAndReplia(indexName.ToLower());
            //if (isRefresh)
            //{
            //    List<string> list = new List<string>();
            //    foreach (var ob in listDocment)
            //    {
            //        //{"index":{"_index":"meterdata","_type":"autoData"}}
            //        var indexJsonStr = new { index = new { _index = indexName.ToLower(), _type = typeName } };
            //        list.Add(JsonConvert.SerializeObject(indexJsonStr));
            //        list.Add(JsonConvert.SerializeObject(ob));
            //    }

            //    var stringRespones = await ElasticJsonClient.BulkAsync<StringResponse>(indexName.ToLower(), typeName, PostData.MultiJson(list));
            //    var resObj = JObject.Parse(stringRespones.Body);
            //    if (!(bool)resObj["errors"])
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        /// <summary>
        /// 删除一个文档
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName">类别名称</param>
        /// <param name="_id">elasticsearch的id</param>
        /// <returns></returns>
        public async Task<bool> DeleteDocumentAsync(string indexName, string typeName, string _id)
        {
            bool flag = false;
            StringResponse resStr = null;
            //try
            //{
            //    resStr = await ElasticJsonClient.DeleteAsync<StringResponse>(indexName.ToLower(), typeName, _id);
            //    var resObj = JObject.Parse(resStr.Body);
            //    if ((int)resObj["_shards"]["total"] == 0 || (int)resObj["_shards"]["successful"] > 0)
            //    {
            //        flag = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            return flag;
        }

        ///// <summary>
        ///// 更新文档  删除重建法
        ///// </summary>
        ///// <param name="indexName">索引名称</param>
        ///// <param name="typeName">类别名称</param>
        ///// <param name="_id">elasticsearch的id</param>
        ///// <param name="objectDocment">单条数据的所有内容</param>
        ///// <returns></returns>
        //public async Task<bool> UpdateDocumentAsync(string indexName, string typeName, string _id, object objectDocment)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(objectDocment);
        //        if (json.IndexOf("[") == 0)
        //        {
        //            var objectDocmentOne = JToken.Parse(json);
        //            json = JsonConvert.SerializeObject(objectDocmentOne[0]);
        //            int idInt = json.IndexOf("\"_id");
        //            string idJson = json.Substring(idInt, json.IndexOf(_id) + _id.Length + 1);
        //            json = json.Replace(idJson, "");
        //        }
        //        var isOk = await DeleteDocumentAsync(indexName,typeName,_id);
        //        if (isOk)
        //        {
        //            flag = await InsertDocumentAsync(indexName,typeName, JToken.Parse(json), _id);
        //        }

        //    }
        //    catch {}
        //    return flag;
        //}

        /// <summary>
        /// 更新文档  
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName">类别名称</param>
        /// <param name="_id">elasticsearch的id</param>
        /// <param name="objectDocment">单条数据的所有内容</param>
        /// <returns></returns>
        //public async Task<bool> UpdateDocumentAsync(string indexName, string typeName, string _id, object objectDocment)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(objectDocment);
        //        if (json.IndexOf("[") == 0)
        //        {
        //            var objectDocmentOne = JToken.Parse(json);
        //            json = JsonConvert.SerializeObject(objectDocmentOne[0]);
        //        }
        //        int idInt = json.IndexOf("\"_id");
        //        if (idInt > 0)
        //        {
        //            string idJson = json.Substring(idInt, json.IndexOf(_id) + _id.Length + 1);
        //            json = json.Replace(idJson, "");
        //        }
        //        //{ "update" : { "_id" : "5cc2d9cf6d2d99ce58007201" } }
        //        //{ "doc" : { "Sex" : "王五111" } }
        //        List<string> list = new List<string>();
        //        list.Add("{\"update\":{\"_id\":\"" + _id + "\"}}");
        //        list.Add("{\"doc\":" + json + "}");
        //        var stringRespones = await ElasticJsonClient.BulkAsync<StringResponse>(indexName.ToLower(), typeName, PostData.MultiJson(list));
        //        var resObj = JObject.Parse(stringRespones.Body);
        //        if (!(bool)resObj["errors"])
        //        {
        //            return true;
        //        }
        //    }
        //    catch { }
        //    return flag;
        //}

        /// <summary>
        /// 批量更新文档
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName">类别名称</param>
        /// <param name="listDocment">数据集合，注：docment 里要有_id,否则更新不进去</param>
        /// <returns></returns>

        //public async Task<bool> UpdateListDocumentAsync(string indexName, string typeName, List<object> listDocment)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        List<string> list = new List<string>();
        //        foreach (var objectDocment in listDocment)
        //        {
        //            string json = JsonConvert.SerializeObject(objectDocment);
        //            JToken docment = null;
        //            var objectDocmentOne = JToken.Parse(json);
        //            docment = objectDocmentOne;
        //            if (json.IndexOf("[") == 0)
        //            {
        //                json = JsonConvert.SerializeObject(objectDocmentOne[0]);
        //                docment = objectDocmentOne[0];
        //            }
        //            string _id = docment["_id"].ToString();
        //            int idInt = json.IndexOf("\"_id");
        //            if (idInt > 0)
        //            {
        //                string idJson = json.Substring(idInt, json.IndexOf(_id) + _id.Length + 1);
        //                json = json.Replace(idJson, "");
        //            }
        //            //{ "update" : { "_id" : "5cc2d9cf6d2d99ce58007201" } }
        //            //{ "doc" : { "Sex" : "王五111" } }
        //            list.Add("{\"update\":{\"_id\":\"" + _id + "\"}}");
        //            list.Add("{\"doc\":" + json + "}");
        //        }
        //        var stringRespones = await ElasticJsonClient.BulkAsync<StringResponse>(indexName.ToLower(), typeName, PostData.MultiJson(list));
        //        var resObj = JObject.Parse(stringRespones.Body);
        //        if (!(bool)resObj["errors"])
        //        {
        //            return true;
        //        }
        //    }
        //    catch { }
        //    return flag;
        //}
    }
}
