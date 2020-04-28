using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace ElasticSearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly ElasticClient _client;
        private readonly IEsClientProvider _provider;

        public ValuesController(IEsClientProvider clientProvider)
        {
            _client = clientProvider.GetClient();
            _provider = clientProvider;
        }


        // GET api/values
        [HttpGet]
        public async Task<bool> Get()
        {
            var result = await _provider.IsIndexExsitAsync("test");

            var mod = await new DocOperating().GetDocByPathAsync(@"D:\log.txt");
            var mods = new List<DocModel>();
         
            var result1 = await _provider.InsertAndUpdateDocumentAsync<DocModel>(mod);

            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
