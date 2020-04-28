using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchAPI.Dto
{
    public class DocModel
    {
        public int Id { get; set; }
        public string DocName { get; set; }

        public string DocPath { get; set; }

        public DateTime DocDate { get; set; }

        public string DocContent { get; set; }
    }
}
