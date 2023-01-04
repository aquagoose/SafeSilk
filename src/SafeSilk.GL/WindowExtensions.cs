using Silk.NET.Core.Contexts;

namespace SafeSilk.GL
{
    public static class WindowExtensions
    {
        public static SafeGL CreateSafeOpenGL(this IGLContextSource src) => SafeGL.GetApi(src);
    }
}