using Microsoft.Extensions.DependencyInjection;
using ShiverEight.CHIP8.cpu;
using ShiverEight.FrontEnd;
using System.IO;

namespace ShiverEight;

public static class Shiver
{
    public static string Cart = string.Empty;
    public static ServiceProvider Services = null!;

    public static void Main(string[] args)
    {
        Services = new ServiceCollection()
            .AddSingleton<State>()
            .AddSingleton<Window>()
            .AddSingleton<Loop>()
            .BuildServiceProvider();

        State state = Services.GetRequiredService<State>();

        // This is just a test dont eat me alive
        Cart = "C:/Users/Nattie/Downloads/IBM Logo.ch8";

        byte[] CartBytes = File.ReadAllBytes(Cart);
        Array.Copy(CartBytes, 0, state.Memory, 0x200, CartBytes.Length);

        state.LoadFontIntoMemory();

        Thread cpu = new Thread(Services.GetRequiredService<Loop>().Execute);
        Thread window = new Thread(Services.GetRequiredService<Window>().Spawn);

        cpu.Start();
        window.Start();

        cpu.Join();
        window.Join();
    }
}
