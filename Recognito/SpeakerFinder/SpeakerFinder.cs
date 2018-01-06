using Recognito.Distances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.SpeakerFinder
{
    public class SpeakerFinder
    {
        readonly ISpeakerFinderAlgorithm finderAlgorithm;

        public SpeakerFinder(ISpeakerFinderAlgorithm finderAlgorithm, double distanceThreshold)
        {
            this.finderAlgorithm = finderAlgorithm;
        }


        List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string filesSearchSpeaker)
        {
            if (!Directory.Exists(filesSearchSpeaker))
                throw new ArgumentNullException(nameof(filesSearchSpeaker), "Directory not find");

            return finderAlgorithm.FindAudioFilesContainingSpeaker(speakerAudioFile, filesSearchSpeaker);
        }
    }
}
