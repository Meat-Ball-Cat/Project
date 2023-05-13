using System;
using System.Diagnostics;

namespace Model
{
    //TODO переделать на тики
    public class Cooldown
    {
        private readonly int _cooldownMS;
        private readonly Stopwatch _stopwatch = new();

        private int _delay = 0;

        public float ElapsedFrac
            => Math.Max(Update().ElapsedMilliseconds - _delay, 0f) / _cooldownMS;
        
        public bool CoolingDown
            => Update().IsRunning && _stopwatch.ElapsedMilliseconds >= _delay;

        /// <param name="cooldownMS">Desired cooldown in MILLISECONDS. Has to be positive.</param>
        public Cooldown(int cooldownMS)
        {
            if (cooldownMS <= 0)
                throw new ArgumentException("Cooldown must be positive.");
                
            _cooldownMS = cooldownMS;
        }

        public void Start()
            => DelayedStart(0);

        /// <summary>
        /// This Cooldown will start returning Cooldown.CoolingDown as true only after the delay time has elapsed
        /// </summary>
        /// <param name="delayMS"></param>
        public void DelayedStart(int delayMS)
        {
            if (delayMS < 0)
                throw new ArgumentException("Cannot set negative cooldown delay.");
            _delay = delayMS;
            _stopwatch.Restart();
        }

        private Stopwatch Update()
        {
            if (_stopwatch.ElapsedMilliseconds - _delay >= _cooldownMS) 
                _stopwatch.Reset();

            return _stopwatch;
        }
    }
}