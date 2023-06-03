using Microsoft.Extensions.DependencyInjection;
using ShiverEight.CHIP8.cpu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiverEight.FrontEnd;

public static class ShiverConsole
{
    public static State state = Shiver.Services.GetRequiredService<State>();

    public static void Update()
    {
        Console.Clear();

        for (int l = 0; l < state.Display.GetLength(1); l++)
        {
            for (int k = 0; k < state.Display.GetLength(0); k++)
            {
                if (state.Display[k, l])
                    Console.Write("#");
                else
                    Console.Write(" ");
            }

            Console.WriteLine();
        }
    }
}
