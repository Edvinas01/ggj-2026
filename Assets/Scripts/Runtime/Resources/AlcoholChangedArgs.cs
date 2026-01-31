namespace RIEVES.GGJ2026.Runtime.Resources
{
    internal readonly struct AlcoholChangedArgs
    {
        public int ValuePrev { get; }

        public int ValueNext { get; }

        public float Ratio { get; }

        public AlcoholChangedArgs(int valuePrev, int valueNext, float ratio)
        {
            ValuePrev = valuePrev;
            ValueNext = valueNext;
            Ratio = ratio;
        }
    }
}
