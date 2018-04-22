﻿using System;
using Challenge2.Library.Contracts;

namespace Challenge2.Library.Services.WatchStates
{
    public class WatchRunning : WatchState
    {
        public WatchRunning(UserInterface ui, IDisposable timer)
        {
            this.ui = ui;
            this.timer = timer;

            ui.StartStopEnabled = true;
            ui.ResetEnabled = false;
            ui.HoldEnabled = true;
        }

        public void ShowTime(TimeSpan ts)
        {
            ui.Text = ts.ToString();
        }

        public WatchState StartStop(Action<TimeSpan> showTime)
        {
            timer.Dispose();
            return new WatchStopped(ui);
        }

        public WatchState Hold()
        {
            return new WatchPaused(ui, timer);
        }

        public WatchState Reset()
        {
            return this;
        }

        //

        private readonly UserInterface ui;
        private readonly IDisposable timer;
    }
}