using LabApi.Events.CustomHandlers;
using XazeAPI.API.Events;

namespace XazeAPI.API;

public abstract class XazeEventHandler : CustomEventsHandler
{
    public virtual void OnPlayerHearingFakePlayer(PlayerHearingFakePlayer hearingFakePlayer)
    {
    }
}