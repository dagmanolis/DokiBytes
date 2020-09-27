namespace DokiBytesCore.Services
{
    public interface IFtpClient
    {
        public void Connect();
        public bool UploadFile(string file, string remoteFullPath);
        public bool RenameFile(string remoteFullPath);
        public bool DeleteFile(string remoteFullPath);
        public void Disconnect();
        public void Dispose();

    }
}
