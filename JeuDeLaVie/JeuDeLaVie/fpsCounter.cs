using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie
{
    public class FPSCounter
    {
        public FPSCounter()
        {
        }

        public long FPSTotal { get; private set; }
        public float totalS { get; private set; }
        public float avgFPS { get; private set; }
        public float CurrentFPS { get; private set; }

        public int NbFrameCount => nbFrameCount;

        private int nbFrameCount = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public void add(float deltaTime)
        {
            CurrentFPS = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFPS);

            if (_sampleBuffer.Count > NbFrameCount)
            {
                _sampleBuffer.Dequeue();
                avgFPS = _sampleBuffer.Average(i => i);
            }
            else
            {
                avgFPS = CurrentFPS;
            }

            FPSTotal++;
            totalS += deltaTime;
        }
    }
}
