namespace Assets.Scripts.Core.Data
{
    public enum FireStage
    {
        Straw,
        Wood,
        Stone,
        Bonfire,
        Legendary,
    }
    public class ProgressionData
    {
        public int level;
        public int exp;
        public FireStage stage;
    }
}
