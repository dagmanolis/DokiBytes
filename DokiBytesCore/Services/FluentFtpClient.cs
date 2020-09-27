using DokiBytesCommon.Models;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Threading;

namespace DokiBytesCore.Services
{
    public class FluentFtpClient : IFtpClient

    {
        private readonly Configuration _settings;
        private readonly FtpClient _ftpClient;

        public FluentFtpClient(IConfiguration configuration)
        {
            _settings = new Configuration();
            configuration.Bind(_settings);
            _ftpClient = new FtpClient();
            _ftpClient.Host = _settings.Host;
            _ftpClient.Port = _settings.Port;
            _ftpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
            _ftpClient.DataConnectionType = FtpDataConnectionType.AutoActive;

        }
        public void Connect()
        {
            _ftpClient.Connect();
        }

        public bool DeleteFile(string remoteFullPath)
        {
            var result = _ftpClient.DeleteFileAsync(remoteFullPath);
            return result.IsCompletedSuccessfully;
        }

        public void Disconnect()
        {
            _ftpClient.DisconnectAsync();
        }

        public void Dispose()
        {
            _ftpClient.Dispose();
        }

        public bool RenameFile(string file)
        {
            throw new System.NotImplementedException();
        }

        public bool UploadFile(string file, string remoteFullPath)
        {
            bool isUploaded = false;
            int maxRetries = 3;
            int retryIndex = 0;
            while (retryIndex < maxRetries)
            {
                try
                {
                    using (var fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {

                        var result = _ftpClient.Upload(fstream, remoteFullPath, createRemoteDir: true);
                        isUploaded = result.IsSuccess();
                    }
                }
                catch
                {
                }
                finally
                {
                    Thread.Sleep(50);
                    retryIndex++;
                }
            }
            return isUploaded;

        }
    }
}
