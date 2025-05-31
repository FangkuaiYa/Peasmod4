using System;
using System.Reflection;
using Peasmod4.API;
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
public class Janitor : CustomRole
{
    public CustomToggleOption CanKillOption;
    public CustomButton CleanButton;
    public CustomRoleOption RoleOption;

    public Janitor(Assembly assembly) : base(assembly)
    {
        GameEventManager.GameStartEventHandler += OnGameStart;

        RoleOption = new CustomRoleOption(this);
        CanKillOption = new CustomToggleOption(MultiMenu.Impostor, "Janitor.CanKill", false);
    }

    public override string Name => "role.Janitor.name".Translate();
    public override string Description => "role.Janitor.Description".Translate();
    public override string LongDescription => "role.Janitor.LongDescription".Translate();
    public override string TaskHint => "role.Janitor.TaskHint".Translate();

    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => Enums.Visibility.Impostor;
    public override Enums.Team Team => Enums.Team.Impostor;
    public override bool HasToDoTasks => false;

    public override bool CanKill(PlayerControl victim = null)
    {
        return (CanKillOption.Value ||
                Utility.GetImpostors().FindAll(player => !player.IsCustomRole(this)).Count == 0) &&
               base.CanKill(victim);
    }

    public void OnGameStart(object sender, EventArgs args)
    {
        CleanButton = new CustomButton("Janitor.CleanButton",
            () => PlayerControl.LocalPlayer.RpcRemoveBody(CleanButton.ObjectTarget.GetComponent<DeadBody>().ParentId),
            "role.Janitor.buttonText", ResourceManager.PlaceholderButton,
            player => player.IsCustomRole(this) && !player.Data.IsDead, _ => true,
            new CustomButton.CustomButtonOptions(targetType: CustomButton.CustomButtonOptions.TargetType.Object,
                targetOutline: Palette.ImpostorRed,
                objectTargetSelector: () =>
                    PlayerControl.LocalPlayer.FindNearestObject(obj => obj.GetComponent<DeadBody>(), 1f)));
    }
}