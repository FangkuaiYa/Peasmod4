using System;
using System.ComponentModel;

namespace Peasmod4.API.Events;

public class PlayerEventManager
{
    public static EventHandler<PlayerDiedEventArgs> PlayerDiedEventHandler;

    public static EventHandler<PlayerExiledEventArgs> PlayerExiledEventHandler;

    public static EventHandler<PlayerMurderedEventArgs> PlayerMurderedEventHandler;

    public static EventHandler<CanPlayerBeMurderedEventArgs> CanPlayerBeMurderedEventHandler;

    public static EventHandler<PlayerCompletedTaskEventArgs> PlayerCompletedTaskEventHandler;

    public class PlayerDiedEventArgs : EventArgs
    {
        public PlayerControl DeadPlayer;

        public PlayerDiedEventArgs(PlayerControl deadPlayer)
        {
            DeadPlayer = deadPlayer;
        }
    }

    public class PlayerExiledEventArgs : EventArgs
    {
        public PlayerControl ExiledPlayer;

        public PlayerExiledEventArgs(PlayerControl exiledPlayer)
        {
            ExiledPlayer = exiledPlayer;
        }
    }

    public class PlayerMurderedEventArgs : EventArgs
    {
        public MurderResultFlags Flags;
        public PlayerControl Killer;
        public PlayerControl Victim;

        public PlayerMurderedEventArgs(PlayerControl killer, PlayerControl victim, MurderResultFlags flags)
        {
            Killer = killer;
            Victim = victim;
            Flags = flags;
        }
    }

    public class CanPlayerBeMurderedEventArgs : CancelEventArgs
    {
        public PlayerControl Killer;
        public PlayerControl Victim;

        public CanPlayerBeMurderedEventArgs(PlayerControl killer, PlayerControl victim)
        {
            Killer = killer;
            Victim = victim;
        }
    }

    public class PlayerCompletedTaskEventArgs : EventArgs
    {
        public PlayerControl Player;
        public PlayerTask Task;

        public PlayerCompletedTaskEventArgs(PlayerControl player, PlayerTask task)
        {
            Player = player;
            Task = task;
        }
    }
}