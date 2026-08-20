using System;

namespace Assets.Scripts.Core.Services
{
    /// <summary>
    /// Источник времени для офлайн-расчёта. Работает в UTC и защищён от перевода часов назад:
    /// отрицательный интервал трактуется как нулевой, а не как «игрок был в отлучке минус два часа».
    /// </summary>
    public class TimeService
    {
        private readonly ITimeProvider _provider;

        public TimeService(ITimeProvider provider)
        {
            _provider = provider ?? new SystemTimeProvider();
        }

        public DateTime UtcNow => _provider.UtcNow;

        public long NowTicks => _provider.UtcNow.Ticks;

        /// <summary>Сколько секунд прошло с указанной UTC-метки. Никогда не отрицательно.</summary>
        public double SecondsSince(long utcTicks)
        {
            if (utcTicks <= 0)
                return 0d;

            var elapsed = (_provider.UtcNow - new DateTime(utcTicks, DateTimeKind.Utc)).TotalSeconds;
            return elapsed > 0d ? elapsed : 0d;
        }
    }
}
