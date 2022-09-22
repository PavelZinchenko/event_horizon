using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Services.InternetTime
{
    public class InternetTimeService : IInitializable, IDisposable
    {
        [Inject]
        public InternetTimeService(ServerTimeReceivedSignal.Trigger timeReceivedTrigger)
        {
            _timeReceivedTrigger = timeReceivedTrigger;
        }

        public bool HasBeenReceived { get; private set; }
        public DateTime DateTime => HasBeenReceived ? _dateTime : DateTime.Now;

        public int TotalDays => (int) (DateTime.Ticks / TimeSpan.TicksPerDay);

        public void Initialize()
        {
            Observable.Timer(DateTimeOffset.Now, TimeSpan.FromHours(1), Scheduler.ThreadPool).Select(RequestTime).Retry().
                ObserveOnMainThread().Subscribe(OnTimeReceived).AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        private void OnTimeReceived(DateTime time)
        {
            _dateTime = time;
            HasBeenReceived = true;
            _timeReceivedTrigger.Fire(_dateTime);
            OptimizedDebug.Log("Internet time received - " + _dateTime);
        }

        private static DateTime RequestTime(long _)
        {
            var client = new TcpClient("time.nist.gov", 13);
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                var response = streamReader.ReadToEnd();
                OptimizedDebug.Log("InternetTimeService: Response - " + response);
                var utcDateTimeString = response.Substring(7, 17);
                return DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }
        }

        private DateTime _dateTime;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly ServerTimeReceivedSignal.Trigger _timeReceivedTrigger;
    }

    public class ServerTimeReceivedSignal : SmartWeakSignal<DateTime>
    {
        public class Trigger : TriggerBase { }
    }
}
