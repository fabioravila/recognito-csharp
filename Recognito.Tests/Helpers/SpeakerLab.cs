using System.Collections.Generic;

namespace Recognito.Tests.Helpers
{
    public class SpeakerLab
    {
        public float TrainRatio { get; set; }
        public List<SpeakerAudioInfo> TrainSet { get; set; }
        public List<SpeakerAudioInfo> VerifySet { get; set; }
    }
}
