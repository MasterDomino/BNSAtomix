using System.IO;
using SevenZip.Compression.LZMA;
using SevenZip;

namespace SmartEngine.Network.VirtualFileSystem.LPK.LZMA
{
    public static class LzmaHelper
    {
        private static readonly int dictionary = 1 << 23;
        private static readonly bool eos;

        private static readonly CoderPropID[] propIDs =
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};

        // these are the default properties, keeping it simple for now:
        private static readonly object[] properties =
				{
                    dictionary,
                    2,
                    3,
                    0,
                    2,
                    128,
					"bt4",
					eos
				};
        private static readonly byte[] props = new byte[5]
        {
            0x5D,
            0,
            0,
            0x80,
            0,
        };

        public static void Compress(Stream inStream, Stream outStream, ICodeProgress progress)
        {
            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            //encoder.WriteCoderProperties(outStream);
            encoder.Code(inStream, outStream, -1, -1, progress);
        }

        public static void Decompress(Stream inStream, Stream outStream, long size, long uncompressedSize, ICodeProgress progress)
        {
            Decoder decoder = new Decoder();

            decoder.SetDecoderProperties(props);

            decoder.Code(inStream, outStream, size, uncompressedSize, progress);
        }

        public static uint CRC32(Stream inStream)
        {
            CRC crc = new CRC();
            crc.Init();
            byte[] buf = new byte[1024];
            while (inStream.Position < inStream.Length)
            {
                int read;
                if (inStream.Position + 1024 < inStream.Length)
                {
                    read = 1024;
                }
                else
                {
                    read = (int)(inStream.Length - inStream.Position);
                }

                inStream.Read(buf, 0, read);
                crc.Update(buf, 0, (uint)read);
            }
            return crc.GetDigest();
        }
    }
}
