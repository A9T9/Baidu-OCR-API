using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduAPITest
{
    public class OCRResult
    {
        public string errMsg { get; set; }
        public int errNum { get; set; }
        public List<RetData> retData { get; set; }
    }
}
