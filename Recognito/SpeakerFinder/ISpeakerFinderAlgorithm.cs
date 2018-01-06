using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.SpeakerFinder
{
    public interface ISpeakerFinderAlgorithm
    {
        List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder);
    }

}
