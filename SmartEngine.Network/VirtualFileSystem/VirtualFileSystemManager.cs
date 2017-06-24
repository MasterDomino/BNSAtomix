using SmartEngine.Network.VirtualFileSystem.IFileSystemImp;

namespace SmartEngine.Network.VirtualFileSystem
{
    /// <summary>
    /// 文件系统类型
    /// </summary>
    public enum FileSystems
    {
        /// <summary>
        /// 真实文件系统
        /// </summary>
        Real,

        /// <summary>
        /// LPK压缩归档文件
        /// </summary>
        LPK,

        /// <summary>
        /// 使用引擎的虚拟文件系统
        /// </summary>
        Engine,
    }

    /// <summary>
    /// 虚拟文件系统管理器
    /// </summary>
    public class VirtualFileSystemManager : Singleton<VirtualFileSystemManager>
    {
        #region Properties

        /// <summary>
        /// get and set filesystem interface
        /// </summary>
        public IFileSystem FileSystem { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// initialize the filesystem of type on path
        /// </summary>
        /// <param name="type">type of filesystem</param>
        /// <param name="path">path to filesystem</param>
        /// <returns></returns>
        public bool Init(FileSystems type, string path)
        {
            FileSystem?.Close();

            switch (type)
            {
                case FileSystems.Real:
                    FileSystem = new RealFileSystem();
                    break;

                case FileSystems.LPK:
                    FileSystem = new LPKFileSystem();
                    break;

                case FileSystems.Engine:

                    //fs = new EngineFileSystem();
                    break;
            }
            return FileSystem.Init(path);
        }

        #endregion
    }
}