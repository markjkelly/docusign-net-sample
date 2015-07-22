using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignSample.DocuSign_Objects
{
    public class Envelope
    {
        public Recipients recipients { get; set; }
        public string emailSubject { get; set; }
        public List<Document> documents { get; set; }
        public string status { get; set; }
    }
}
