using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomVisionInferencing
{
    public sealed class CustomVisionClassificationModelOutput
    {
        public string TopResultLabel { get; private set; }
        public float TopResultScore { get; private set; }
        public IDictionary<string, float> LabelScores { get; private set; }

        CustomVisionClassificationModelOutput(){}

        public static CustomVisionClassificationModelOutput Create(string topLabel, IDictionary<string, float> labelScores)
        {
            var topLabelValue = topLabel ?? throw new ArgumentException(nameof(topLabel));
            var labelScoresValue = labelScores ?? throw new ArgumentException(nameof(labelScores));

            return new CustomVisionClassificationModelOutput
            {
                TopResultLabel = topLabelValue,
                TopResultScore = labelScoresValue.First(i => i.Key == topLabelValue).Value,
                LabelScores = labelScoresValue,
            };
        }
    }
}