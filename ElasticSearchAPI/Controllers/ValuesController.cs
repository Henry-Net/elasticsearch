using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using Nest;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

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
            //var result = await _provider.IsIndexExsitAsync("test");

            //var mod = await new DocOperating().GetDocByPathAsync(@"D:\log.txt");
            //var mods = new List<DocModel>();

            //var result1 = await _provider.InsertAndUpdateDocumentAsync<DocModel>(mod);
            //NOPIHandler.ReadWordText(@"D:\Data\test");
            //PdfSharp.PdfSharpException

            return false;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            //var a =NOPIHandler.ReadWordText(@"D:\Data\test\1.docx");
            const string filename = @"D:\Data\test\5.pdf";
            PdfDocument document = PdfReader.Open(filename);
            var list = new List<string>();
            foreach (var page in document.Pages)
            {
                var text = PdfSharpExtensions.ExtractText(page);
                //var plist = text.ToList();
                list.AddRange(text);

            }
            

            return string.Join(",", list.ToArray());
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
