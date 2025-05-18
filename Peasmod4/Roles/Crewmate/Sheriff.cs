﻿using System.Reflection;
using Peasmod4.API;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Options;
using UnityEngine;

namespace Peasmod4.Roles.Crewmate;

#if !API
[RegisterCustomRole]
#endif
public class Sheriff : CustomRole
{
    public CustomToggleOption CanKillNeutralsOption;
    public CustomToggleOption InnocentVictimDiesAsWellOption;

    public CustomRoleOption RoleOption;

    public Sheriff(Assembly assembly) : base(assembly)
    {
        PlayerEventManager.PlayerMurderedEventHandler += OnPlayerKilled;
        PlayerEventManager.CanPlayerBeMurderedEventHandler += CanPlayerBeKilled;

        RoleOption = new CustomRoleOption(this);
        CanKillNeutralsOption =
            new CustomToggleOption(MultiMenu.Crewmate, "SheriffCanKillNeutrals", "Can kill neutrals", false);
        InnocentVictimDiesAsWellOption = new CustomToggleOption(MultiMenu.Crewmate, "SheriffInnocentVictimDiesAsWell",
            "Innocent victim dies as well");
    }

    public override string Name => "Sheriff";
    public override string Description => "Execute the bad guys";
    public override string LongDescription => "";
    public override string TaskHint => Description;
    public override Color Color => new(255f / 255f, 114f / 255f, 0f / 255f);
    public override Enums.Visibility Visibility => Enums.Visibility.NoOne;
    public override Enums.Team Team => Enums.Team.Crewmate;
    public override bool HasToDoTasks => true;

    public override bool CanKill(PlayerControl victim = null)
    {
        return true;
    }

    public void OnPlayerKilled(object sender, PlayerEventManager.PlayerMurderedEventArgs args)
    {
        if (args.Killer.IsLocal() && args.Killer.IsCustomRole(this) && !args.Victim.IsLocal() &&
            args.Flags.HasFlag(MurderResultFlags.Succeeded))
            if (!(args.Victim.Data.Role.IsImpostor || (CanKillNeutralsOption.Value && args.Victim.IsCustomRole() &&
                                                       (args.Victim.GetCustomRole().Team == Enums.Team.Alone ||
                                                        args.Victim.GetCustomRole().Team == Enums.Team.Role))))
                args.Killer.RpcMurderPlayer(args.Killer, true);
    }

    public void CanPlayerBeKilled(object sender, PlayerEventManager.CanPlayerBeMurderedEventArgs args)
    {
        if (args.Killer.IsCustomRole(this) && args.Victim.PlayerId != args.Killer.PlayerId)
            if (!(args.Victim.Data.Role.IsImpostor || (CanKillNeutralsOption.Value && args.Victim.IsCustomRole() &&
                                                       (args.Victim.GetCustomRole().Team == Enums.Team.Alone ||
                                                        args.Victim.GetCustomRole().Team == Enums.Team.Role))))
                if (!InnocentVictimDiesAsWellOption.Value)
                {
                    args.Cancel = true;
                    args.Killer.RpcMurderPlayer(args.Killer, true);
                }
    }
}