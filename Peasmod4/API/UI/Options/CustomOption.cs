using System;
using System.Collections.Generic;
using Peasmod4.API.Networking;
using Peasmod4.API.Roles;
using Reactor.Localization.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace Peasmod4.API.UI.Options;

public class CustomOption
{
    public static List<CustomOption> AllOptions = new();

    public static int num = 1;
    public readonly int ID;
    public readonly MultiMenu Menu;

    public CustomRole CustomRole;
    public CustomRoleOptionType CustomRoleOptionType;

    public Func<object, string> Format;
    public string Name;
    public string GetName()
    {
        return Language.GetString(Name);
    }

    public StringNames StringName;

    protected internal CustomOption(int id, MultiMenu menu, string name, CustomOptionType type,
        object defaultValue,
        Func<object, string> format = null, CustomRoleOptionType customRoleOptionType = CustomRoleOptionType.None,
        CustomRole customRole = null)
    {
        ID = id;
        Menu = menu;
        Name = name;
        Type = type;
        DefaultValue = ValueObject = defaultValue;
        Format = format ?? (obj => $"{obj}");
        CustomRole = customRole;
        CustomRoleOptionType = customRoleOptionType;

        if (Type == CustomOptionType.Button) return;
        AllOptions.Add(this);
        Set(ValueObject);

        StringName = CustomStringName.CreateAndRegister((customRoleOptionType == CustomRoleOptionType.Chance || customRoleOptionType == CustomRoleOptionType.Count) ?
            Utility.ColorString(customRole.Color, customRole.Name) + $" {GetName()}" : GetName());
    }

    protected internal object ValueObject { get; set; }
    protected internal OptionBehaviour Setting { get; set; }
    protected internal CustomOptionType Type { get; set; }
    public object DefaultValue { get; set; }
    internal static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
    internal static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";

    public override string ToString()
    {
        return Format(ValueObject);
    }

    public virtual void OptionCreated()
    {
        Setting.name = Setting.gameObject.name = GetName();
    }

    protected internal void Set(object value, bool SendRpc = true, bool Notify = false)
    {
        //System.Console.WriteLine($"{Name} set to {value}");

        ValueObject = value;

        if (Setting != null && AmongUsClient.Instance.AmHost && SendRpc)
            Coroutines.Start(RpcUpdateSetting.SendRpc(this));

        try
        {
            if (Setting is ToggleOption toggle)
            {
                var newValue = (bool)ValueObject;
                toggle.oldValue = newValue;
                if (toggle.CheckMark != null) toggle.CheckMark.enabled = newValue;
            }
            else if (Setting is NumberOption number)
            {
                var newValue = (float)ValueObject;

                number.Value = number.oldValue = newValue;
                number.ValueText.text = ToString();
            }
            else if (Setting is StringOption str)
            {
                var newValue = (int)ValueObject;

                str.Value = str.oldValue = newValue;
                str.ValueText.text = ToString();
            }
        }
        catch
        {
        }

        if (HudManager.InstanceExists && Type != CustomOptionType.Header && Notify)
            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringName, ToString(),
                HudManager.Instance.Notifier.lastMessageKey != (int)StringName);
    }
}