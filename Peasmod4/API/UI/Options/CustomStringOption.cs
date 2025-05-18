namespace Peasmod4.API.UI.Options;

public class CustomStringOption : CustomOption
{
    protected internal CustomStringOption(MultiMenu menu, string optionName, string name, string[] values,
        int startingId = 0) :
        base(num++, menu, optionName, name.Translate(), CustomOptionType.String, startingId)
    {
        Values = values;
        Format = value => Values[(int)value];
    }

    protected string[] Values { get; set; }
    protected internal int Value => (int)ValueObject;

    protected internal void Increase()
    {
        if (Value >= Values.Length - 1)
            Set(0);
        else
            Set(Value + 1);
    }

    protected internal void Decrease()
    {
        if (Value <= 0)
            Set(Values.Length - 1);
        else
            Set(Value - 1);
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.Value = str.oldValue = Value;
        str.ValueText.text = ToString();
    }
}