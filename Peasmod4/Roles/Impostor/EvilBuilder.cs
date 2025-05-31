﻿using System;
using System.Linq;
using System.Reflection;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Buttons;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using Peasmod4.Roles.Abilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Peasmod4.Roles.Impostor;

#if !API
[RegisterCustomRole]
#endif
public class EvilBuilder : CustomRole
{
    public CustomButton BuildButton;

    public int LeftUses;
    public CustomRoleOption RoleOption;
    public CustomNumberOption VentBuildAmount;

    public EvilBuilder(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        VentBuildAmount = new CustomNumberOption(MultiMenu.Impostor, "EvilBuilder.VentBuildAmount", 0, 1f,
            new FloatRange(0f, 100f));

        GameEventManager.GameStartEventHandler += OnGameStart;
    }

    public override string Name => "role.EvilBuilder.name".Translate();
    public override string Description => "role.EvilBuilder.Description".Translate();
    public override string LongDescription => "role.EvilBuilder.LongDescription".Translate();
    public override string TaskHint => "role.EvilBuilder.TaskHint".Translate();
    public override Color Color => Palette.ImpostorRed;
    public override Enums.Visibility Visibility => Enums.Visibility.Impostor;
    public override Enums.Team Team => Enums.Team.Impostor;
    public override bool HasToDoTasks => false;

    public void OnGameStart(object sender, EventArgs args)
    {
        LeftUses = VentBuildAmount.Value == 0 ? -1 : (int)VentBuildAmount.Value;
        BuildButton = new CustomButton("BuildButton", () =>
            {
                LeftUses--;
                var pos = PlayerControl.LocalPlayer.transform.position;
                BuildVent.RpcCreateVent(PlayerControl.LocalPlayer, pos.x, pos.y, pos.z);
            }, "role.EvilBuilder.buttonText", ResourceManager.BuildVentButton, player => player.IsCustomRole(this) && !player.Data.IsDead,
            player =>
            {
                if (LeftUses == 0)
                    return false;

                if (Object.FindObjectOfType<Vent>() == null)
                    return false;
                var vent = Object.FindObjectOfType<Vent>().gameObject;
                var ventSize = Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) *
                               0.75f;
                var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, ventSize, 0).Where(c =>
                        (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                    .ToArray();
                return hits.Length == 0;
            });
    }
}