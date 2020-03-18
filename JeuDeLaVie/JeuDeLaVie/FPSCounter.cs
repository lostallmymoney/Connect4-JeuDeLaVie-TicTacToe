using System.Collections.Generic;
using System.Linq;

namespace JeuDeLaVie
{
    public class FPSCounter
    {
        public FPSCounter()
        {
        }

        public long FPSTotal { get; private set; }
        public float TotalS { get; private set; }
        public float AvgFPS { get; private set; }
        public float CurrentFPS { get; private set; }

        public int NbFrameCount { get; } = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public void Add(float deltaTime)
        {
            CurrentFPS = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFPS);

            if (_sampleBuffer.Count > NbFrameCount)
            {
                _sampleBuffer.Dequeue();
                AvgFPS = _sampleBuffer.Average(i => i);
            }
            else
            {
                AvgFPS = CurrentFPS;
            }

            FPSTotal++;
            TotalS += deltaTime;
        }
    }
}
