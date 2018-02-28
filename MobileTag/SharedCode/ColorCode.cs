using Android.Graphics;

public static class ColorCode
{
    public static uint RED = 0xFF0000B4;
    public static uint GREEN = 0x00FF00B4;
    public static uint BLUE = 0x0000FFB4;
    public static uint PURPLE = 0x9932CCB4;
    public static uint PINK = 0x78FF32B4;

    public static Color SetTeamColor(int teamID)
    {
        uint rgb;
        const uint mask = 0x000000FF;
        switch (teamID)
        {
            case 1: rgb = ColorCode.RED; break;
            case 2: rgb = ColorCode.GREEN; break;
            case 3: rgb = ColorCode.BLUE; break;
            case 4: rgb = ColorCode.PURPLE; break;
            case 5: rgb = ColorCode.PINK; break;
            default: rgb = 0xFFFF00B4; break;
        }
        byte r = (byte)((rgb >> 24) & mask);
        byte g = (byte)((rgb >> 16) & mask);
        byte b = (byte)((rgb >> 8) & mask);
        byte o = (byte)(rgb & mask);
        Android.Graphics.Color color = Android.Graphics.Color.Argb(o, r, g, b);
        return color;
    }
}