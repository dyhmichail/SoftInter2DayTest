namespace TestTaskMike.Domain
{
    public class ResidentialBuildingType : BuildingType
    {
        public override BuildingTypeEnum BuildingTypeKey => BuildingTypeEnum.Residential;

        public override int cost { get; protected set; }
        public override int maxUpgradeLevel { get; protected set; }
        public override int goldIncomePerLevel { get; protected set; }

        public ResidentialBuildingType(int cost = 10, int maxUpgradeLevel = 2, int goldIncomePerLevel = 1)
        {
            this.cost = cost;
            this.maxUpgradeLevel = maxUpgradeLevel;
            this.goldIncomePerLevel = goldIncomePerLevel;
        }
    }
}
