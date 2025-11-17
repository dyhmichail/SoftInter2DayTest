public class IndustrialBuildingType : BuildingType
{
    public override BuildingTypeEnum BuildingTypeKey => BuildingTypeEnum.Industrial;

    public override int cost { get; protected set; }
    public override int maxUpgradeLevel { get; protected set; }
    public override int goldIncomePerLevel { get; protected set; }

    public IndustrialBuildingType(int cost = 150, int maxUpgradeLevel = 2, int goldIncomePerLevel = 3)
    {
        this.cost = cost;
        this.maxUpgradeLevel = maxUpgradeLevel;
        this.goldIncomePerLevel = goldIncomePerLevel;
    }
}
