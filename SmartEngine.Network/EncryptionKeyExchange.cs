using System.Numerics;
using SmartEngine.Network.Utils;

namespace SmartEngine.Network
{
    public abstract class EncryptionKeyExchange
    {
        public static BigInteger Module = new BigInteger(Conversions.HexStr2Bytes("f488fd584e49dbcd20b49de49107366b336c380d451d0f7c88b31c7c5b2d8ef6f3c923c043f0a55b188d8ebb558cb85d38d334fd7c175743a31d186cde33212cb52aff3ce1b1294018118d7c84a70a72d686c40319c807297aca950cd9969fabd00a509b0246d3083d66a45d419f9c7cbd894b221926baaba25ec355e92f78c7"));

        /// <summary>
        /// 交换得到的加解密密钥
        /// </summary>
        protected byte[] key;

        /// <summary>
        /// 交换得到的加解密密钥
        /// </summary>
        public byte[] Key { get { return key; } }

        public abstract EncryptionKeyExchange CreateNewInstance();

        /// <summary>
        /// 生成私钥
        /// </summary>
        public abstract void MakePrivateKey();

        /// <summary>
        /// 生成密钥交换用数据
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetKeyExchangeBytes(Mode mode);

        /// <summary>
        /// 根据公钥和私钥生成最终密钥
        /// </summary>
        /// <param name="keyExchangeBytes"></param>
        /// <param name="mode">当前生成的密钥是客户端还是服务器端的</param>
        public abstract void MakeKey(Mode mode, byte[] keyExchangeBytes);

        /// <summary>
        /// 加密算法是否就绪
        /// </summary>
        public abstract bool IsReady
        {
            get;
        }
    }
}
