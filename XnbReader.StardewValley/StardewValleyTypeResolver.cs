namespace XnbReader.StardewValley;

public class StardewValleyTypeResolver: TypeResolver
{
    public override string SimplifyType(string type)
    {
        type = type.Replace("StardewValley.GameData", "XnbReader.StardewValley");
        return base.SimplifyType(type);
    }
}
