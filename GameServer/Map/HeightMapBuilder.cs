using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using PhotoshopFile;

using SmartEngine.Core.Math;
using SmartEngine.Network.Map.PathFinding;
using SmartEngine.Core;
namespace SagaBNS.GameServer.Map
{
    public class HeightMapBuilder : IGeoData
    {
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<short, short>> values = new ConcurrentDictionary<int, ConcurrentDictionary<short, short>>();
        private readonly ConcurrentDictionary<int, ulong> areaUsage = new ConcurrentDictionary<int, ulong>();
        private const int DIAMETER = 10;
        public short MinX { get; set; }
        public short MaxX { get; set; }
        public short MinY { get; set; }
        public short MaxY { get; set; }
        public short MinZ { get; set; }
        public short MaxZ { get; set; }
        public string Name { get; set; }

        public HeightMapBuilder()
        {
            MinX = short.MaxValue;
            MinY = short.MaxValue;
            MinZ = short.MaxValue;
            MaxX = short.MinValue;
            MaxY = short.MinValue;
            MaxZ = short.MinValue;
        }

        public void Purge(short x, short y, int range)
        {
            for (int j = y - (range * DIAMETER); j < y + (range * DIAMETER); j += DIAMETER)
            {
                for (int i = x - (range * DIAMETER); i < x + (range * DIAMETER); i += DIAMETER)
                {
                    int hash = CalcHash((short)i, (short)j);
                    values.TryRemove(hash, out ConcurrentDictionary<short, short> removed);
                }
            }
        }

        public void Collect(short x, short y, short z)
        {
            int hash = CalcHash(x, y);
            if (!values.ContainsKey(hash))
            {
                values[hash] = new ConcurrentDictionary<short, short>();
            }

            if (x < MinX)
            {
                MinX = x;
            }

            if (x > MaxX)
            {
                MaxX = x;
            }

            if (y < MinY)
            {
                MinY = y;
            }

            if (y > MaxY)
            {
                MaxY = y;
            }

            if (z < MinZ)
            {
                MinZ = z;
            }

            if (z > MaxZ)
            {
                MaxZ = z;
            }

            bool already = false;
            foreach (KeyValuePair<short, short> i in values[hash])
            {
                if (Math.Abs(i.Key - z) < 50)
                {
                    already = true;
                    break;
                }
            }
            if (!already)
            {
                values[hash][z] = z;
            }
        }

        public ConcurrentDictionary<short,short> GetZ(short x, short y)
        {
            int hash = CalcHash(x, y);
            if (values.TryGetValue(hash, out ConcurrentDictionary<short, short> val))
            {
                return val;
            }
            else
            {
                return new ConcurrentDictionary<short, short>();
            }
        }

        public short GetZ(short x, short y, short referenceZ)
        {
            int hash = CalcHash(x, y);
            if (values.TryGetValue(hash, out ConcurrentDictionary<short, short> val))
            {
                foreach (KeyValuePair<short, short> i in val)
                {
                    if (Math.Abs(i.Key - referenceZ) < 50)
                    {
                        return i.Key;
                    }
                }
                return 0;
            }
            else
            {
                return 0;
            }
        }

        private int CalcHash(short x, short y)
        {
            int divide = DIAMETER * 2;
            return (((ushort)x / divide) << 16) | ((ushort)y / divide);
        }

        private int CalcHash(short x, short y, short z)
        {
            uint divide = DIAMETER * 2;
            ulong key = (((ulong)x / divide) << 32) | (((ulong)y / divide) << 16) | ((ulong)z / divide);
            key = (~key) + (key << 18); // key = (key << 18) - key - 1;
            key = key ^ (key >> 31);
            key = key * 21; // key = (key + (key << 2)) + (key << 4);
            key = key ^ (key >> 11);
            key = key + (key << 6);
            key = key ^ (key >> 22);
            return (int)key;
        }

        public void ToStream(System.IO.Stream stream)
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(stream);
            bw.Write(MinX);
            bw.Write(MaxX);
            bw.Write(MinY);
            bw.Write(MaxY);
            bw.Write(MinZ);
            bw.Write(MaxZ);
            bw.Write(values.Count);
            foreach (KeyValuePair<int, ConcurrentDictionary<short, short>> i in values)
            {
                bw.Write(i.Key);
                bw.Write((byte)i.Value.Count);
                foreach (KeyValuePair<short, short> j in i.Value)
                {
                    bw.Write(j.Key);
                }
            }
        }

        public static HeightMapBuilder FromStream(System.IO.Stream stream)
        {
            HeightMapBuilder builder = new HeightMapBuilder();
            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            builder.MinX = br.ReadInt16();
            builder.MaxX = br.ReadInt16();
            builder.MinY = br.ReadInt16();
            builder.MaxY = br.ReadInt16();
            builder.MinZ = br.ReadInt16();
            builder.MaxZ = br.ReadInt16();
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int key = br.ReadInt32();
                if (!builder.values.ContainsKey(key))
                {
                    builder.values[key] = new ConcurrentDictionary<short, short>();
                }

                byte c = br.ReadByte();
                for (int j = 0; j < c; j++)
                {
                    builder.values[key][br.ReadInt16()] = 0;
                }
            }
            return builder;
        }

        public void ToBMP(string path)
        {
            int width = MaxX / (DIAMETER * 2) - MinX / (DIAMETER * 2);
            int height = MaxY / (DIAMETER * 2) - MinY / (DIAMETER * 2);
            int depth = MaxZ - MinZ;

            PsdFile psdFile = new PsdFile()
            {

                //-----------------------------------------------------------------------

                Rows = height,
                Columns = width,

                // We only save in 8 bits per channel RGBA format, which corresponds to
                // Paint.NET's internal representation.
                Channels = 4,
                ColorMode = PsdColorMode.RGB,
                Depth = 8
            };

            //-----------------------------------------------------------------------
            // No color mode data is necessary for RGB
            //-----------------------------------------------------------------------

            ResolutionInfo resInfo = new ResolutionInfo()
            {
                HeightDisplayUnit = ResolutionInfo.Unit.In,
                WidthDisplayUnit = ResolutionInfo.Unit.In,

                HResDisplayUnit = ResolutionInfo.ResUnit.PxPerInch,
                VResDisplayUnit = ResolutionInfo.ResUnit.PxPerInch,

                HDpi = new UFixed16_16(72),
                VDpi = new UFixed16_16(72)
            };
            psdFile.Resolution = resInfo;
            psdFile.ImageCompression = ImageCompression.Rle;

            //-----------------------------------------------------------------------
            // Set document image data from the fully-rendered image
            //-----------------------------------------------------------------------

            int imageSize = psdFile.Rows * psdFile.Columns;

            psdFile.Layers.Clear();
            for (short i = 0; i < psdFile.Channels; i++)
            {
                var channel = new Layer.Channel(i, psdFile.BaseLayer)
                {
                    ImageData = new byte[imageSize],
                    ImageCompression = psdFile.ImageCompression
                };
            }
            AddNewLayer(psdFile, 0);
            var alpha = psdFile.Layers[0].AlphaChannel.ImageData;
            for (int i = 0; i < psdFile.Rows * psdFile.Columns; i++)
            {
                alpha[i] = 255;
            }
            AddNewLayer(psdFile, 1);
            for (int y = MinY; y <= MaxY - (DIAMETER * 2); y += (DIAMETER * 2))
            {
                for (int x = MinX; x <= MaxX - (DIAMETER * 2); x += (DIAMETER * 2))
                {
                    int hash = CalcHash((short)x, (short)y);
                    try
                    {
                        if (values.ContainsKey(hash))
                        {
                            var points = (from p in values[hash].Keys
                                          orderby p descending
                                          select p).ToArray();
                            for (int index = 0; index < points.Length; index++)
                            {
                                int c = (int)((((float)points[index] - MinZ) / depth) * 250 + 5);
                                int x2 = (x - MinX) / (DIAMETER * 2);
                                int y2 = (y - MinY) / (DIAMETER * 2);

                                if (index >= psdFile.Layers.Count - 1)
                                {
                                    AddNewLayer(psdFile, index + 1);
                                }

                                int pos = x2 + width * y2;
                                var rgb = psdFile.Layers[index + 1].ChannelsArray;
                                var alphaCh = psdFile.Layers[index + 1].AlphaChannel;
                                rgb[0].ImageData[pos] = (byte)c;
                                rgb[1].ImageData[pos] = (byte)c;
                                rgb[2].ImageData[pos] = (byte)c;
                                alphaCh.ImageData[pos] = 255;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            Layer l = psdFile.Layers[0];
            psdFile.Layers.RemoveAt(0);
            psdFile.Layers.Reverse();
            psdFile.Layers.Insert(0, l);
            psdFile.Save(path);
            psdFile = null;
        }

        private Layer AddNewLayer(PsdFile psdFile, int index)
        {
            PhotoshopFile.Layer psdLayer = new PhotoshopFile.Layer(psdFile)
            {
                BlendModeKey = "norm",
                Visible = true,

                // Set layer metadata
                Rect = new System.Drawing.Rectangle(0, 0, psdFile.Columns, psdFile.Rows),
                Name = "Layer " + index,
                Opacity = 255
            };
            psdLayer.Visible = true;
            psdLayer.MaskData = new PhotoshopFile.Layer.Mask(psdLayer);
            psdLayer.BlendingRangesData = new PhotoshopFile.Layer.BlendingRanges(psdLayer);

            // Preserve Unicode layer name as Additional Layer Information
            var luniLayerInfo = new PhotoshopFile.Layer.AdjustmentLayerInfo("luni");
            var luniData = Encoding.BigEndianUnicode.GetBytes("\u0000\u0000" + psdLayer.Name);
            Util.SetBigEndianInt32(luniData, 0, psdLayer.Name.Length);
            luniLayerInfo.Data = luniData;
            psdLayer.AdjustmentInfo.Add(luniLayerInfo);

            // Store channel metadata
            int layerSize = psdLayer.Rect.Width * psdLayer.Rect.Height;
            for (int i = -1; i < 3; i++)
            {
                PhotoshopFile.Layer.Channel ch = new PhotoshopFile.Layer.Channel((short)i, psdLayer)
                {
                    ImageCompression = ImageCompression.Rle,
                    ImageData = new byte[layerSize]
                };
            }
            return psdLayer;
        }

        private void FilterNearHeight(ConcurrentDictionary<short, short> val, int hash, Dictionary<int, List<short>> removeList)
        {
            var ordered = from p in val
                          orderby p.Key descending
                          select p.Key;
            short last = short.MaxValue;
            foreach (short i in ordered)
            {
                if (last - i > 100)
                {
                    last = i;
                }
                else
                {
                    if (!removeList.ContainsKey(hash))
                    {
                        removeList.Add(hash, new List<short>());
                    }

                    removeList[hash].Add(i);
                }
            }
        }

        public void Filter()
        {
            float avg = short.MinValue;
            Dictionary<int, List<short>> removeList = new Dictionary<int,List<short>>();
            float counter = 0;
            float count = 0;
            short minZ = short.MaxValue;
            short maxZ = short.MinValue;
            foreach (int i in values.Keys)
            {
                List<short> list2 = values[i].Keys.ToList();
                if (avg > short.MinValue)
                {
                    foreach (short val in list2)
                    {
                        if ((Math.Abs(val - avg)) > 2000f)
                        {
                            if(!removeList.ContainsKey(i))
                            {
                                removeList.Add(i,new List<short>());
                            }

                            removeList[i].Add(val );
                            continue;
                        }
                        else
                        {
                            counter += val;
                            count += 1;
                            avg = counter / count;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < list2.Count; j++)
                    {
                        counter += list2[j];
                    }
                    avg = counter / list2.Count;
                    count = list2.Count;
                }
                ConcurrentDictionary<short,short> list = values[i];
                short x = (short)((((uint)i >> 16) & 0xFFFF) * DIAMETER *2);
                short y = (short)((((uint)i) & 0xFFFF)* DIAMETER *2);
                //FilterNearHeight(list, i, removeList);
                if (list.Count > 400)
                {
                    /*foreach (KeyValuePair<short, short> z in list)
                    {
                        if (!removeList.ContainsKey(i))
                            removeList.Add(i, new List<short>());
                        removeList[i].Add(z.Key);
                    }*/
                }
                else
                {
                    foreach (KeyValuePair<short, short> z in list)
                    {
                        int connetected = 0;
                        HashSet<int> already = new HashSet<int>();
                        for (int j = x - DIAMETER * 2; j <= x + DIAMETER * 2; j += DIAMETER * 2)
                        {
                            for (int k = y - DIAMETER * 2; k <= y + DIAMETER * 2; k += DIAMETER * 2)
                            {
                                if (j == x && k == y)
                                {
                                    continue;
                                }

                                int hash2=CalcHash((short)j, (short)k);
                                if (already.Contains(hash2))
                                {
                                    continue;
                                }

                                already.Add(hash2);

                                foreach (short l in GetZ((short)j, (short)k).Keys)
                                {
                                    if (Math.Abs(l - z.Key) < 20)
                                    {
                                        connetected++;
                                        if (connetected > 3)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (connetected > 3)
                                {
                                    break;
                                }
                            }
                            if (connetected > 3)
                            {
                                break;
                            }
                        }

                        if (connetected <= 3)
                        {
                            if (!removeList.ContainsKey(i))
                            {
                                removeList.Add(i, new List<short>());
                            }

                            removeList[i].Add(z.Key);
                        }
                        if (z.Key < minZ)
                        {
                            minZ = z.Key;
                        }

                        if (z.Key > maxZ)
                        {
                            maxZ = z.Key;
                        }
                    }
                }
            }
            count = removeList.Count;
            foreach (int i in removeList.Keys)
            {
                ConcurrentDictionary<short, short> list = values[i];
                foreach (short val in removeList[i])
                {
                    //if (Math.Abs(val - avg) > 2000)
                    {
                        list.TryRemove(val, out short removed);
                    }
                }
            }

            MinZ = minZ;
            MaxZ = maxZ;
            Logger.Log.Info(string.Format("{0} Points filtered!", count));
        }

        public void ActorMove(ulong actorID, short fromX, short fromY, short fromZ, short toX, short toY, short toZ)
        {
            int hash = CalcHash(fromX, fromY, fromZ);
            areaUsage.TryRemove(hash, out ulong removed);
            hash = CalcHash(toX, toY, toZ);
            areaUsage[hash] = actorID;
        }

        public void ClearArea(short x, short y, short z)
        {
            int hash = CalcHash(x, y, z);
            areaUsage.TryRemove(hash, out ulong removed);
        }

        #region IGeoData 成员

        public bool IsWalkable(int fromX, int fromY, int fromZ, int toX, int toY,int toZ)
        {
            return IsWalkable(fromX, fromY, fromZ, toX, toY, toZ, false);
        }

        public bool IsWalkable(int fromX, int fromY, int fromZ, int toX, int toY, int toZ, bool ignoreActor)
        {
            RealCoordinatesFromNormalized(toX, toY, toZ, out int resX, out int resY, out int resZ2);
            int hash = CalcHash((short)resX, (short)resY);
            int hashActor = CalcHash((short)resX, (short)resY, (short)resZ2);
            if (!ignoreActor && areaUsage.TryGetValue(hash, out ulong actor))
            {
                if (actor != 0)
                {
                    return false;
                }
            }

            if (values.TryGetValue(hash, out ConcurrentDictionary<short, short> toZList))
            {
                RealCoordinatesFromNormalized(fromX, fromY, fromZ, out resX, out resY, out int resZ);
                hash = CalcHash((short)resX, (short)resY);
                if (values.TryGetValue(hash, out ConcurrentDictionary<short, short> fromZList))
                {
                    bool foundFrom = false;

                    foreach (KeyValuePair<short, short> i in fromZList)
                    {
                        if (Math.Abs(resZ - i.Key) < 30)
                        {
                            foundFrom = true;
                            break;
                        }
                    }

                    if (foundFrom)
                    {
                        foreach (KeyValuePair<short, short> i in toZList)
                        {
                            if (Math.Abs(resZ2 - i.Key) < 20 || (resZ2 > i.Key && Math.Abs(resZ2 - i.Key) < 50))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void NormalizeCoordinates(int x, int y, int z, out int res_x, out int res_y,out int res_z)
        {
            res_x = x / DIAMETER;
            res_y = y / DIAMETER;
            res_z = z / DIAMETER;
        }

        public void RealCoordinatesFromNormalized(int x, int y, int z, out int res_x, out int res_y,out int res_z)
        {
            res_x = x * DIAMETER;
            res_y = y * DIAMETER;
            res_z = z * DIAMETER;
        }

        /// <summary>
        /// 取得最大可能向指定方向推进的距离
        /// </summary>
        /// <param name="x">源X</param>
        /// <param name="y">源Y</param>
        /// <param name="dir">方向</param>
        /// <param name="distance">最大距离</param>
        /// <param name="res_x">最大可能目标X</param>
        /// <param name="res_y">最大可能目标Y</param>
        public bool GetMaximunPushBackPos(int x, int y, int z, ushort dir, int distance, out int res_x, out int res_y,out int res_z)
        {
            res_x = x;
            res_y = y;
            res_z = z;
            int tmpZ = z;
            Vec3 vec = dir.DirectionToVector();
            int finalDistance = 0;
            for (int i = 0; i <= distance; i += DIAMETER)
            {
                Vec3 delta = vec * i;
                NormalizeCoordinates(x + (int)delta.X, y + (int)delta.Y, tmpZ, out int fromX, out int fromY, out int fromZ);
                delta = vec * (i + DIAMETER);
                tmpZ = GetZ((short)(x + delta.X), (short)(y + delta.Y), (short)tmpZ);
                NormalizeCoordinates(x + (int)delta.X, y + (int)delta.Y, tmpZ, out int toX, out int toY, out int toZ);
                if (!IsWalkable(fromX, fromY, fromZ, toX, toY, toZ, true))
                {
                    break;
                }

                res_x = x + (int)delta.X;
                res_y = y + (int)delta.Y;
                res_z = GetZ((short)(x + delta.X), (short)(y + delta.Y), (short)tmpZ);
                finalDistance = i + DIAMETER;
            }
            if (finalDistance > distance)
            {
                Vec3 delta = vec * (finalDistance - DIAMETER);
                tmpZ = z;
                NormalizeCoordinates(x + (int)delta.X, y + (int)delta.Y, tmpZ, out int fromX, out int fromY, out int fromZ);
                delta = vec * distance;
                tmpZ = GetZ((short)(x + delta.X), (short)(y + delta.Y), (short)tmpZ);

                NormalizeCoordinates(x + (int)delta.X, y + (int)delta.Y, tmpZ, out int toX, out int toY, out int toZ);
                if (IsWalkable(fromX, fromY, fromZ, toX, toY, toZ, true))
                {
                    res_x = x + (int)delta.X;
                    res_y = y + (int)delta.Y;
                    res_z = GetZ((short)(x + delta.X), (short)(y + delta.Y), (short)tmpZ);
                }
            }
            if (finalDistance > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
