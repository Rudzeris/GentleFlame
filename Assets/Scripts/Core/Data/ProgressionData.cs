using Assets.Scripts.Core.Enums;
using System;

namespace Assets.Scripts.Core.Data
{
    [Serializable]
    public class ProgressionData
    {
        public int level = 1;
        public int exp;
        public FireStage stage = FireStage.Straw;
    }
}
