using System;
using System.Reflection;
using AmongUs.GameOptions;
using Peasmod4.API;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Options;
using UnityEngine;

namespace Peasmod4.Roles.Neutral;

#if !API
[RegisterCustomRole]
#endif
public class Traitor : CustomRole
{
    public bool HasBetrayed;

    public CustomRoleOption RoleOption;

    public Traitor(Assembly assembly) : base(assembly)
    {
        GameEventManager.GameStartEventHandler += OnGameStart;
        PlayerEventManager.PlayerCompletedTaskEventHandler += OnTaskComplete;

        RoleOption = new CustomRoleOption(this);
    }

    public override string Name => "role.Traitor.name".Translate();
    public override string Description => "role.Traitor.Description".Translate();
    public override string LongDescription => "role.Traitor.LongDescription".Translate();
    public override string TaskHint => "role.Traitor.TaskHint".Translate();
    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => HasBetrayed ? Enums.Visibility.Impostor : Enums.Visibility.NoOne;
    public override Enums.Team Team => HasBetrayed ? Enums.Team.Impostor : Enums.Team.Alone;
    public override bool HasToDoTasks => true;

    public void OnGameStart(object sender, EventArgs args)
    {
        HasBetrayed = false;
    }

    public void OnTaskComplete(object sender, PlayerEventManager.PlayerCompletedTaskEventArgs args)
    {
        if (args.Player.IsLocal() && args.Player.IsCustomRole(this) && args.Player.AllTasksCompleted())
        {
            args.Player.RpcSetVanillaRole(RoleTypes.Impostor);
            HasBetrayed = true;
        }
    }
}