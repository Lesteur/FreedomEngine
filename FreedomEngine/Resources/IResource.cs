using System;

namespace FreedomEngine.Resources
{
    public interface IResource<out TNative> : IDisposable
    {
        TNative Native { get; }

        bool IsDisposed { get; }
    }
}