using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.Playground
{
    class Program
    {

        static Recognito<string> recognito = new Recognito<string>(16000);

        static void Main(string[] args)
        {
            try
            {
                string base_dir = "C:\\Users\\fabio\\Desktop\\audio_sample";

                var tests = new List<string>();

                foreach (var pessoas in Directory.GetDirectories(base_dir).OrderBy(f => f))
                {
                    var info = new DirectoryInfo(pessoas);
                    var nome = info.Name;

                    Console.WriteLine($"nome:{nome}");

                    VoicePrint voice = null;

                    foreach (var audio in Directory.GetFiles(pessoas, "audio_*.wav", SearchOption.TopDirectoryOnly))
                    {
                        Console.WriteLine($"nome:{audio}");


                        using (var fs = new FileStream(audio, FileMode.Open))
                        {
                            if (voice == null)
                                voice = recognito.CreateVoicePrint(nome, fs);
                            else
                                voice = recognito.MergeVoiceSample(nome, fs);
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
                Console.WriteLine("Press Any Key do Close");
                Console.ReadKey();
            }

        }
    }
}
