namespace Assets.Scripts.Core.Data
{
    public enum FireMood
    {
        Happy,
        Sad,
        Sleep,
        Angry,
        Inspires,
    }
    public enum FireBright
    {
        Bright,
        Dim,
        AlmostOut,
        Extinguished,
    }
    public class FireStateData
    {
        public FireBright fireBright;
        public FireMood fireMood;
    }
}
