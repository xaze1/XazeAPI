namespace XazeAPI.API.Interfaces
{
    public interface ICustomGlow
    {
        public abstract LightSystem.LightConfig.LightState State { get; }
        public abstract UnityEngine.Color[] Colors { get; }
        public abstract float Intensity { get; }
        public abstract float Range { get; }
    }
}
