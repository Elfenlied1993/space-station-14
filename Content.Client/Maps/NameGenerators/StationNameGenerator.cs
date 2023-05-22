namespace Content.Client.Maps.NameGenerators;

[ImplicitDataDefinitionForInheritors]
public abstract class StationNameGenerator
{
    public abstract string FormatName(string input);
}
