using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class NotoficationCountResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

}
