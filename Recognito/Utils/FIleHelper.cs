using NAudio.Wave;
using System;
using System.IO;

namespace Recognito.Utils
{
    public static class FileHelper
    {
        const string ERRO_AUDIO_FILE = "The system could not decode your file type. " +
                                       "Try converting your file to some PCM 16bit 16000 Hz mono file format using dedicated " +
                                       "software. (Hint : http://sox.sourceforge.net/";

        public static double[] ReadAudioInputStream(Stream audioInput)
        {
            //Implementation with NAudio WaveFileReader 

            using (var reader = new WaveFileReader(audioInput))
            {
                if (reader.WaveFormat.BitsPerSample != 16)
                    throw new ArgumentException("audioInput", ERRO_AUDIO_FILE);



                byte[] bytesBuffer = new byte[reader.Length];
                int read = reader.Read(bytesBuffer, 0, (int)audioInput.Length);


                var floatSamples = new double[read / 2];
                for (int sampleIndex = 0; sampleIndex < read / 2; sampleIndex++)
                {
                    var intSampleValue = BitConverter.ToInt16(bytesBuffer, sampleIndex * 2);
                    floatSamples[sampleIndex] = intSampleValue / 32768.0;
                }

                return floatSamples;
            }
        }
    }
}
