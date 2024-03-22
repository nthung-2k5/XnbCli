namespace XnbReader;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class HasHeaderAttribute(bool required): Attribute;
