using Peasmod4.API.Roles;
using System;
using UnityEngine;

namespace Peasmod4.API.UI.Options;

public class CustomNumberOption : CustomOption
{
    protected internal CustomNumberOption(MultiMenu multiMenu, string optionName, string name, float value, float increment, FloatRange floatRange,
        Func<object, string> format = null, CustomRoleOptionType customRoleOptionType = CustomRoleOptionType.None, CustomRole customRole = null)
        : base(num++, multiMenu, optionName, name, CustomOptionType.Number, value, format, customRoleOptionType, customRole)
    {
        Min = floatRange.min;
        Max = floatRange.max;
        Increment = increment;
        IntSafe = Min % 1 == 0 && Max % 1 == 0 && Increment % 1 == 0;
    }

    protected float Min { get; set; }
    protected float Max { get; set; }
    protected float Increment { get; set; }
    public bool IntSafe { get; private set; }

    protected internal float Value => (float)ValueObject;

    protected internal void Increase()
    {
        var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;

        if (Value + increment >
            Max + 0.001f) // the slight increase is because of the stupid float rounding errors in the Giant speed
            Set(Min);
        else
            Set(Value + increment);
    }

    protected internal void Decrease()
    {
        var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;

        if (Value - increment < Min - 0.001f) // added it here to in case I missed something else
            Set(Max);
        else
            Set(Value - increment);
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.ValidRange = new FloatRange(Min, Max);
        number.Increment = Increment;
        number.Value = number.oldValue = Value;
        number.ValueText.text = ToString();
    }
}