using Microsoft.Extensions.DependencyInjection;
using Raylib_cs;
using ShiverEight.CHIP8.cpu;
using System.Numerics;

namespace ShiverEight.FrontEnd;

public class Window
{
    private RenderTexture2D _displayTexture;
    private RenderTexture2D _menuTexture;
    private EmuStates _gameState = EmuStates.Menu;

    private readonly State _state;

    public Window(State state)
    { 
        _state = state;
    }

    public void Spawn()
    {
        Raylib.InitWindow(600, 400, "No cart inserted");
        Raylib.SetTargetFPS(60);

        Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);

        _displayTexture = Raylib.LoadRenderTexture(64, 32);

        int menuWidth = 200, menuHeight = 100;
        _menuTexture = Raylib.LoadRenderTexture(menuWidth, menuHeight);

        while (!Raylib.WindowShouldClose())
        {
            if (_gameState == EmuStates.Menu)
            {
                Vector2 mouse = Raylib.GetMousePosition();
                Vector2 virtualMouse = new Vector2(
                    mouse.X / Raylib.GetScreenWidth() * menuWidth, 
                    mouse.Y / Raylib.GetScreenHeight() * menuHeight
                );

                if (Raylib.CheckCollisionRecs(
                        new Rectangle(100 - 12, 40, 25, 13),
                        new Rectangle((int)virtualMouse.X, (int)virtualMouse.Y, 5, 5)
                    )
                )
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        if (Shiver.Cart != null)
                        {
                            _state.ReadCartIntoMemory();

                            Thread cpu = new Thread(Shiver.Services.GetRequiredService<Loop>().Execute);
                            cpu.Start();

                            Raylib.HideCursor();

                            _gameState = EmuStates.Playing;
                        }
                        else
                        {
                            ShiverCart.GetCartPath();
                        }
                    }
                }

                if (Raylib.CheckCollisionRecs(
                        new Rectangle(100 - 19, 55, 40, 13),
                        new Rectangle((int)virtualMouse.X, (int)virtualMouse.Y, 5, 5)
                    )
                )
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        ShiverCart.GetCartPath();
                    }
                }

                Raylib.BeginTextureMode(_menuTexture);
                    Raylib.ClearBackground(Color.BLACK);

                    Raylib.DrawText("Shiver Eight", 100 - ((2 * 12) + 6), 10, 10, Color.WHITE);

                    Raylib.DrawRectangleLinesEx(
                        new Rectangle(100 - 12, 40, 25, 13),
                        1f,
                        Color.WHITE
                    );
                    Raylib.DrawText("Play", 100 - 10, 41, 1, Color.WHITE);

                    Raylib.DrawRectangleLinesEx(
                        new Rectangle(100 - 19, 55, 40, 13),
                        1f,
                        Color.WHITE
                    );
                    Raylib.DrawText("Choose", 100 - 17, 56, 1, Color.WHITE);

                if (Shiver.Cart != null)
                        Raylib.DrawText($"Cart: {ShiverCart.GetShortName(Shiver.Cart)}", 5, 88, 1, Color.WHITE);
                    else
                        Raylib.DrawText("Cart: None", 5, 88, 1, Color.WHITE);
                Raylib.EndTextureMode();
            }

            if (_gameState == EmuStates.Playing)
            {
                Raylib.BeginTextureMode(_displayTexture);
                    Raylib.ClearBackground(Color.BLACK);

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
            }
            
            Raylib.BeginDrawing();
                if (_gameState == EmuStates.Menu)
                {
                    Raylib.DrawTexturePro(
                        _menuTexture.texture,
                        new Rectangle(0, 0, _menuTexture.texture.width, -_menuTexture.texture.height),
                        new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
                        new Vector2(0, 0),
                        0,
                        Color.WHITE
                    );
                }

                if (_gameState == EmuStates.Playing)
                {
                    Raylib.DrawTexturePro(
                        _displayTexture.texture,
                        new Rectangle(0, 0, _displayTexture.texture.width, -_displayTexture.texture.height),
                        new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()),
                        new Vector2(0, 0),
                        0,
                        Color.WHITE
                    );
                }
            Raylib.EndDrawing();
        }

        Raylib.UnloadRenderTexture(_displayTexture);
        Raylib.UnloadRenderTexture(_menuTexture);
        Raylib.CloseWindow();
    }
}
