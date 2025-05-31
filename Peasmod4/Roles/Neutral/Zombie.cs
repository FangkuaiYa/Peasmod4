using System.Reflection;
using HarmonyLib;
using Peasmod4.API;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Buttons;
using Peasmod4.API.UI.EndGame;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using UnityEngine;

namespace Peasmod4.Roles.Neutral;

[HarmonyPatch]
#if !API
[RegisterCustomRole]
#endif
public class Zombie : CustomRole
{
    public readonly Color ZombieColor = new(109 / 255f, 142 / 255f, 74 / 255f);
    public CustomEndGameManager.CustomEndReason EndReason;

    public CustomButton InfectButton;
    public CustomNumberOption InfectCooldown;
    public CustomNumberOption LightMod;
    public bool MadeWinCall;
    public CustomStringOption Reach;
    public CustomRoleOption RoleOption;
    public CustomNumberOption SpeedMod;
    public CustomToggleOption VisibleToEveryone;

    public Zombie(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        Reach = new CustomStringOption(MultiMenu.Neutral, "Zombie.Reach", new[] { "Zombie.Reach.Small", "Zombie.Reach.Medium", "Zombie.Reach.Long" });
        InfectCooldown = new CustomNumberOption(MultiMenu.Neutral, "Zombie.InfectCooldown", 20f, 2.5f,
            new FloatRange(10f, 100f), CustomOption.CooldownFormat);
        SpeedMod = new CustomNumberOption(MultiMenu.Neutral, "Zombie.SpeedMod", 1f, 0.25f,
            new FloatRange(0.25f, 1.5f), CustomOption.MultiplierFormat);
        LightMod = new CustomNumberOption(MultiMenu.Neutral, "Zombie.LightMod", 1f, 0.25f,
            new FloatRange(0.25f, 1.5f), CustomOption.MultiplierFormat);
        VisibleToEveryone =
            new CustomToggleOption(MultiMenu.Neutral, "Zombie.VisibleToEveryone", false);

        GameEventManager.GameStartEventHandler += (_, _) =>
            EndReason = CustomEndGameManager.RegisterCustomEndReason("role.Zombie.winText", ZombieColor, false,
                false);
        HudEventManager.HudUpdateEventHandler += OnUpdate;
    }

    public override string Name => "role.Zombie.name".Translate();
    public override string Description => "role.Zombie.Description".Translate();

    public override string LongDescription => "role.Zombie.LongDescription".Translate();

    public override string TaskHint => "role.Zombie.TaskHint".Translate();
    public override Color Color => ZombieColor;

    public override Enums.Visibility Visibility =>
        VisibleToEveryone.Value ? Enums.Visibility.Everyone : Enums.Visibility.Role;

    public override Enums.Team Team => Enums.Team.Role;
    public override bool HasToDoTasks => false;

    public override void OnRoleAssigned(PlayerControl player)
    {
        if (!player.IsLocal())
            return;

        MadeWinCall = false;

        InfectButton = new CustomButton("InfectButton", () =>
            {
                if (InfectButton.PlayerTarget.Data.Role.IsImpostor)
                    return;
                InfectButton.PlayerTarget.RpcSetCustomRole(this);

                if (PlayerControl.AllPlayerControls.WrapToSystem().FindAll(player => player != null &&
                        !player.IsCustomRole(this) && !player.Data.IsDead && !player.Data.Role.IsImpostor).Count == 0)
                {
                    EndReason.Trigger();
                    MadeWinCall = true;
                }
            }, "role.Troll.buttonText",
            ResourceManager.PlaceholderButton, player => player.IsCustomRole(this),
            player => player.IsCustomRole(this), new CustomButton.CustomButtonOptions(InfectCooldown.Value,
                targetType: CustomButton.CustomButtonOptions.TargetType.Player,
                playerTargetSelector: () =>
                    PlayerControl.LocalPlayer.FindNearestPlayer(
                        player => !player.IsCustomRole(this) && !player.Data.IsDead, Reach.Value + 1),
                targetOutline: ZombieColor));

        if (PlayerControl.LocalPlayer.IsCustomRole(this)) PlayerControl.LocalPlayer.MyPhysics.Speed *= SpeedMod.Value;
    }

    public void OnUpdate(object sender, HudEventManager.HudUpdateEventArgs args)
    {
        if (AmongUsClient.Instance.AmHost && !MadeWinCall && AmongUsClient.Instance.IsGameStarted)
            if (PlayerControl.AllPlayerControls.WrapToSystem().FindAll(player => player != null &&
                    player.Data != null &&
                    !player.IsCustomRole(this) && !player.Data.IsDead && !player.Data.Role.IsImpostor).Count == 0 &&
                PlayerControl.AllPlayerControls.Count > 1)
            {
                EndReason.Trigger();
                MadeWinCall = true;
            }
    }

    public override bool DidWin(GameOverReason gameOverReason, PlayerControl player, ref bool overrides)
    {
        if (EndReason.EndReason == gameOverReason)
        {
            overrides = true;
            return player.IsCustomRole(this);
        }

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static void AdjustLightRadius(ShipStatus __instance, ref float __result)
    {
        if (PlayerControl.LocalPlayer.IsCustomRole<Zombie>())
            __result *= CustomRoleManager.GetRole<Zombie>().LightMod.Value;
    }
}