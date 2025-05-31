namespace Peasmod4.API.UI.Options;

public class CustomToggleOption : CustomOption
{
    protected internal CustomToggleOption(MultiMenu menu, string optionName, bool value = true) : base(
        num++, menu, optionName, CustomOptionType.Toggle, value)
    {
        Format = val => (bool)val ? Language.GetString("option.optionOn.text") : Language.GetString("option.optionOff.text");
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