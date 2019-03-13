using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Recognito.Tests.Helpers
{
    public class DatasetWork
    {
        //litle script to sort dataset audio to split in trains ans verify
        //DATASET USER IS: “ST-AEDS-20180100_1, Free ST American English Corpus”.
        //I user ratio of .07 do train(70% train-30% verify)
        string dataset_path;
        string dataset_train_path;
        string dataset_verify_path;
        float train_ratio = 0.95f;

        string basePath;
        public DatasetWork(string basePath, float train_ratio)
        {
            this.basePath = basePath;
            dataset_path = Path.Combine(basePath, "full");
            dataset_train_path = Path.Combine(basePath, "train");
            dataset_verify_path = Path.Combine(basePath, "verify");
            this.train_ratio = train_ratio;
        }


        public SpeakerLab Split_Train_Verify(bool renewFiles, Func<string, SpeakerAudioInfo> speakerPatternAction)
        {
            if (!Directory.Exists(dataset_train_path)) Directory.CreateDirectory(dataset_train_path);

            if (!Directory.Exists(dataset_verify_path)) Directory.CreateDirectory(dataset_verify_path);

            renewFiles = (Directory.GetFiles(dataset_train_path).Count() == 0 || Directory.GetFiles(dataset_verify_path).Count() == 0);


            if (renewFiles)
            {
                if (renewFiles && Directory.Exists(dataset_train_path))
                {
                    Directory.Delete(dataset_train_path, true);
                    Directory.CreateDirectory(dataset_train_path);
                }

                if (renewFiles && Directory.Exists(dataset_verify_path))
                {
                    Directory.Delete(dataset_verify_path, true);
                    Directory.CreateDirectory(dataset_verify_path);
                }
            }


            if (!renewFiles)
            {
                return new SpeakerLab
                {
                    TrainRatio = train_ratio,
                    TrainSet = Directory.GetFiles(dataset_train_path).Select(p => speakerPatternAction(p)).ToList(),
                    VerifySet = Directory.GetFiles(dataset_verify_path).Select(p => speakerPatternAction(p)).ToList()
                };
            }



            var dict = new Dictionary<string, List<SpeakerAudioInfo>>();
            foreach (var file in Directory.GetFiles(dataset_path, "*.*", SearchOption.AllDirectories).OrderBy(f => f))
            {
                var info = speakerPatternAction(file);

                if (info == null)
                    continue;

                if (!dict.ContainsKey(info.SpeakerId))
                    dict[info.SpeakerId] = new List<SpeakerAudioInfo>();

                dict[info.SpeakerId].Add(info);
            }

            var train_set = new List<SpeakerAudioInfo>();
            var verify_set = new List<SpeakerAudioInfo>();

            //now we will sort the destiny of files of all speakers
            foreach (var item in dict)
            {
                var list = item.Value;
                var train_count = (int)Math.Round(list.Count * train_ratio, 0);

                //sort the list
                list.Shuffle();

                for (int i = 0; i < list.Count; i++)
                {
                    var train = i <= train_count;

                    if (renewFiles)
                    {
                        var dest_folder = (train ? dataset_train_path : dataset_verify_path);
                        File.Copy(list[i].FullPath, Path.Combine(dest_folder, list[i].FullFileName));
                    }

                    if (train)
                    {
                        train_set.Add(list[i]);
                    }
                    else
                    {
                        verify_set.Add(list[i]);
                    }
                }
            }


            return new SpeakerLab
            {
                TrainRatio = train_ratio,
                TrainSet = train_set,
                VerifySet = verify_set
            };
        }
    }
}
