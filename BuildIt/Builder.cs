using System;

namespace BuildIt
{
    public class Builder
    {
        public static Builder instance;

        public bool IsRunning { get; internal set; }

        public string Status { get; internal set; }

        private int ticks;
        public int Ticker { get => ticks; }

        public void Next()
        {
            if(IsRunning)
            {
                ticks++;

                if (ticks % 30 == 0)
                {
                    Status = $"Ticker {ticks}";
                    Update();
                }

                if (ticks > 2048)
                    Stop();
            }
        }

        public void Init()
        {
            // do nothing
        }

        public void Start()
        {
            IsRunning = true;
            ticks = 0;
            Status = "Starting";
            Update();
        }

        public event EventHandler OnUpdate = delegate { };

        public void Update()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            IsRunning = false;
            Status = "Stopped";
            Update();
        }
    }
}
