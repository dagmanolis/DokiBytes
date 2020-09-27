using System;

namespace DokiBytesCore.Services
{
    public interface IFolderMonitor
    {
        public event EventHandler FileChanged;
        public void Watch();
        public void StopWatching();
        public void Dispose();

    }
}
