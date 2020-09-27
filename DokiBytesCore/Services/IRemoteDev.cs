using DokiBytesCommon.Models;
using System;

namespace DokiBytesCore.Services
{
    public interface IRemoteDev
    {
        public event EventHandler<FtpOperationEventArgs> FtpOperationFinished;

        public void Run();
        public void Stop();
        public void Dispose();
    }
}
