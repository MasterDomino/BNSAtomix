using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Unpacker
{
    internal class ArchiveHeader
    {
        public string Size { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsEncrypted { get; set; }
        public int FileCount { get; set; }
        public int TableSize { get; set; }
        public int TableCompressedSize { get; set; }
        public int DataOffset { get; set; }

        private readonly Dictionary<string, FileHeader> files = new Dictionary<string, FileHeader>();
        public Dictionary<string, FileHeader> Files { get { return files; } }
    }

    internal class FileHeader
    {
        public string Name { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsEncrypted { get; set; }
        public int CompressedSize { get; set; }
        public int Size { get; set; }
        public int EncryptedSize { get; set; }
        public int Offset { get; set; }
    }

    public unsafe class Archive
    {
        [DllImport("zlib.dll")]
        private static extern void compress(byte* dest, int* destLen, byte* src, int srcLen);
        [DllImport("zlib.dll")]
        private static extern void uncompress(byte* dest, int* destLen, byte* src, int srcLen);
        private readonly ArchiveHeader header = new ArchiveHeader();
        private string NameOfFile;

        private System.IO.FileStream fs;
        private System.IO.BinaryReader br;
        private readonly Rijndael aes = Rijndael.Create();
        private bool newUnpack;

        public void OpenFile(string path)
        {
            string[] array = path.Split('\\');
            NameOfFile = array[array.Count() - 1].ToLower();
            fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            br = new System.IO.BinaryReader(fs);
            fs.Position = 21;
            header.FileCount = br.ReadInt32();
            header.IsCompressed = br.ReadByte() == 1;
            header.IsEncrypted = br.ReadByte() == 1;
            fs.Position = 89;
            header.TableCompressedSize = br.ReadInt32();
            header.TableSize = br.ReadInt32();
            byte[] src = br.ReadBytes(header.TableCompressedSize);
            byte[] buf = new byte[header.TableCompressedSize + 16];
            src.CopyTo(buf, 0);
            byte[] buf2 = new byte[header.TableCompressedSize + 16];
            byte[] dst = new byte[header.TableSize];

            aes.Mode = CipherMode.ECB;

            ICryptoTransform decrypt = aes.CreateDecryptor(Encoding.ASCII.GetBytes("bns_fgt_cb_2010!"), new byte[16]);
            decrypt.TransformBlock(buf, 0, header.TableCompressedSize + 16, buf2, 0);
            int decompressedBytes = header.TableSize;
            fixed (byte* ptr2 = dst)
            {
                fixed (byte* ptr = buf2)
                {
                    uncompress(ptr2, &decompressedBytes, ptr, header.TableCompressedSize);
                }
            }

            int unknown = br.ReadInt32();
            int offset = (int)fs.Position;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(dst);
            System.IO.BinaryReader br2 = new System.IO.BinaryReader(ms);
            for (int i = 0; i < header.FileCount; i++)
            {
                FileHeader file = new FileHeader();
                int count = br2.ReadInt32();
                file.Name = Encoding.Unicode.GetString(br2.ReadBytes(count * 2));
                ms.Position++;
                file.IsCompressed = br2.ReadByte() == 1;
                file.IsEncrypted = br2.ReadByte() == 1;
                ms.Position++;
                file.Size = br2.ReadInt32();
                file.CompressedSize = br2.ReadInt32();
                file.EncryptedSize = br2.ReadInt32();
                file.Offset = offset + br2.ReadInt32();
                header.Files.Add(file.Name, file);
                ms.Position += 60;
            }
            ms.Close();
        }

        public void ExtractFile(string path, string destination)
        {
            if (header.Files.ContainsKey(path))
            {
                string folder = destination;
                folder = System.IO.Path.GetDirectoryName(folder + "\\" + path);
                string filename = System.IO.Path.GetFileName(path);
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                System.IO.FileStream fs2 = new System.IO.FileStream(folder + "\\" + filename, System.IO.FileMode.Create);
                FileHeader file = header.Files[path];
                fs.Position = file.Offset;
                byte[] src = br.ReadBytes(file.EncryptedSize);

                byte[] buf = new byte[file.EncryptedSize + 16];
                src.CopyTo(buf, 0);
                byte[] buf2 = new byte[file.EncryptedSize + 16];
                byte[] dst = new byte[file.Size];

                aes.Mode = CipherMode.ECB;

                ICryptoTransform decrypt = aes.CreateDecryptor(Encoding.ASCII.GetBytes("bns_fgt_cb_2010!"), new byte[16]);
                decrypt.TransformBlock(buf, 0, file.EncryptedSize + 16, buf2, 0);
                int decompressedBytes = file.Size;
                fixed (byte* ptr2 = dst)
                {
                    fixed (byte* ptr = buf2)
                    {
                        uncompress(ptr2, &decompressedBytes, ptr, file.CompressedSize);
                    }
                }
                string fileName = System.IO.Path.GetFileName(path);
                if (fileName == "system.config.xml" || fileName == "client.config2.xml" || fileName == "system.config2.xml" || System.IO.Path.GetExtension(path) == ".x16")
                {
                    fs2.Close();
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(dst))
                    {
                        System.Xml.XmlDocument xml = XML.XML.FromStream(ms);
                        xml.Save(folder + "\\" + filename);
                    }
                }
                else
                {
                    if (newUnpack && (System.IO.Path.GetExtension(path) == ".x16" || System.IO.Path.GetExtension(path) == ".xml"))
                    {
                        byte[] temp = new byte[8];
                        Array.Copy(dst, 0, temp, 0, 8);
                        if (Encoding.ASCII.GetString(temp).Equals("LMXBOSLB"))
                        {
                            temp = testUnpack(dst);
                            fs2.Write(temp, 0, temp.Length);
                        }
                        else
                        {
                            fs2.Write(dst, 0, dst.Length);
                        }
                    }
                    else
                    {
                        fs2.Write(dst, 0, dst.Length);
                    }

                    fs2.Flush();
                    fs2.Close();
                }
            }
        }

        private System.IO.BinaryWriter bwtest;
        private System.IO.BinaryReader brtest;
        private byte[] testUnpack(byte[] input)
        {
            System.IO.MemoryStream oms = new System.IO.MemoryStream();
            bwtest = new System.IO.BinaryWriter(oms);

            System.IO.MemoryStream ims = new System.IO.MemoryStream(input);
            brtest = new System.IO.BinaryReader(ims);

            bwtest.Write((ushort)0xFEFF);
            bwtest.Write(Encoding.Unicode.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));

            brtest.BaseStream.Position = 81;
            brtest.BaseStream.Position += (brtest.ReadInt32() * 2) + 4;
            uint count = brtest.ReadUInt32();
            string parameters = string.Empty;
            string parent;
            for (uint i = 0; i < count; i++)
            {
                parameters += " " + Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2)) + "=\"" + Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2)) + "\"";
            }

            brtest.BaseStream.Position += 1;
            parent = Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2));
            bwtest.Write(Encoding.Unicode.GetBytes("<" + parent + parameters + ">"));

            count = brtest.ReadUInt32();
            brtest.BaseStream.Position += 4;

            for (uint i = 0; i < count; i++)
            {
                getChild();
            }

            bwtest.Write(Encoding.Unicode.GetBytes("</" + parent + ">"));
            return oms.ToArray();
        }

        private void getChild()
        {
            string parameters;
            switch (brtest.ReadUInt32())
                {
                    case 2:
                        {
                            uint charCount = brtest.ReadUInt32() * 2;
                            bwtest.Write(brtest.ReadBytes((int)charCount));
                            brtest.BaseStream.Position += 1;
                            brtest.BaseStream.Position += (brtest.ReadUInt32() * 2) + 4;
                            uint check = brtest.ReadUInt32();
                            if (check != 0)
                        {
                            System.Windows.Forms.MessageBox.Show("Unexpected Child Node");
                        }

                        brtest.BaseStream.Position += 4;
                           break;
                        }
                    case 1:
                        {
                            parameters = string.Empty;
                            string child;
                            uint childCount = brtest.ReadUInt32();
                            for (uint j = 0; j < childCount; j++)
                            {
                                parameters += " " + Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2)) + "=\"" + Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2)) + "\"";
                            }

                            brtest.BaseStream.Position += 1;
                            child = Encoding.Unicode.GetString(brtest.ReadBytes(brtest.ReadInt32() * 2));
                            uint childCountStr = brtest.ReadUInt32();
                            brtest.BaseStream.Position += 4;
                            if (childCountStr == 0)
                        {
                            bwtest.Write(Encoding.Unicode.GetBytes("<" + child + parameters + "/>"));
                        }
                        else if (childCountStr > 0)
                            {
                                bwtest.Write(Encoding.Unicode.GetBytes("<" + child + parameters + ">"));

                                for (uint j = 0; j < childCountStr; j++)
                            {
                                getChild();
                            }

                            bwtest.Write(Encoding.Unicode.GetBytes("</" + child + ">"));
                            }
                            break;
                        }
                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("Unknown Type");
                            break;
                        }
                }
        }

        private byte[] PackFile(string path, out int uncompressedSize,out int compressedSize)
        {
            byte[] buffer;
            string fileName = System.IO.Path.GetFileName(path);
            if ((fileName == "system.config.xml" || fileName == "client.config2.xml" || fileName == "system.config2.xml" || System.IO.Path.GetExtension(path) == ".x16"))
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.Load(path);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    XML.XML.ToStream(ms, xml, path);
                    buffer = ms.ToArray();
                    uncompressedSize = buffer.Length;
                }
            }
            else
            {
                System.IO.FileStream fs2 = new System.IO.FileStream(path, System.IO.FileMode.Open);
                uncompressedSize = (int)fs2.Length;
                buffer = new byte[fs2.Length];
                fs2.Read(buffer, 0, (int)fs2.Length);
                fs2.Close();
            }

            byte[] dst = new byte[uncompressedSize];
            int size = uncompressedSize;
            fixed (byte* ptr2 = dst)
            {
                fixed (byte* ptr = buffer)
                {
                    compress(ptr2, &size, ptr, uncompressedSize);
                }
            }
            compressedSize = size;
            aes.Mode = CipherMode.ECB;

            ICryptoTransform decrypt = aes.CreateEncryptor(Encoding.ASCII.GetBytes("bns_fgt_cb_2010!"), new byte[16]);
            int encryptedSize = compressedSize;
            if (compressedSize % 16 != 0)
            {
                encryptedSize = compressedSize + (16 - (compressedSize % 16));
            }

            byte[] buffer2 = new byte[encryptedSize];
            decrypt.TransformBlock(dst, 0, encryptedSize, buffer2, 0);
            return buffer2;
        }

        private byte[] packAndEncrypt(byte[] buffer)
        {
            int uncompressedSize = buffer.Length;
            byte[] dst = new byte[uncompressedSize];
            int size = uncompressedSize;
            fixed (byte* ptr2 = dst)
            {
                fixed (byte* ptr = buffer)
                {
                    compress(ptr2, &size, ptr, uncompressedSize);
                }
            }
            int compressedSize = size;
            aes.Mode = CipherMode.ECB;

            ICryptoTransform decrypt = aes.CreateEncryptor(Encoding.ASCII.GetBytes("bns_fgt_cb_2010!"), new byte[16]);
            int encryptedSize = compressedSize;
            if (compressedSize % 16 != 0)
            {
                encryptedSize = compressedSize + (16 - (compressedSize % 16));
            }

            byte[] buffer2 = new byte[encryptedSize];
            decrypt.TransformBlock(dst, 0, encryptedSize, buffer2, 0);
            return buffer2;
        }

        public void ExtractAllFiles(string destination,bool newFormat)
        {
            newUnpack = newFormat;
            foreach (string i in header.Files.Keys)
            {
                ExtractFile(i, destination);
            }
        }

        public void RepackFolder(string path, bool newFormat)
        {
            newUnpack = newFormat;
            string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            string folder = System.IO.Path.GetDirectoryName(path);
            string file = folder + "\\" + System.IO.Path.GetFileName(path) + ".dat";
            NameOfFile = file.ToLower();
            string tmpFile = folder + "\\" + System.IO.Path.GetFileName(path) + ".tmp";

            System.IO.FileStream fsTmp = new System.IO.FileStream(tmpFile, System.IO.FileMode.Create);
            System.IO.FileStream fs2 = new System.IO.FileStream(file, System.IO.FileMode.Create);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs2);
            int offset = 0;
            ArchiveHeader header = new ArchiveHeader();
            foreach (string i in files)
            {
                byte[] buffer = PackFile(i, out int uncompressedSize, out int compressedSize);
                FileHeader f = new FileHeader()
                {
                    Name = i.Replace(path + "\\", ""),
                    Offset = offset,
                    Size = uncompressedSize,
                    EncryptedSize = buffer.Length,
                    CompressedSize = compressedSize
                };
                fsTmp.Write(buffer, 0, buffer.Length);
                header.Files.Add(f.Name, f);
                offset += buffer.Length;
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.BinaryWriter bw2 = new System.IO.BinaryWriter(ms);
            foreach (FileHeader i in header.Files.Values)
            {
                bw2.Write(i.Name.Length);
                bw2.Write(Encoding.Unicode.GetBytes(i.Name));
                ms.Position++;
                bw2.Write((byte)1);
                bw2.Write((byte)1);
                ms.Position++;
                bw2.Write(i.Size);
                bw2.Write(i.CompressedSize);
                bw2.Write(i.EncryptedSize);
                bw2.Write(i.Offset);
                ms.Position += 60;
            }
            fsTmp.Flush();
            byte[] table = ms.ToArray();
            header.TableSize = table.Length;
            table = packAndEncrypt(table);
            header.TableCompressedSize = table.Length;

            bw.Write(0x45534f55);
            bw.Write(0x424c4144);
            bw.Write((byte)2);

            fs2.Position = 21;
            bw.Write(header.Files.Count);
            bw.Write((byte)1);
            bw.Write((byte)1);
            fs2.Position = 89;
            bw.Write(header.TableCompressedSize);
            bw.Write(header.TableSize);
            bw.Write(table);
            bw.Write((int)(fs2.Position + 4));

            fsTmp.Position = 0;
            foreach (FileHeader i in header.Files.Values)
            {
                byte[] buf = new byte[i.EncryptedSize];
                fsTmp.Read(buf, 0, i.EncryptedSize);
                bw.Write(buf);
            }
            fsTmp.Close();
            bw.Flush();
            fs2.Flush();
            fs2.Close();
            System.IO.File.Delete(tmpFile);
        }

        public void Close()
        {
            fs.Close();
            br = null;
        }
    }
}
