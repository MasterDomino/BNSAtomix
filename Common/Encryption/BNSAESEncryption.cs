using System;
using System.Security.Cryptography;

namespace SagaBNS.Common.Encryption
{
    //this only works with modified client, which has disabled the encryption of the first gs packet
    public class BNSAESEncryption : SmartEngine.Network.Encryption
    {
        private readonly Rijndael aes;
        public byte[] Key;
        private ICryptoTransform enc;
        private ICryptoTransform dec;

        public BNSAESEncryption()
        {
            aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            enc = aes.CreateEncryptor(Key, new byte[16]);
            dec = aes.CreateDecryptor(Key, new byte[16]);
        }

        public override SmartEngine.Network.Encryption Create()
        {
            return new BNSAESEncryption();
        }

        public override void Encrypt(byte[] src, int offset, int len)
        {
            enc = aes.CreateEncryptor(Key, new byte[16]);
            enc.TransformBlock(src, offset, len, src, offset);
        }

        public override void Decrypt(byte[] src, int offset, int len)
        {
            byte[] buf = new byte[len + 16];//more 16 bytes to ensure it decrypts completely
            dec = aes.CreateDecryptor(Key, new byte[16]);
            src.CopyTo(buf, 0);
            dec.TransformBlock(buf, offset, len + 16, buf, offset);
            Array.Copy(buf, offset, src, offset, len);
        }
    }
}
