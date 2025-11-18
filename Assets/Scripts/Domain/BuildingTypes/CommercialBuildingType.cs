namespace TestTaskMike.Domain
{
    public class CommercialBuildingType : BuildingType
    {
        public override BuildingTypeEnum BuildingTypeKey => BuildingTypeEnum.Commercial;

        public override int cost { get; protected set; }
        public override int maxUpgradeLevel { get; protected set; }
        public override int goldIncomePerLevel { get; protected set; }

        public CommercialBuildingType(int cost = 50, int maxUpgradeLevel = 2, int goldIncomePerLevel = 2)
        {
            this.cost = cost;
            this.maxUpgradeLevel = maxUpgradeLevel;
            this.goldIncomePerLevel = goldIncomePerLevel;
        }
    }
}
