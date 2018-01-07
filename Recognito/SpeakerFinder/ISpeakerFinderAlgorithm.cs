using System.Collections.Generic;
using System.IO;

namespace Recognito.SpeakerFinder
{
    public interface ISpeakerFinderAlgorithm
    {
        List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder);
    }
}
