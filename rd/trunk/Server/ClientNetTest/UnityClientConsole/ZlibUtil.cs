using System.Collections;
using zlib;
using System.IO;
using System;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;

public class ZlibUtil {

    public static void CompressData(byte[] inData, out byte[] outData)
    {
        using (MemoryStream outMemoryStream = new MemoryStream())
        using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_SYNC_FLUSH))
        using (Stream inMemoryStream = new MemoryStream(inData))
        {
            CopyStream(inMemoryStream, outZStream);
            outZStream.finish();
            outData = outMemoryStream.ToArray();
        }
    }

    public static void DecompressData(byte[] inData, out byte[] outData)
    {
        using (MemoryStream outMemoryStream = new MemoryStream())
        using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
        using (Stream inMemoryStream = new MemoryStream(inData))
        {
            try {

                CopyStream(inMemoryStream, outZStream);
               // outZStream.finish();
                outData = outMemoryStream.ToArray();
            }
            catch (ZStreamException e)
            {
                outData = null;
            }
        }
    }

    public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
    {
        byte[] buffer = new byte[2000];
        int len;
        while ((len = input.Read(buffer, 0, 2000)) > 0)
        {
            output.Write(buffer, 0, len);
        }
        output.Flush();
    }

    public static byte[] Compress(byte[] input)
    {
        // Create the compressor with highest level of compression  
        Deflater compressor = new Deflater();
        compressor.SetLevel(Deflater.BEST_COMPRESSION);

        // Give the compressor the data to compress  
        compressor.SetInput(input);
        compressor.Finish();

        /* 
         * Create an expandable byte array to hold the compressed data. 
         * You cannot use an array that's the same size as the orginal because 
         * there is no guarantee that the compressed data will be smaller than 
         * the uncompressed data. 
         */
        MemoryStream bos = new MemoryStream(input.Length);

        // Compress the data  
        byte[] buf = new byte[1024];
        while (!compressor.IsFinished)
        {
            int count = compressor.Deflate(buf);
            bos.Write(buf, 0, count);
        }

        // Get the compressed data  
        return bos.ToArray();
    }

    public static byte[] Uncompress(byte[] input)
    {
        Inflater decompressor = new Inflater();
        decompressor.SetInput(input);

        // Create an expandable byte array to hold the decompressed data  
        MemoryStream bos = new MemoryStream(input.Length);

        // Decompress the data  
        byte[] buf = new byte[1024];
        while (!decompressor.IsFinished)
        {
            int count = decompressor.Inflate(buf);
            if (count != 0)
            {
                bos.Write(buf, 0, count);
            }
            else {
                break;
            }
        }

        // Get the decompressed data  
        return bos.ToArray();
    }

}
