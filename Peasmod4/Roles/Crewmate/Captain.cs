using System;
using System.Reflection;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Buttons;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using UnityEngine;

namespace Peasmod4.Roles.Crewmate;

#if !API
[RegisterCustomRole]
#endif
public class Captain : CustomRole
{
    public CustomButton CallButton;
    public CustomNumberOption CallCooldownOption;
    public CustomRoleOption RoleOption;

    public Captain(Assembly assembly) : base(assembly)
    {
        GameEventManager.GameStartEventHandler += OnStart;

        RoleOption = new CustomRoleOption(this);
        CallCooldownOption = new CustomNumberOption(MultiMenu.Crewmate, "Captain.CallCooldownOption", 10, 1,
            new FloatRange(10, 60), CustomOption.CooldownFormat);
    }

    public override string Name => "role.Captain.name".Translate();
    public override Sprite Icon => ResourceManager.CallMeetingButton;
    public override string Description => "role.Captain.Description".Translate();
    public override string LongDescription => "role.Captain.LongDescription".Translate();
    public override string TaskHint => "role.Captain.TaskHint".Translate();
    public override Color Color => Palette.LightBlue;
    public override Enums.Visibility Visibility => Enums.Visibility.NoOne;
    public override Enums.Team Team => Enums.Team.Crewmate;
    public override bool HasToDoTasks => true;

    public void OnStart(object sender, EventArgs args)
    {
        CallButton = new CustomButton("CaptainCall", () => { PlayerControl.LocalPlayer.CmdReportDeadBody(null); },
            "role.Captain.buttonText", Icon, player => player.IsCustomRole(this) && !player.Data.IsDead, _ => true,
            new CustomButton.CustomButtonOptions(CallCooldownOption.Value));
    }
}