using System;
using System.Drawing;
using System.IO;
using SafeSilk.GL;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Tests.GL;

public class Main : IDisposable
{
    private IWindow _window;
    private SafeGL _gl;

    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;
    private uint _texture;

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
           -0.5f,  0.5f, 0.0f, 0.0f, 0.0f,
            0.5f,  0.5f, 0.0f, 1.0f, 0.0f,
            0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
           -0.5f, -0.5f, 0.0f, 0.0f, 1.0f 
        };

        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

        uint[] indices = new[]
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

        const string vertexCode = @"
#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

out vec2 frag_texCoords;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

        const string fragmentCode = @"
#version 330 core

in vec2 frag_texCoords;

out vec4 out_color;

uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
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

        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("Content/awesomeface.png"),
            ColorComponents.RedGreenBlueAlpha);
        
        _texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)result.Width, (uint)result.Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
        
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 20, 0);
        
        _gl.EnableVertexAttribArray(1);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 20, 12);
    }
    
    private void Draw(double dt)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        
        _gl.BindVertexArray(_vao);
        _gl.UseProgram(_program);
        
        _gl.Uniform1(_gl.GetUniformLocation(_program, "uTexture"), 0);
        
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        
        _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
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