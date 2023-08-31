﻿namespace Project.AppCore.Options
{
    public sealed class Token
    {
        public int MaxFreeTime { get; set; }
#if DEBUG
        public int Expire => 0;
        public int LimitedFreeTime => 60;
#else
        public int Expire { get; set; }
public int LimitedFreeTime => MaxFreeTime < 300 ? 300 : MaxFreeTime;
#endif
    }
}