using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Techugo.POS.ECom.Model
{
    public class ReasonResponse: BaseResponse
    {
        [JsonPropertyName("data")]
        public List<ReasonVM> Data { get; set; } = new List<ReasonVM>();
    }

    public class ReasonVM
    {
        public int ReasonID { get; set; }
        public string Reason { get; set; }
    }
}
