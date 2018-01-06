using NAudio.Utils;
using NAudio.Wave;
using System;
using System.IO;

namespace Recognito.Utils
{
    public static class AudioConverter
    {
        const string ERRO_AUDIO_FILE = "The system could not decode your file type. " +
                                       "Try converting your file to some PCM 16bit 16000 Hz mono file format using dedicated " +
                                       "software. (Hint : http://sox.sourceforge.net/";

        static WaveFormat format = new WaveFormat(16000, 16, 1);

        public static double[] ConvertAudioToDoubleArray(Stream audioInput, float sampleRate)
        {
            //Implementation with NAudio WaveFileReader
            using (var reader = new WaveFileReader(audioInput))
            {
                var format = reader.WaveFormat;
                float diff = Math.Abs(format.SampleRate - sampleRate);
                if (diff > 5 * MathHelper.Ulp(0.0f))
                {
                    throw new ArgumentException($"The sample rate for this file is different than Recognito's defined sample rate : [{format.SampleRate}]");
                }

                if (format.BitsPerSample != 16)
                    throw new ArgumentException("audioInput", ERRO_AUDIO_FILE);


                byte[] bytesBuffer = new byte[reader.Length];
                int read = reader.Read(bytesBuffer, 0, (int)audioInput.Length);

                var floatSamples = new double[read / 2];
                for (int sampleIndex = 0; sampleIndex < read / 2; sampleIndex++)
                {
                    var shortSampleValue = BitConverter.ToInt16(bytesBuffer, sampleIndex * 2);

                    floatSamples[sampleIndex] = shortSampleValue / 32768.0;
                }

                return floatSamples;


                ////THis is other way, but is a bitr slower
                //var sprov = reader.ToSampleProvider();
                //var samples = new float[reader.Length];
                //sprov.Read(samples, 0, (int)reader.Length);
                //return Array.ConvertAll(samples, (f) => (double)f);
            }
        }

        public static Stream WriteAudioInputStream(double[] doubles, bool seekBegin = true)
        {
            var outStream = new MemoryStream();

            using (WaveFileWriter writer = new WaveFileWriter(new IgnoreDisposeStream(outStream), format))
            {
                //writer.Write(bytes, 0, bytes.Length);
                float[] floates = Array.ConvertAll(doubles, (d) => (float)d);
                writer.WriteSamples(floates, 0, floates.Length);
                writer.Flush();
            }


            if (seekBegin)
                outStream.SeekBegin();

            return outStream;
        }


        private static byte[] ShortsToBytes(double[] data)
        {
            //Note, for this only bigendian is acceps
            var bytes = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                short s = (short)(data[i] * 32768.0);

                var bb = BitConverter.GetBytes(s);

                bytes[i * 2] = bb[0];
                bytes[i * 2 + 1] = bb[1];
            }

            return bytes;
        }

        private static short ByteArrayToShort(byte[] bytes, int offset)
        {
            return BitConverter.ToInt16(bytes, offset);
        }

    }
}
