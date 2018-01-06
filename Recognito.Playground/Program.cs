using NAudio.Wave;
using Recognito.Utils;
using Recognito.Vad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Recognito.Playground
{
    class Program
    {
        const int sampleRate = 16000;
        static Recognito<string> recognito = new Recognito<string>(sampleRate);

        static void Main(string[] args)
        {
            var start = DateTime.Now;

            try
            {
                string base_dir = "C:\\Users\\fabio\\Desktop\\audio_sample";

                var tests = new List<string>();

                using (var waveOut = new WaveOutEvent())
                {

                    var voiceDetector = new AutocorrellatedVoiceActivityDetector();


                    foreach (var pessoas in Directory.GetDirectories(base_dir).OrderBy(f => f))
                    {
                        var info = new DirectoryInfo(pessoas);
                        var nome = info.Name;

                        Console.WriteLine($"nome:{nome}");

                        VoicePrint voice = null;

                        foreach (var audio in Directory.GetFiles(pessoas, "audio_*.wav", SearchOption.TopDirectoryOnly))
                        {
                            Console.WriteLine($"nome:{audio}");




                            using (var fs = File.OpenRead(audio))
                            {
                                if (voice == null)
                                    voice = recognito.CreateVoicePrint(nome, fs);
                                else
                                    voice = recognito.MergeVoiceSample(nome, fs);
                            }

                            using (var wr = new WaveFileReader(audio))
                            {
                                Console.WriteLine("Play Original");
                                waveOut.Init(wr);
                                waveOut.PlayAndWait();
                            }

                            using (var fs = new FileStream(audio, FileMode.Open))
                            {
                                var sentence = AudioConverter.ConvertAudioToDoubleArray(fs, sampleRate);

                                var words = voiceDetector.SplitBySilence(sentence, sampleRate);

                                var aa = AudioConverter.WriteAudioInputStream(sentence);



                                if (words.Length > 1)
                                {
                                    foreach (var word in words)
                                    {
                                        var aw = AudioConverter.WriteAudioInputStream(word);

                                        using (var wr = new WaveFileReader(aw))
                                        {
                                            Console.WriteLine("Play Word");
                                            waveOut.Init(wr);
                                            waveOut.PlayAndWait();
                                            Thread.Sleep(1000);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                Console.WriteLine("\n\nTestes");
                tests = Directory.GetFiles(base_dir, "teste_*.wav", SearchOption.AllDirectories).ToList();

                foreach (var test in tests)
                {
                    Console.WriteLine($"Testando: {test}");

                    using (var fs = new FileStream(test, FileMode.Open))
                    {
                        var identify = recognito.Identify(fs).FirstOrDefault();

                        Console.WriteLine($"identify.Key:{identify.Key},identify.Distance: {identify.Distance}, identify.LikelihoodRatio:{identify.LikelihoodRatio}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR]");
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine($"Time:{(DateTime.Now - start)}");

                Console.WriteLine("Press Any Key do Close");
                Console.ReadKey();
            }

        }
    }
}
