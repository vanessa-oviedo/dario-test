using Wheelzy.Application.Interfaces;

namespace Wheelzy.Infrastructure
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
