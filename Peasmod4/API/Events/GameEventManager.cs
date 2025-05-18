using System;

namespace Peasmod4.API.Events;

public class GameEventManager
{
    public static EventHandler GameStartEventHandler;

    public static EventHandler<GameEndEventArgs> GameEndEventHandler;

    public static EventHandler<GameJoinedEventArgs> GameJoinedEventHandler;

    public class GameEndEventArgs : EventArgs
    {
        public GameOverReason Reason;

        public GameEndEventArgs(GameOverReason reason)
        {
            Reason = reason;
        }
    }

    public class GameJoinedEventArgs : EventArgs
    {
        public string LobbyCode;

        public GameJoinedEventArgs(string lobbyCode)
        {
            LobbyCode = lobbyCode;
        }
    }
}