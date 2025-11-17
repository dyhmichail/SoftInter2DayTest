public enum BuildingTypeEnum
{
    None,
    Residential,
    Commercial,
    Industrial
}

public abstract class BuildingType
{
    public abstract BuildingTypeEnum BuildingTypeKey { get; }

    public abstract int cost { get; protected set; }
    public abstract int maxUpgradeLevel { get; protected set; }
    public abstract int goldIncomePerLevel { get; protected set; }
}
