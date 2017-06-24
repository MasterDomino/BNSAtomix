using System.IO;

namespace SmartEngine.Network.VirtualFileSystem.IFileSystemImp
{
    public class RealFileSystem : IFileSystem
    {
        private string rootPath = ".";
        #region IFileSystem Members

        public bool Init(string path)
        {
            if (path != string.Empty)
            {
                rootPath = path + "/";
            }
            else
            {
                rootPath = string.Empty;
            }

            return true;
        }

        public Stream OpenFile(string path)
        {
            if (path.IndexOf(":") < 0)
            {
                return new FileStream(rootPath + path, FileMode.Open, FileAccess.Read);
            }
            else
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string[] SearchFile(string path, string pattern)
        {
            return Directory.GetFiles(rootPath + path, pattern);
        }

        public string[] SearchFile(string path, string pattern, SearchOption option)
        {
            return Directory.GetFiles(rootPath + path, pattern, option);
        }

        public void Close()
        {
        }

        #endregion
    }
}
