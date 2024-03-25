namespace XnbReader.StardewValley;

public record QuantityModifier(string Id, string Condition, QuantityModifier.ModificationType Modification, float Amount, List<float> RandomAmount)
{
    public enum ModificationType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Set,
    }
    
    public enum QuantityModifierMode
    {
        Stack,
        Minimum,
        Maximum,
    }
    
}
