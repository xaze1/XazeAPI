using System;

namespace XazeAPI.API.Events;

public static class XazeEvents
{
    public static event Action<PlayerHearingFakePlayer> HearingFake;
    public static void OnPlayerHearingFake(PlayerHearingFakePlayer hearingFakePlayer)
    {
        HearingFake?.Invoke(hearingFakePlayer);
    }
}