using LabApi.Events;

namespace XazeAPI.API.Events;

public static class XazeEvents 
{
    public static event LabEventHandler<PlayerHearingFakePlayer> HearingFake;
    public static void OnPlayerHearingFake(PlayerHearingFakePlayer hearingFakePlayer)
    {
        HearingFake.InvokeEvent<PlayerHearingFakePlayer>(hearingFakePlayer);
    }
}