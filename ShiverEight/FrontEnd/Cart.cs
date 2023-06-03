using NativeFileDialogSharp;
using Raylib_cs;

namespace ShiverEight.FrontEnd;

public static class ShiverCart
{
    public static string GetShortName(string fullCart) 
        => fullCart.Split('\\').Last();

    public static void GetCartPath()
    {
        DialogResult res = Dialog.FileOpen("ch8", "");

        if (res.IsOk)
        {
            if (!res.Path.EndsWith(".ch8"))
                return;

            Shiver.Cart = res.Path;
            Raylib.SetWindowTitle(GetShortName(res.Path));
        }
        else
        {
            return;
        }
    }
}
