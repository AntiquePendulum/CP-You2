﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Reactive;
using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Prism.Mvvm;

namespace CP_You2.Models
{
    public class CpuPercentageManager : BindableBase
    {
        private const string CATEGORY_NAME = "Processor";
        private const string COUNTER_NAME = "% Processor Time";
        private const string INSTANCE_NAME = "_Total";
        private static readonly PerformanceCounter TotalCounter = new PerformanceCounter(CATEGORY_NAME, COUNTER_NAME, INSTANCE_NAME);
        private static readonly PerformanceCounter[] Counters = new PerformanceCounter[Environment.ProcessorCount];

        private int _totalPercentage = -1;
        public int TotalPercentage
        {
            get
            {
                if (_totalPercentage != -1) return _totalPercentage;

                _totalPercentage = 0;
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000))
                    .Subscribe(_ => this.SetProperty(ref _totalPercentage, (int)TotalCounter.NextValue() ));
                return _totalPercentage;
            }
        }

        private int[] _cpuPercentages;

        public int[] CpuPercentages
        {
            get
            {
                if (_cpuPercentages != null) return _cpuPercentages;
                _cpuPercentages = new int[Environment.ProcessorCount];
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000))
                    .Subscribe(
                        _ => this.SetProperty(ref _cpuPercentages, Counters.Select(c => (int)c.NextValue()).ToArray())
                    );
                return _cpuPercentages;
            }
        }

        public CpuPercentageManager()
        {
            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Counters[i] = new PerformanceCounter(CATEGORY_NAME, COUNTER_NAME, i.ToString());
            }
        }

    }
}
