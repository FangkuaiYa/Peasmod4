using System;
using System.Reflection;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Buttons;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using Peasmod4.Roles.Abilities;
using UnityEngine;

namespace Peasmod4.Roles.Impostor;

#if !API
[RegisterCustomRole]
#endif
public class Glaciator : CustomRole
{
    public CustomNumberOption CooldownOption;
    public CustomNumberOption DurationOption;
    public CustomButton FreezeButton;
    public CustomRoleOption RoleOption;

    public Glaciator(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        CooldownOption = new CustomNumberOption(MultiMenu.Impostor, "Glaciator.FreezeCooldown", 20f, 2.5f,
            new FloatRange(10f, 100f), CustomOption.CooldownFormat);
        DurationOption = new CustomNumberOption(MultiMenu.Impostor, "Glaciator.FreezeDuration", 5f, 2.5f,
            new FloatRange(5f, 50f), CustomOption.CooldownFormat);

        GameEventManager.GameStartEventHandler += OnGameStart;
    }

    public override string Name => "role.Glaciator.name".Translate();
    public override string Description => "role.Glaciator.Description".Translate();
    public override string LongDescription => "role.Glaciator.LongDescription".Translate();
    public override string TaskHint => "role.Glaciator.TaskHint".Translate();

    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => Enums.Visibility.Impostor;
    public override Enums.Team Team => Enums.Team.Impostor;
    public override bool HasToDoTasks => false;

    public void OnGameStart(object sender, EventArgs args)
    {
        FreezeButton = new CustomButton("Glaciator.FreezeButton",
            () => StopMovement.RpcToggleMovement(PlayerControl.LocalPlayer), "role.Glaciator.buttonText",
            ResourceManager.PlaceholderButton, p => !p.Data.IsDead && p.IsCustomRole(this), _ => true,
            new CustomButton.CustomButtonOptions(CooldownOption.Value, true, DurationOption.Value,
                () => StopMovement.RpcToggleMovement(PlayerControl.LocalPlayer)));
    }
}