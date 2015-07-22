using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSignSample.DocuSign_Objects
{
    public class Tabs
    {
        public List<SignHereTab> signHereTabs { get; set; }
        public List<InitialHereTab> initialHereTabs { get; set; }
        public List<FullNameTab> fullNameTabs { get; set; }
        public List<DateSignedTab> dateSignedTabs { get; set; }
    }
}
