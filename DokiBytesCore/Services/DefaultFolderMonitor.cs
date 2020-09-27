using DokiBytesCommon.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace DokiBytesCore.Services
{
    public class DefaultFolderMonitor : IFolderMonitor
    {
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();
        private readonly Configuration _settings;
        public event EventHandler FileChanged;
        private DateTime lastRead = DateTime.MinValue;
        private Hashtable fileWriteTime = new Hashtable();

        public DefaultFolderMonitor(IConfiguration configuration)
        {
            _settings = new Configuration();
            configuration.Bind(_settings);
            _watcher = new FileSystemWatcher();
            _watcher.Path = _settings.LocalPath;
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;

            _watcher.Filter = "*.*";

            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }
        public void Watch()
        {
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string path = e.FullPath.ToString();
            string currentLastWriteTime = File.GetLastWriteTime(e.FullPath).ToString();

            if (!fileWriteTime.ContainsKey(path) ||
                 fileWriteTime[path].ToString() != currentLastWriteTime
            )
            {
                RaiseOnFileChangedEvent(e);
                fileWriteTime[path] = currentLastWriteTime;
            }
            //Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {

            // Specify what is done when a file is renamed.  
            //Console.WriteLine(" {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }
        protected virtual void RaiseOnFileChangedEvent(FileSystemEventArgs e)
        {
            FileChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public void StopWatching()
        {
            _watcher.EnableRaisingEvents = false;
        }
    }
}
