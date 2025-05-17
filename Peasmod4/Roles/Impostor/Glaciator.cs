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
    public Glaciator(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        CooldownOption = new CustomNumberOption(MultiMenu.Impostor, "Glaciator.FreezeCooldown", "Freeze cooldown", 20f, 2.5f, new FloatRange(10f, 100f), CustomOption.CooldownFormat);
        DurationOption = new CustomNumberOption(MultiMenu.Impostor, "Glaciator.FreezeDuration", "Freeze duration", 5f, 2.5f, new FloatRange(5f, 50f), CustomOption.CooldownFormat);

        PeasmodPlugin.Logger.LogError("FUCK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        GameEventManager.GameStartEventHandler += OnGameStart;
    }

    public override string Name => "Glaciator";
    public override string Description => "Freeze everyone";
    public override string LongDescription => "";
    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => Enums.Visibility.Impostor;
    public override Enums.Team Team => Enums.Team.Impostor;
    public override bool HasToDoTasks => false;

    public CustomNumberOption CooldownOption;
    public CustomNumberOption DurationOption;
    public CustomRoleOption RoleOption;
    public CustomButton FreezeButton;

    public void OnGameStart(object sender, EventArgs args)
    {
        FreezeButton = new CustomButton("Glaciator.FreezeButton", () => StopMovement.RpcToggleMovement(PlayerControl.LocalPlayer), "Freeze",
            ResourceManager.PlaceholderButton, p => !p.Data.IsDead && p.IsCustomRole(this), _ => true,
            new CustomButton.CustomButtonOptions(CooldownOption.Value, true, DurationOption.Value, () => StopMovement.RpcToggleMovement(PlayerControl.LocalPlayer)));
    }
}