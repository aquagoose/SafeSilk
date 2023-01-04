using System;
using System.Drawing;
using SafeSilk.GL;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;

namespace Tests.GL;

public class Main : IDisposable
{
    private IWindow _window;
    private SafeGL _gl;

    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;

    public Main(WindowOptions options)
    {
        _window = Window.Create(options);
        _window.Load += Initialize;
        _window.Render += Draw;
    }

    private void Initialize()
    {
        _gl = _window.CreateSafeOpenGL();
        
        _gl.ClearColor(Color.CornflowerBlue);

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        float[] vertices = new[]
        {
            0.0f,  0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
           -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f
        };

        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

        uint[] indices = new[]
        {
            0u, 1u, 2u
        };

        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

        const string vertexCode = @"
#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

out vec4 frag_color;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_color = aColor;
}";

        const string fragmentCode = @"
#version 330 core

in vec4 frag_color;

out vec4 out_color;

void main()
{
    out_color = frag_color;
}";

        uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);

        _gl.ShaderSource(vertexShader, vertexCode);
        _gl.ShaderSource(fragmentShader, fragmentCode);
        
        _gl.CompileShader(vertexShader);
        Console.WriteLine(_gl.GetShaderInfoLog(vertexShader));
        
        _gl.CompileShader(fragmentShader);
        Console.WriteLine(_gl.GetShaderInfoLog(fragmentShader));

        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);
        _gl.LinkProgram(_program);
        Console.WriteLine(_gl.GetProgramInfoLog(_program));
        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
        
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 28, 0);
        
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 28, 12);
    }
    
    private void Draw(double dt)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        
        _gl.BindVertexArray(_vao);
        _gl.UseProgram(_program);
        
        _gl.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);
    }

    public void Run()
    {
        _window.Run();
    }

    public void Dispose()
    {
        _window.Dispose();
    }
}