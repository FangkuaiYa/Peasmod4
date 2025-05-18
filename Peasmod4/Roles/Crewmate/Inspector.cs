using System.Reflection;
using Peasmod4.API.Components;
using Peasmod4.API.Events;
using Peasmod4.API.Roles;
using Peasmod4.API.UI.Options;
using Peasmod4.Roles.Abilities;
using UnityEngine;

namespace Peasmod4.Roles.Crewmate;

#if !API
[RegisterCustomRole]
#endif
public class Inspector : CustomRole
{
    private float _Timer;

    public CustomNumberOption FootstepVisibleTimeOption;
    public CustomRoleOption RoleOption;
    public CustomNumberOption TimeBetweenOption;

    public Inspector(Assembly assembly) : base(assembly)
    {
        RoleOption = new CustomRoleOption(this);
        FootstepVisibleTimeOption = new CustomNumberOption(MultiMenu.Crewmate, "Inspector.FootstepVisibleTime",
            "Time footsteps stay visible", 5f, 1f, new FloatRange(1f, 30f), CustomOption.CooldownFormat);
        TimeBetweenOption = new CustomNumberOption(MultiMenu.Crewmate, "Inspector.TimeBetween",
            "Time between each footstep", 1f, 0.1f, new FloatRange(0.2f, 2f), CustomOption.CooldownFormat);

        HudEventManager.HudUpdateEventHandler += OnUpdate;
    }

    public override string Name => "Inspector";
    public override string Description => "Follow the bad guys";
    public override string LongDescription => "";
    public override Color Color => new(58f / 255f, 255f / 255f, 127f / 255f);
    public override Enums.Visibility Visibility => Enums.Visibility.NoOne;
    public override Enums.Team Team => Enums.Team.Crewmate;
    public override bool HasToDoTasks => true;

    public void OnUpdate(object sender, HudEventManager.HudUpdateEventArgs args)
    {
        if (!PlayerControl.LocalPlayer.IsCustomRole(this))
            return;

        _Timer -= Time.deltaTime;
        if (_Timer <= 0f)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var newFootprint = new GameObject(player.name + "-Footprint").AddComponent<FootprintBehaviour>();
                var pos = player.transform.position;
                newFootprint.transform.position = new Vector3(pos.x, pos.y, pos.z + 0.05f);
                newFootprint.maxLifeTime = newFootprint.remainingLifeTime = FootstepVisibleTimeOption.Value;
                newFootprint.color = Palette.PlayerColors[player.cosmetics.bodyMatProperties.ColorId];
            }

            _Timer = TimeBetweenOption.Value;
        }
    }
}