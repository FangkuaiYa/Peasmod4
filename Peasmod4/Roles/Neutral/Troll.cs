using System;
using System.Collections.Generic;
using System.Reflection;
using Peasmod4.API;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.EndGame;
using Peasmod4.API.UI.Options;
using UnityEngine;

namespace Peasmod4.Roles.Neutral;

#if !API
[RegisterCustomRole]
#endif
public class Troll : CustomRole
{
    public static Dictionary<byte, CustomEndGameManager.CustomEndReason> EndReasons = new();

    public CustomRoleOption RoleOption;

    public Troll(Assembly assembly) : base(assembly)
    {
        GameEventManager.GameStartEventHandler += OnGameStart;
        PlayerEventManager.CanPlayerBeMurderedEventHandler += CanPlayerBeMurdered;

        RoleOption = new CustomRoleOption(this);
    }

    public override string Name => "role.Troll.name".Translate();
    public override string Description => "role.Troll.Description".Translate();
    public override string LongDescription => "role.Troll.LongDescription".Translate();
    public override string TaskHint => "role.Troll.TaskHint".Translate();
    public override Color Color => Palette.AcceptedGreen;
    public override Enums.Visibility Visibility => Enums.Visibility.NoOne;
    public override Enums.Team Team => Enums.Team.Alone;
    public override bool HasToDoTasks => false;

    public override bool DidWin(GameOverReason gameOverReason, PlayerControl player, ref bool overrides)
    {
        return EndReasons.ContainsKey(player.PlayerId) && EndReasons[player.PlayerId].EndReason == gameOverReason;
    }

    public void OnGameStart(object sender, EventArgs args)
    {
        EndReasons.Clear();
        foreach (var player in PlayerControl.AllPlayerControls)
            if (player.IsCustomRole(this))
            {
                PeasmodPlugin.Logger.LogInfo("Registered Reason: " + player.name);
                EndReasons.Add(player.PlayerId,
                    CustomEndGameManager.RegisterCustomEndReason("role.Troll.winText", Color, false, false));
            }
    }

    public void CanPlayerBeMurdered(object sender, PlayerEventManager.CanPlayerBeMurderedEventArgs args)
    {
        if (args.Victim.IsLocal() && args.Victim.IsCustomRole(this))
        {
            EndReasons[args.Victim.PlayerId].Trigger();
            args.Cancel = true;
        }
    }
}