namespace Peasmod4.API.UI.Options;

public class CustomToggleOption : CustomOption
{
    protected internal CustomToggleOption(MultiMenu menu, string optionName, string name, bool value = true) : base(
        num++, menu, optionName, name, CustomOptionType.Toggle, value)
    {
        Format = val => (bool)val ? "On" : "Off";
    }

    protected internal bool Value => (bool)ValueObject;

    protected internal void Toggle()
    {
        Set(!Value);
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var tgl = Setting.Cast<ToggleOption>();
        tgl.CheckMark.enabled = Value;
    }
}