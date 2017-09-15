using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace WCF.Lib.File.Helper
{
    public enum CompressAlgorithm
    {
        GZip,
        Deflate
    }
    public class ByteCompressor
    {

        private CompressAlgorithm algorithm;

        public ByteCompressor(CompressAlgorithm algorithm)
        {
            this.algorithm = algorithm;
        }

        //public static byte[] Zip(byte[] data)
        //{
        //    MemoryStream mstream = new MemoryStream();
        //    BZip2OutputStream zipOutStream = new BZip2OutputStream(mstream);
        //    zipOutStream.Write(data, 0, data.Length);
        //    zipOutStream.Close();
        //    zipOutStream.Dispose();
        //    byte[] result = mstream.ToArray();
        //    mstream.Close();
        //    return result;
        //}
        //public static byte[] Unzip(byte[] data)
        //{
        //    MemoryStream mstream = new MemoryStream(data);
        //    BZip2InputStream zipInputStream = new BZip2InputStream(mstream);
        //    byte[] byteUncompressed = new byte[zipInputStream.Length];
        //    zipInputStream.Read(byteUncompressed, 0, (int)byteUncompressed.Length);
        //    zipInputStream.Close();
        //    mstream.Close();
        //    return byteUncompressed;
        //}

        //压缩数组
        public byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                if (algorithm == CompressAlgorithm.GZip)
                {
                    Stream compressStream = new GZipStream(ms, CompressionMode.Compress, true);
                    compressStream.Write(data, 0, data.Length);
                    compressStream.Close();
                }
                else
                {
                    Stream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
                    compressStream.Write(data, 0, data.Length);
                    compressStream.Close();
                }
                byte[] newByteArray = new byte[ms.Length];

                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(newByteArray, 0, newByteArray.Length);

                //ArraySegment<byte> bytes = new ArraySegment<byte>(newByteArray);

                return newByteArray;
            }

        }

        //压缩流
        public Stream Compress(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            if (algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(ms, CompressionMode.Compress, true);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                compressStream.Write(buffer, 0, buffer.Length);
                compressStream.Close();
            }
            else
            {
                Stream compressStream = new DeflateStream(ms, CompressionMode.Compress, true);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                compressStream.Write(buffer, 0, buffer.Length);
                compressStream.Close();
            }
            return ms;
        }

        //解压缩数组
        public byte[] DeCompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                if (algorithm == CompressAlgorithm.GZip)
                {
                    Stream compressStream = new GZipStream(ms, CompressionMode.Decompress, false);
                    byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                    compressStream.Close();
                    return newByteArray;
                }
                else
                {
                    Stream compressStream = new DeflateStream(ms, CompressionMode.Decompress, false);
                    byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                    compressStream.Close();
                    return newByteArray;
                }

            }

        }

        //解压缩数组
        public Stream DeCompress(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (algorithm == CompressAlgorithm.GZip)
            {
                Stream compressStream = new GZipStream(stream, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new MemoryStream(newByteArray);
            }
            else
            {
                Stream compressStream = new DeflateStream(stream, CompressionMode.Decompress, false);
                byte[] newByteArray = RetrieveBytesFromStream(compressStream, 1);
                compressStream.Close();
                return new MemoryStream(newByteArray);
            }
        }
        public static byte[] RetrieveBytesFromStream(Stream stream, int bytesblock)
        {

            List<byte> lst = new List<byte>();
            byte[] data = new byte[1024];
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                byte[] buffers = new byte[bytesRead];
                Array.Copy(data, buffers, bytesRead);
                lst.AddRange(buffers);
                totalCount += bytesRead;
            }
            return lst.ToArray();
        }
    }
}
