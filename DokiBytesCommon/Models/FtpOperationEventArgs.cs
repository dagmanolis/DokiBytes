using System;

namespace DokiBytesCommon.Models
{
    public class FtpOperationEventArgs : EventArgs
    {
        public string Operation { get; set; }
        public string RemoteFullPath { get; set; }
    }
}
