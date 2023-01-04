using System;
using System.Runtime.CompilerServices;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using SGL = Silk.NET.OpenGL.GL;

namespace SafeSilk.GL
{
    public unsafe class SafeGL : SGL
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BufferData<T>(BufferTargetARB target, T[] data, BufferUsageARB usage) where T : unmanaged
        {
            fixed (void* ptr = data)
                BufferData(target, (nuint) (data.Length * sizeof(T)), ptr, usage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, uint stride, int offset)
        {
            VertexAttribPointer(index, size, type, normalized, stride, (void*) offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawElements(PrimitiveType mode, uint count, DrawElementsType type, int indices)
        {
            DrawElements(mode, count, type, (void*) indices);
        }

        public new static SafeGL GetApi(Func<string, nint> getProcAddress)
        {
            SGL gl = SGL.GetApi(getProcAddress);
            return new SafeGL(gl.Context);
        }
        
        public new static SafeGL GetApi(IGLContext ctx)
        {
            SGL gl = SGL.GetApi(ctx);
            return new SafeGL(gl.Context);
        }

        public new static SafeGL GetApi(IGLContextSource contextSource)
        {
            SGL gl = SGL.GetApi(contextSource);
            return new SafeGL(gl.Context);
        }
        
        public new static SafeGL GetApi(INativeContext ctx)
        {
            SGL gl = SGL.GetApi(ctx);
            return new SafeGL(gl.Context);
        }

        public SafeGL(INativeContext ctx) : base(ctx)
        {
        }
    }
}