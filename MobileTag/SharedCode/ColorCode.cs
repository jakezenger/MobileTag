using Android.Graphics;

public static class ColorCode
{
    public static Color RED = Color.Argb(180, 255, 0, 0);
    public static Color GREEN = Color.Argb(120, 0, 255, 128);
    public static Color BLUE = Color.Argb(120, 0, 0, 255);
    public static Color PURPLE = Color.Argb(120, 128, 0, 255);
    public static Color PINK = Color.Argb(120, 255, 0, 255);

    public static Color TeamColor(int teamID)
    {
        Color rgb;

        switch (teamID)
        {
            case 1: rgb = RED; break;
            case 2: rgb = GREEN; break;
            case 3: rgb = BLUE; break;
            case 4: rgb = PURPLE; break;
            case 5: rgb = PINK; break;
            default: rgb = Color.Argb(100, 255, 255, 255); break;
        }

        return rgb;
    }

    public static string TeamName(int teamID)
    {
        switch(teamID)
        {
            case 1: return "Red";
            case 2: return "Green";
            case 3: return "Blue";
            case 4: return "Purple";
            case 5: return "Pink";
            default: return "Unknown";                
        }
    }
}