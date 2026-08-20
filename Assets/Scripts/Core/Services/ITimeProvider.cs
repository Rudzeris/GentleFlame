using System;

namespace Assets.Scripts.Core.Services
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }

    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
