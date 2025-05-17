using Peasmod4.API.Roles;
using System;

namespace Peasmod4.API.UI.Options;

public class CustomRoleOption : CustomOption
{
    internal CustomNumberOption chanceOption;
    internal CustomNumberOption countOption;

    protected internal CustomRoleOption(CustomRole customRole) : base(num++, GetMultiMenu(customRole), "roleOption", ColorString(customRole.Color, customRole.Name), CustomOptionType.Header, 0)
    {
        chanceOption = new CustomNumberOption(GetMultiMenu(customRole), $"{customRole.Name}ChanceOption", "Spawn Chance", 0, 10, new FloatRange(0, 100), PercentFormat, CustomRoleOptionType.Chance, customRole);
        countOption = new CustomNumberOption(GetMultiMenu(customRole), $"{customRole.Name}CountOption", "Spawn Count", 1, 1, new FloatRange(1, 15), null, CustomRoleOptionType.Count, customRole);
    }

    private static MultiMenu GetMultiMenu(CustomRole customRole)
    {
        switch (customRole.Team)
        {
            case Enums.Team.Role:
                return MultiMenu.Neutral;
            case Enums.Team.Alone:
                return MultiMenu.Neutral;
            case Enums.Team.Crewmate:
                return MultiMenu.Crewmate;
            case Enums.Team.Impostor:
                return MultiMenu.Impostor;
            default:
                return MultiMenu.Main;
        }
    }

    public Func<object, string> PercentFormat { get; } = value => $"{value:0}%";

    protected internal int GetChance()
    {
        return (int)chanceOption.Value;
    }
    protected internal int GetCount()
    {
        return (int)countOption.Value;
    }
}