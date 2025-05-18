using System;

namespace Peasmod4.API.Events;

public class MeetingEventManager
{
    public static EventHandler<MeetingEndEventArgs> MeetingEndEventHandler;

    public class MeetingEndEventArgs : EventArgs
    {
        public NetworkedPlayerInfo ExiledPlayer;
        public bool Tie;

        public MeetingEndEventArgs(NetworkedPlayerInfo exiledPlayer, bool tie)
        {
            ExiledPlayer = exiledPlayer;
            Tie = tie;
        }
    }
}