using ElasticSearchAPI.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchAPI
{
    public class DocOperating
    {
        public async Task<DocModel> GetDocByPathAsync(string Path)
        {
            DocModel docModel = null;
            var a = await File.ReadAllTextAsync(Path);
            docModel = new DocModel();
            docModel.Id = 4;
            docModel.DocName = "log.txt";
            docModel.DocPath = @"D:\log.txt";
            docModel.DocContent = a;
            //var b = await File.ReadAllTextAsync(@"D:\log.doc"); //buxing

            return docModel;
        }
    }
}
