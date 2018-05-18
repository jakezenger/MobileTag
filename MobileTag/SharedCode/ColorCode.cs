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

    public static string TeamName(int teamID)
    {
        switch(teamID)
        {
            case 1: return "Red";
            case 2: return "Green";
            case 3: return "Blue";
            case 4: return "Orange";
            case 5: return "Pink";
            default: return "Unknown";                
        }
    }

    public static Color BrightTeamColor(int teamID)
    {
        switch(teamID)
        {
            case 1: return Color.Argb(255,255, 80, 80);
            case 2: return Color.Argb(255,0, 204, 102);
            case 3: return Color.Argb(255,102, 153, 255);
            case 4: return Color.Argb(255, 255, 119, 0);
            case 5: return Color.Argb(255, 255, 0, 102);
            default: return Color.Argb(0, 0, 0, 0);
        }
    }

    public static string BrightHexColorCode(int teamID)
    {
        switch (teamID)
        {
            case 1: return "#ff5050";
            case 2: return "#00cc66";
            case 3: return "#6699ff";
            case 4: return "#ff7700";
            case 5: return "#ff0066";
            default: return "#000000";
        }
    }
}