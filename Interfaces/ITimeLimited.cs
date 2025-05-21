using SFML.System;

namespace k.Interfaces
{
    public interface ITimeLimited
    {
        Clock Timer { get; set; }
        bool IsExpired();
        bool InUse { get; set; } // Вказує, чи ефект наразі активний
    }
}