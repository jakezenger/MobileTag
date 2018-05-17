using Android.Graphics;

public static class ColorCode
{
    public static Color RED = Color.Argb(120, 255, 0, 0);
    public static Color GREEN = Color.Argb(120, 0, 255, 128);
    public static Color BLUE = Color.Argb(120, 0, 0, 255);
    public static Color ORANGE = Color.Argb(120, 255, 119, 0);
    public static Color PINK = Color.Argb(120, 255, 0, 255);

    public static Color TeamColor(int teamID)
    {
        Color rgb;

        switch (teamID)
        {
            case 1: rgb = RED; break;
            case 2: rgb = GREEN; break;
            case 3: rgb = BLUE; break;
            case 4: rgb = ORANGE; break;
            case 5: rgb = PINK; break;
            default: rgb = Color.Argb(100, 255, 255, 255); break;
        }

        return rgb;
    }
}