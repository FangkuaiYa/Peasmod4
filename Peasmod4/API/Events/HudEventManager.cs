using System;

namespace Peasmod4.API.Events;

public class HudEventManager
{
    public static EventHandler<HudUpdateEventArgs> HudUpdateEventHandler;

    public static EventHandler<HudSetActiveEventArgs> HudSetActiveEventHandler;

    public class HudUpdateEventArgs : EventArgs
    {
        public HudManager Hud;

        public HudUpdateEventArgs(HudManager hud)
        {
            Hud = hud;
        }
    }

    public class HudSetActiveEventArgs : EventArgs
    {
        public bool Active;
        public HudManager Hud;

        public HudSetActiveEventArgs(HudManager hud, bool active)
        {
            Hud = hud;
            Active = active;
        }
    }
}