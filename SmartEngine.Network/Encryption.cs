namespace SmartEngine.Network
{
    /// <summary>
    /// 使用类似RSA的算法交换密钥的加密抽象类，需被继承后才能使用
    /// </summary>
    public abstract class Encryption
    {
        private static Encryption impl = new AESEncryption();
        private static EncryptionKeyExchange exchange = new DefaultEncryptionKeyExchange();
        private EncryptionKeyExchange keyExchange;
        public EncryptionKeyExchange KeyExchange
        {
            get
            {
                if (keyExchange == null)
                {
                    keyExchange = exchange.CreateNewInstance();
                }

                return keyExchange;
            }
        }

        /// <summary>
        /// 加密所使用的具体实现，默认为AES加密
        /// </summary>
        public static Encryption Implementation { get { return impl; } set { impl = value; } }

        public static EncryptionKeyExchange KeyExchangeImplementation { get { return exchange; } set { exchange = value; } }

        /// <summary>
        /// 创建新实例
        /// </summary>
        /// <returns></returns>
        public abstract Encryption Create();

        /// <summary>
        /// 加密缓存区
        /// </summary>
        /// <param name="src">缓存区</param>
        /// <param name="offset">开始加密的偏移</param>
        public abstract void Encrypt(byte[] src, int offset, int len);

        /// <summary>
        /// 解密缓存区
        /// </summary>
        /// <param name="src">缓存区</param>
        /// <param name="offset">开始解密的偏移</param>
        public abstract void Decrypt(byte[] src, int offset, int len);
    }
}
