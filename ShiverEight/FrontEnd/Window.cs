using Raylib_cs;
using ShiverEight.CHIP8.cpu;
using System.Numerics;

namespace ShiverEight.FrontEnd;

public class Window
{
    private RenderTexture2D displayTexture;
    private readonly State _state;

    public Window(State state)
    { 
        _state = state;
    }

    public void Spawn()
    {
        Raylib.InitWindow(600, 400, Shiver.Cart);
        Raylib.SetTargetFPS(60);

        Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);

        displayTexture = Raylib.LoadRenderTexture(64, 32);

        while (!Raylib.WindowShouldClose())
        { 
            Raylib.BeginTextureMode(displayTexture);
                for (int y = 0; y < _state.Display.GetLength(1); y++)
                {
                    for (int x = 0; x < _state.Display.GetLength(0); x++)
                    {
                        if (_state.Display[x, y])
                            Raylib.DrawPixel(x, y, Color.WHITE);
                        else
                            Raylib.DrawPixel(x, y, Color.BLACK);
                    }
                }
            Raylib.EndTextureMode();

            Raylib.BeginDrawing();
                Raylib.DrawTexturePro(
                    displayTexture.texture,
                    new Rectangle(0, 0, displayTexture.texture.width, -displayTexture.texture.height),
                    new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
                    new Vector2(0, 0),
                    0,
                    Color.WHITE
                );
            Raylib.EndDrawing();
        }

        Raylib.UnloadRenderTexture(displayTexture);
        Raylib.CloseWindow();
    }
}
