namespace Oty.Things;

public static class DisposedExceptionHelper
{
    public static void ThrowIfDisposed(bool disposed, string objectName)
    {
        if (disposed)
        {
            throw new ObjectDisposedException(objectName);
        }
    }
}