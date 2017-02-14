using System;

[Serializable]
public class Attribute
{
    public float value;

    public float modifier;

    public float coefficient;

    public float totalValue
    {
        get { return value*coefficient + modifier; }
    }
}
