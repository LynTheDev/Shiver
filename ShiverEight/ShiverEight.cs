using Microsoft.Extensions.DependencyInjection;
using ShiverEight.CHIP8.cpu;
using ShiverEight.FrontEnd;
using System.IO;

namespace ShiverEight;

public static class Shiver
{
    public static string? Cart;
    public static ServiceProvider Services = null!;

    public static void Main(string[] args)
    {
        Services = new ServiceCollection()
            .AddSingleton<State>()
            .AddSingleton<Window>()
            .AddSingleton<Loop>()
            .BuildServiceProvider();

        State state = Services.GetRequiredService<State>();

        state.LoadFontIntoMemory();

        Thread window = new Thread(Services.GetRequiredService<Window>().Spawn);
        window.Start();

        //cpu.Join();
        window.Join();
    }
}
