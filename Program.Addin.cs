using ImGuiNET;
using ClickableTransparentOverlay;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

partial class Program : Overlay
{
    static uint rgb(int r, int g, int b) => (uint) (((r << 24) | (g << 16) | (b << 8) | 255) & 0xffffffffL);
    Image<Rgba32> fone;

    nint fonehandle;

    public Program(int wd, int hg) : base(wd, hg)
    {
        ReplaceFont("Cousine-Regular.ttf", 28, FontGlyphRangeType.Cyrillic);

        Configuration configuration = Configuration.Default.Clone();
        configuration.PreferContiguousImageBuffers  = true;
        fone = new (configuration, wd, hg, Color.FromRgba(255,255,255,128));
    }

    public static void Main()
    {
        new Program(1920, 1080).Start().Wait();
    }

    public void ShowFone(bool withUpdate = false)
    {
        if (withUpdate)
            RemoveImage("fone");
        AddOrGetImagePointer("fone", fone, false, out fonehandle);
        ImGui.GetBackgroundDrawList().AddImage(fonehandle, new(0,0), new(fone.Width, fone.Height));
    }
}