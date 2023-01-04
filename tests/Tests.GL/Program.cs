using Silk.NET.Maths;
using Silk.NET.Windowing;
using Tests.GL;

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 600)
};

using Main main = new Main(options);
main.Run();