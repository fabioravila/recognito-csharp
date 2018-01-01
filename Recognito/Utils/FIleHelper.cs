using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.Utils
{
    public static class FileHelper
    {
        const string ERRO_AUDIO_FILE = "The system could not decode your file type. " +
                                       "Try converting your file to some PCM 16bit 16000 Hz mono file format using dedicated " +
                                       "software. (Hint : http://sox.sourceforge.net/";


        public static double[] ReadAudioInputStream(Stream audioInput)
        {
            //Implementation with NAudio StreamMediaFoundationReader 
            double[] floatBuffer;

            using (var media = new StreamMediaFoundationReader(audioInput, new MediaFoundationReader.MediaFoundationReaderSettings))
            {
                var _byteBuffer32_length = (int)media.Length * 2;
                var _floatBuffer_length = _byteBuffer32_length / sizeof(float);

                var stream32 = new Wave16ToFloatProvider(media);
                var _waveBuffer = new WaveBuffer(_byteBuffer32_length);
                stream32.Read(_waveBuffer, 0, (int)_byteBuffer32_length);

                floatBuffer = new double[_floatBuffer_length];

                for (int i = 0; i < _floatBuffer_length; i++)
                {
                    floatBuffer[i] = _waveBuffer.FloatBuffer[i];
                }
            }

            return floatBuffer;
        }

        public static double[] ReadAudioInputStream2(Stream audioInput)
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


        public static double[] ReadAudioInputStream3(Stream audioInput)
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








        //No naudio
        //        / convert two bytes to one double in the range -1 to 1
        //static double bytesToDouble(byte firstByte, byte secondByte)
        //        {
        //            // convert two bytes to one short (little endian)
        //            short s = (secondByte << 8) | firstByte;
        //            // convert to range from -1 to (just below) 1
        //            return s / 32768.0;
        //        }

        //        // Returns left and right double arrays. 'right' will be null if sound is mono.
        //        public void openWav(string filename, out double[] left, out double[] right)
        //        {
        //            byte[] wav = File.ReadAllBytes(filename);

        //            // Determine if mono or stereo
        //            int channels = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

        //            // Get past all the other sub chunks to get to the data subchunk:
        //            int pos = 12;   // First Subchunk ID from 12 to 16

        //            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
        //            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        //            {
        //                pos += 4;
        //                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
        //                pos += 4 + chunkSize;
        //            }
        //            pos += 8;

        //            // Pos is now positioned to start of actual sound data.
        //            int samples = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
        //            if (channels == 2) samples /= 2;        // 4 bytes per sample (16 bit stereo)

        //            // Allocate memory (right will be null if only mono sound)
        //            left = new double[samples];
        //            if (channels == 2) right = new double[samples];
        //            else right = null;

        //            // Write to double array/s:
        //            int i = 0;
        //            while (pos < length)
        //            {
        //                left[i] = bytesToDouble(wav[pos], wav[pos + 1]);
        //                pos += 2;
        //                if (channels == 2)
        //                {
        //                    right[i] = bytesToDouble(wav[pos], wav[pos + 1]);
        //                    pos += 2;
        //                }
        //                i++;
        //            }
        //        }



        //Other way

        //static bool readWav(string filename, out float[] L, out float[] R)
        //{
        //    L = R = null;
        //    //float [] left = new float[1];

        //    //float [] right;
        //    try
        //    {
        //        using (FileStream fs = File.Open(filename, FileMode.Open))
        //        {
        //            BinaryReader reader = new BinaryReader(fs);

        //            // chunk 0
        //            int chunkID = reader.ReadInt32();
        //            int fileSize = reader.ReadInt32();
        //            int riffType = reader.ReadInt32();


        //            // chunk 1
        //            int fmtID = reader.ReadInt32();
        //            int fmtSize = reader.ReadInt32(); // bytes for this chunk
        //            int fmtCode = reader.ReadInt16();
        //            int channels = reader.ReadInt16();
        //            int sampleRate = reader.ReadInt32();
        //            int byteRate = reader.ReadInt32();
        //            int fmtBlockAlign = reader.ReadInt16();
        //            int bitDepth = reader.ReadInt16();

        //            if (fmtSize == 18)
        //            {
        //                // Read any extra values
        //                int fmtExtraSize = reader.ReadInt16();
        //                reader.ReadBytes(fmtExtraSize);
        //            }

        //            // chunk 2
        //            int dataID = reader.ReadInt32();
        //            int bytes = reader.ReadInt32();

        //            // DATA!
        //            byte[] byteArray = reader.ReadBytes(bytes);

        //            int bytesForSamp = bitDepth / 8;
        //            int samps = bytes / bytesForSamp;


        //            float[] asFloat = null;
        //            switch (bitDepth)
        //            {
        //                case 64:
        //                    double[]
        //                    asDouble = new double[samps];
        //                    Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
        //                    asFloat = Array.ConvertAll(asDouble, e => (float)e);
        //                    break;
        //                case 32:
        //                    asFloat = new float[samps];
        //                    Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
        //                    break;
        //                case 16:
        //                    Int16[]
        //                    asInt16 = new Int16[samps];
        //                    Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
        //                    asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
        //                    break;
        //                default:
        //                    return false;
        //            }

        //            switch (channels)
        //            {
        //                case 1:
        //                    L = asFloat;
        //                    R = null;
        //                    return true;
        //                case 2:
        //                    L = new float[samps];
        //                    R = new float[samps];
        //                    for (int i = 0, s = 0; i < samps; i++)
        //                    {
        //                        L[i] = asFloat[s++];
        //                        R[i] = asFloat[s++];
        //                    }
        //                    return true;
        //                default:
        //                    return false;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        Debug.Log("...Failed to load note: " + filename);
        //        return false;
        //        //left = new float[ 1 ]{ 0f };
        //    }

        //    return false;
        //}
    }
}
