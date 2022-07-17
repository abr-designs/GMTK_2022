namespace DefaultNamespace
{
    public static class GameStateManager
    {
        public static int CarryOverModifier { get; set; }
        public static int TotalKills { get; set; }
        public static int TotalCoins { get; set; }
        public static float TotalTime { get; set; }

        public static void Reset()
        {
            CarryOverModifier = 0;
            TotalKills = 0;
            TotalCoins = 0;
            TotalTime = 0f;
        }
    }
}