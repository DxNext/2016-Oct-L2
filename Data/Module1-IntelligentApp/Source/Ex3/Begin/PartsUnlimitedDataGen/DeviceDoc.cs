using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsUnlimitedDataGen
{
    public class DeviceDoc
    {
        public string deviceId { get; set; }
        public string generationId { get; set; }
        public string etag { get; set; }
        public string connectionState { get; set; }
        public string status { get; set; }
        public object statusReason { get; set; }
        public string connectionStateUpdatedTime { get; set; }
        public string statusUpdatedTime { get; set; }
        public string lastActivityTime { get; set; }
        public int cloudToDeviceMessageCount { get; set; }
        public Authentication authentication { get; set; }
        public string id { get; set; }
    }
}
