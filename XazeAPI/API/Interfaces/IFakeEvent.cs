using JetBrains.Annotations;
using XazeAPI.API.AudioCore.FakePlayers;

namespace XazeAPI.API.Interfaces;

public interface IFakeEvent
{
    CustomAudioPlayer? AudioPlayer { get; }
    ReferenceHub? FakePlayer { get; }
}