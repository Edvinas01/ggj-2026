namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal readonly struct AlcoholChangedArgs
    {
        public float RatioPrev { get; }
        public float Ratio { get; }

        public AlcoholChangedArgs(float retioPrev, float ratio)
        {
            RatioPrev = retioPrev;
            Ratio = ratio;
        }
    }
}
