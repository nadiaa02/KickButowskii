using UnityEngine;

public enum BodyType
{
    Slim,
    Normal,
    Big
}

public enum HairType
{
    Short,
    Long,
    Fringe,
    Bald
}

public enum UpperClothesType
{
    Shirt,
    LongSleeve,
    Straps
}

public enum LowerClothesType
{
    Pants,
    Shorts,
    Skirt
}

public enum SkinPalette
{
    Palette1,
    Palette2,
    Palette3
}

public enum EyeColor
{
    Green,
    Blue,
    Brown
}

[System.Serializable]
public class CharacterSpec
{
    public SkinPalette skinPalette;
    public BodyType bodyType;
    public HairType hairType;
    public UpperClothesType upperClothesType;
    public LowerClothesType lowerClothesType;
    public EyeColor eyeColor;

    public Color skinColor;
    public Color hairColor;
    public Color clothesMainColor;
    public Color clothesSecondaryColor;
}
