using System;

namespace FreedomEngine.Resources
{
    public abstract class Resource<TNative> : IResource<TNative> where TNative : class
    {
        protected readonly TNative _native;

        protected bool _disposed = false;


        public TNative Native => _native;

        public bool IsDisposed => _disposed;


        public Resource(TNative native)
        {
            _native = native ?? throw new ArgumentNullException(nameof(native));
        }


        protected void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                DisposeNativeResource();
            }

            _disposed = true;
        }

        protected virtual void DisposeNativeResource()
        {
            // Default: Do nothing. Override if the native resource needs disposal.
            // Note: Resources managed by ContentManager should NOT be disposed here.
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Resource()
        {
            Dispose(false);
        }
    }
}