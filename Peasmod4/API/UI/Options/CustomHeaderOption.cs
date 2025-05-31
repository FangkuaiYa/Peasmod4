namespace Peasmod4.API.UI.Options;

public class CustomHeaderOption : CustomOption
{
    protected internal CustomHeaderOption(MultiMenu menu, string name) : base(num++, menu, name,
        CustomOptionType.Header, 0)
    {
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = GetName();
    }
}