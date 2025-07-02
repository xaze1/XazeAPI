namespace XazeAPI.API.Stats.Player
{
    public abstract class PlayerBaseStat
    {
        public ReferenceHub Hub { get; set; } 

        public PlayerBaseStat()
        {
            Hub = null;
        }
        
        public PlayerBaseStat(ReferenceHub hub)
        {
            Hub = hub;
        }
    }
}
