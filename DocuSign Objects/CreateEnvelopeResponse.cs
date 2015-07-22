using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignSample.DocuSign_Objects
{
    public class CreateEnvelopeResponse
    {
        public string envelopeId { get; set; }
        public string uri { get; set; }
        public string statusDateTime { get; set; }
        public string status { get; set; }
    }
}
