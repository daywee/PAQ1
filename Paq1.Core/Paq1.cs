using System;
using System.Collections.Generic;
using System.Text;

namespace Paq1.Core
{
    public class Paq1
    {
        public void Compress(string sourcePath, string targetPath)
        {
            using (var sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (var targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
                EncodeHeader(targetFile, sourceFile.LengthReal);

                var predictor = new Predictor();
                var encoder = new ArithmeticEncoder(targetFile);

                while (!sourceFile.IsEof)
                {
                    var bit = sourceFile.Read();
                    double p0 = predictor.Predict();
                    // todo: remove casting
                    encoder.Encode((int)bit, p0);
                    predictor.Update((int)bit);
                }

                encoder.EncodeRemainingBits();
            }
        }

        public void Decompress(string sourcePath, string targetPath)
        {

            using (var sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (var targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
                long originalSize = DecodeHeader(sourceFile);

                var predictor = new Predictor();
                var decoder = new ArithmeticDecoder(sourceFile);

                while (targetFile.LengthReal < originalSize)
                {
                    double p0 = predictor.Predict();
                    var bit = decoder.Decode(p0);
                    targetFile.Write(bit);
                    predictor.Update((int)bit);
                }
            }
        }

        private void EncodeHeader(BitFile targetFile, long size)
        {
            string header = $"PAQ1\r\n{size}\0";
            var headerBytes = Encoding.ASCII.GetBytes(header);

            foreach (var headerByte in headerBytes)
            {
                targetFile.Write(headerByte);
            }
        }

        private long DecodeHeader(BitFile sourceFile)
        {
            byte endOfHeader = Encoding.ASCII.GetBytes("\0")[0];
            var headerBytes = new List<byte>();

            byte headerByte;
            do
            {
                headerByte = sourceFile.ReadByte();
                headerBytes.Add(headerByte);
            } while (headerByte != endOfHeader);

            string header = Encoding.ASCII.GetString(headerBytes.ToArray());
            if (header.Substring(0, 6) != "PAQ1\r\n")
                throw new NotSupportedException("Bad PAQ1 header format.");

            var size = header.Substring(6, header.Length - 7);

            return long.Parse(size);
        }
    }
}
