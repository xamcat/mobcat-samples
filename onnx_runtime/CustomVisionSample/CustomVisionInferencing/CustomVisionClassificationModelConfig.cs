namespace CustomVisionInferencing
{
    public static class CustomVisionClassificationModelConfig
    {
        public const string ModelInputName = "data";
        public const string ModelClassLabelOutputName = "classLabel";
        public const string ModelLossOutputName = "loss";
        public const int BatchSize = 1;
        public const int NumberOfChannels = 3;
        public const int ImageSizeX = 224;
        public const int ImageSizeY = 224;
        public const int RedChannelIndex = 2;
        public const int GreenChannelIndex = 1;
        public const int BlueChannelIndex = 0;
    }
}