using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignSample.DocuSign_Objects
{
    public class Signer
    {
        public string email { get; set; }
        public string name { get; set; }
        public int recipientId { get; set; }
        public string clientUserId { get; set; }
        public string accessCode { get; set; }
        public Tabs tabs { get; set; }
    }
}
