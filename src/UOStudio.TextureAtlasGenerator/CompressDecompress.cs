using System;
using System.IO;
using SevenZip;

namespace UOStudio.TextureAtlasGenerator
{
    internal class CompressDecompress
    {
        public static byte[] CompressFileLZMA(byte[] bytesIn)
        {
            var dictionary = 1 << 23;
            var posStateBits = 2;
            var litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            var litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            var algorithm = 2;
            var numFastBytes = 128;

            var mf = "bt4";
            var eos = true;
            var stdInMode = false;


            CoderPropID[] propIDs =  {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

            object[] properties = {
                dictionary,
                posStateBits,
                litContextBits,
                litPosBits,
                algorithm,
                numFastBytes,
                mf,
                eos
            };

            using var inStream = new MemoryStream(bytesIn);
            using var outStream = new MemoryStream();
            var encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            var fileSize = eos || stdInMode ? -1 : inStream.Length;
            for (var i = 0; i < 8; i++)
            {
                outStream.WriteByte((byte)(fileSize >> (8 * i)));
            }
            encoder.Code(inStream, outStream, -1, -1, null);

            return outStream.ToArray();
        }

        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            using var input = new FileStream(inFile, FileMode.Open);
            using var output = new FileStream(outFile, FileMode.Create);
            var decoder = new SevenZip.Compression.LZMA.Decoder();

            var properties = new byte[5];
            if (input.Read(properties, 0, 5) != 5)
            {
                throw new Exception("input .lzma is too short");
            }
            decoder.SetDecoderProperties(properties);

            long outSize = 0;
            for (var i = 0; i < 8; i++)
            {
                var v = input.ReadByte();
                if (v < 0)
                {
                    throw new Exception("Can't Read 1");
                }
                outSize |= (long)(byte)v << (8 * i);
            }
            var compressedSize = input.Length - input.Position;

            decoder.Code(input, output, compressedSize, outSize, null);
        }
    }
}