using System;
using System.Reflection;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Buttons;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using Peasmod4.Roles.Abilities;
using Reactor.Networking.Rpc;
using UnityEngine;

namespace Peasmod4.Roles.Impostor;

#if !API
[RegisterCustomRole]
#endif
public class Ninja : CustomRole
{
    public CustomNumberOption HideCooldown;
    public CustomNumberOption HideDuration;
    public CustomRoleOption RoleOption;
    public CustomButton TurnInvisibleButton;

    public Ninja(Assembly assembly) : base(assembly)
    {
        GameEventManager.GameStartEventHandler += OnGameStart;

        RoleOption = new CustomRoleOption(this);
        HideCooldown = new CustomNumberOption(MultiMenu.Impostor, "Ninja.HideCooldown", 20f, 2.5f,
            new FloatRange(0, 100), CustomOption.CooldownFormat);
        HideDuration = new CustomNumberOption(MultiMenu.Impostor, "Ninja.HideDuration", 5f, 2.5f,
            new FloatRange(0, 50), CustomOption.CooldownFormat);
    }

    public override string Name => "role.Ninja.name".Translate();
    public override Sprite Icon => ResourceManager.TurnInvisibleButton;
    public override string Description => "role.Ninja.Description".Translate();
    public override string LongDescription => "role.Ninja.LongDescription".Translate();
    public override string TaskHint => "role.Ninja.TaskHint".Translate();
    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => Enums.Visibility.Impostor;
    public override Enums.Team Team => Enums.Team.Impostor;
    public override bool HasToDoTasks => false;

    public void OnGameStart(object sender, EventArgs args)
    {
        TurnInvisibleButton = new CustomButton("NinjaHide",
            () =>
            {
                Rpc<TurnInvisible.RpcTurnInvisible>.Instance.Send(
                    new TurnInvisible.RpcTurnInvisible.Data(PlayerControl.LocalPlayer, true));
            }, "role.Ninja.buttonText", Utility.CreateSprite("Peasmod4.Resources.Buttons.TurnInvisible.png", 794f),
            player => player.IsCustomRole(this) && !player.Data.IsDead, _ => true, new CustomButton.CustomButtonOptions(
                HideCooldown.Value, true, HideDuration.Value,
                () =>
                {
                    Rpc<TurnInvisible.RpcTurnInvisible>.Instance.Send(
                        new TurnInvisible.RpcTurnInvisible.Data(PlayerControl.LocalPlayer, false));
                }));
    }
}