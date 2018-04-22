﻿using System;
using System.Diagnostics;
using System.Timers;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services
{
    public class RealTimer : SystemTimer
    {
        public IDisposable Start(TimeSpan interval, Action<TimeSpan> callback)
        {
            var sw = new Stopwatch();
            var timer = new Timer(interval.TotalMilliseconds);

            void ElapsedEventHandler(object sender, ElapsedEventArgs e) => callback(sw.Elapsed);

            timer.Elapsed += ElapsedEventHandler;

            sw.Start();
            timer.Start();

            return new Disposable(
                () =>
                {
                    timer.Stop();
                    sw.Stop();

                    timer.Elapsed -= ElapsedEventHandler;
                    timer.Dispose();
                });
        }
    }
}