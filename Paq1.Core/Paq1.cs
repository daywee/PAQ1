namespace Paq1.Core
{
    public class Paq1
    {
        public long Compress(string sourcePath, string targetPath)
        {
            using (var sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (var targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
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

                return sourceFile.Length; // todo: encode length to header
            }
        }

        public void Decompress(string sourcePath, string targetPath, long originalSize)
        {

            using (var sourceFile = new BitFile(sourcePath, BitFileMode.Read))
            using (var targetFile = new BitFile(targetPath, BitFileMode.Write))
            {
                var predictor = new Predictor();
                var decoder = new ArithmeticDecoder(sourceFile);

                while (targetFile.Length < originalSize)
                {
                    double p0 = predictor.Predict();
                    var bit = decoder.Decode(p0);
                    targetFile.Write(bit);
                    predictor.Update((int)bit);
                }
            }
        }
    }
}
