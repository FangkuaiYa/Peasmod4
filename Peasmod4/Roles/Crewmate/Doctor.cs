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

namespace Peasmod4.Roles.Crewmate;

#if !API
[RegisterCustomRole]
#endif
public class Doctor : CustomRole
{
    public CustomButton ReviveButton;

    public CustomNumberOption ReviveCooldown;
    public CustomRoleOption RoleOption;

    public Doctor(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        ReviveCooldown = new CustomNumberOption(MultiMenu.Crewmate, "Doctor.ReviveCooldown", 20f, 5f,
            new FloatRange(0f, 120f), CustomOption.CooldownFormat);

        GameEventManager.GameStartEventHandler += OnGameStart;
    }

    public override string Name => "role.Doctor.name".Translate();
    public override string Description => "role.Doctor.Description".Translate();
    public override string LongDescription => "role.Doctor.LongDescription".Translate();
    public override string TaskHint => "role.Doctor.TaskHint".Translate();
    public override Color Color => Color.green;
    public override Enums.Visibility Visibility => Enums.Visibility.NoOne;
    public override Enums.Team Team => Enums.Team.Crewmate;
    public override bool HasToDoTasks => true;

    public void OnGameStart(object sender, EventArgs args)
    {
        ReviveButton = new CustomButton("DoctorRevive", () =>
        {
            PeasmodPlugin.Logger.LogInfo(ReviveButton.ObjectTarget.name);
            var body = ReviveButton.ObjectTarget.GetComponent<DeadBody>();
            var player = body.ParentId.GetPlayer();
            player.RpcRevive();
            player.NetTransform.RpcSnapTo(body.TruePosition);
            //ReviveButton.ObjectTarget.Destroy();
        }, "role.Doctor.buttonText", ResourceManager.RevivePlayerButton, player => player.IsCustomRole(this) && !player.Data
            .IsDead, _ => true, new CustomButton.CustomButtonOptions(
            targetType: CustomButton.CustomButtonOptions.TargetType.Object, objectTargetSelector:
            () => PlayerControl.LocalPlayer.FindNearestObject(obj => obj.GetComponent<DeadBody>(), 1f),
            targetOutline: Color, maxCooldown: ReviveCooldown.Value));
    }
}