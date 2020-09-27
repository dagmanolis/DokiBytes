using DokiBytesCommon.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace DokiBytesCore.Services
{
    public class RemoteDevService : IRemoteDev
    {
        private readonly Configuration _settings;
        private readonly IFtpClient _ftpClient;
        private readonly IFolderMonitor _folderMonitor;
        public event EventHandler<FtpOperationEventArgs> FtpOperationFinished;
        private readonly List<string> _excludedDirectories;
        public RemoteDevService(IConfiguration configuration, IFtpClient ftpClient, IFolderMonitor folderMonitor)
        {
            _settings = new Configuration();
            _ftpClient = ftpClient;
            _folderMonitor = folderMonitor;
            configuration.Bind(_settings);
            _folderMonitor.FileChanged += OnFileChanged;
            _excludedDirectories = new List<string>();
            foreach (var ignore_dir in _settings.Ignore)
            {
                _excludedDirectories.Add(String.Format("{0}{1}", _settings.LocalPath, ignore_dir));
            }
        }

        public void Run()
        {
            _ftpClient.Connect();
            _folderMonitor.Watch();
        }
        private void OnFileChanged(object sender, EventArgs e)
        {
            var _e = (FileSystemEventArgs)e;

            //check if directory is excluded
            foreach (var ignored_dir in _excludedDirectories)
            {
                if (_e.FullPath.StartsWith(ignored_dir))
                    return;
            }

            string remotePath;
            string remoteFullPath;
            FtpOperationEventArgs ftpOperationArgs;
            switch (_e.ChangeType.ToString())
            {
                case "Deleted":
                    remotePath = _e.FullPath.Substring(_settings.LocalPath.Length);
                    remotePath = remotePath.Remove(remotePath.Length - _e.Name.Length, _e.Name.Length);
                    remotePath = remotePath.Replace("\\", "/");
                    remoteFullPath = _settings.RemotePath + remotePath + _e.Name;
                    _ftpClient.DeleteFile(remoteFullPath);
                    ftpOperationArgs = new FtpOperationEventArgs();
                    ftpOperationArgs.Operation = "deleted";
                    ftpOperationArgs.RemoteFullPath = remoteFullPath;
                    RaiseOnFtpOperationFinishedEvent(ftpOperationArgs);
                    break;
                case "Created":
                case "Changed":
                    remotePath = _e.FullPath.Substring(_settings.LocalPath.Length);
                    remotePath = remotePath.Remove(remotePath.Length - _e.Name.Length, _e.Name.Length);
                    remotePath = remotePath.Replace("\\", "/");
                    remoteFullPath = _settings.RemotePath + remotePath + _e.Name;
                    var success = _ftpClient.UploadFile(_e.FullPath, remoteFullPath);
                    if (success)
                    {
                        ftpOperationArgs = new FtpOperationEventArgs();
                        ftpOperationArgs.Operation = "uploaded";
                        ftpOperationArgs.RemoteFullPath = remoteFullPath;
                        RaiseOnFtpOperationFinishedEvent(ftpOperationArgs);
                    }
                    else
                    {
                        ftpOperationArgs = new FtpOperationEventArgs();
                        ftpOperationArgs.Operation = "failed to upload";
                        ftpOperationArgs.RemoteFullPath = remoteFullPath;
                        RaiseOnFtpOperationFinishedEvent(ftpOperationArgs);
                    }

                    break;
                default:
                    break;
            }
        }

        protected virtual void RaiseOnFtpOperationFinishedEvent(FtpOperationEventArgs e)
        {
            FtpOperationFinished?.Invoke(this, e);
        }

        public void Stop()
        {
            _ftpClient.Disconnect();
            _folderMonitor.StopWatching();
        }
        public void Dispose()
        {
            _ftpClient.Dispose();
            _folderMonitor.Dispose();
        }
    }
}
