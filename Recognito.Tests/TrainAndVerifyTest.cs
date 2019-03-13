using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recognito.Tests.Helpers;
using System.Linq;
using System.Diagnostics;

namespace Recognito.Tests
{
    [TestClass]
    public class TrainAndVerifyTest
    {
        static Recognito<string> engine;
        static SpeakerLab lab;
        static DatasetWork work;


        const string dataset_base_path = @"C:\Users\fabio\Desktop\speaker recognition\datasets\Mini LibriSpeech ASR corpus\dataset";
        //const string dataset_base_path = @"C:\Users\fabio\Desktop\speaker recognition\datasets\ST_AEDS_20180100_1\ST-AEDS-20180100_1-OS";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            engine = new Recognito<string>(16000);
            work = new DatasetWork(dataset_base_path, 0.01f);

            lab = work.Split_Train_Verify(renewFiles: false, speakerPatternAction: SpeakerAudioInfo.From_Mini_LibriSpeech_ASR);
            //lab = work.Split_Train_Verify(renewFiles: false, speakerPatternAction: SpeakerAudioInfo.From_ST_AEDS_File);


            //train
            var start = DateTime.Now;

            foreach (var item in lab.TrainSet)
            {
                using (var fs = File.OpenRead(item.FullPath))
                {
                    engine.CreateOrMergeVoicePrint(item.SpeakerId, fs);
                }
            }



            var total = (DateTime.Now - start);
            var per_file = (int)(total.TotalMilliseconds / lab.TrainSet.Count);

            Debug.WriteLine($"total_time: {total}");
            Debug.WriteLine($"ms_per_file: {per_file}");
            Debug.WriteLine($"total_files: {lab.TrainSet.Count}");
        }

        [TestMethod]
        public void Verify()
        {
            lab.VerifySet.Shuffle();

            var total_match = 0;
            var total_miss = 0;
            var total_error = 0;

            foreach (var verify in lab.VerifySet)
            {
                var expected = verify.SpeakerId;
                using (var fs = File.OpenRead(verify.FullPath))
                {
                    var result = engine.Identify(fs).FirstOrDefault();

                    if (result != null)
                    {
                        if (expected == result.Key)
                            total_match++;
                        else
                            total_miss++;
                    }
                    else
                    {
                        total_error++;
                    }
                }
            }


            Debug.WriteLine($"total_match: {total_match}");
            Debug.WriteLine($"total_miss: {total_miss}");
            Debug.WriteLine($"total_error: {total_error}");
        }


        [ClassCleanup]
        public static void ClassCleanUp()
        {
        }
    }
}
