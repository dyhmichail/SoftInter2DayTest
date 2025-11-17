using UnityEngine;
using VContainer;

public class BuildingPanel : MonoBehaviour
{
    BuildingTypeEnum _choosedBuildingType;

    [Inject]
    private ConstractionService _constractionService;

    public void ChooseHouseBuild()
    {
        _choosedBuildingType = BuildingTypeEnum.Residential;
        _constractionService.ChoosedBuildingType = _choosedBuildingType;
    }

    public void ChooseFarmBuild()
    {
        _choosedBuildingType = BuildingTypeEnum.Commercial;
        _constractionService.ChoosedBuildingType = _choosedBuildingType;
    }

    public void ChooseIndustrialBuild()
    {
        _choosedBuildingType = BuildingTypeEnum.Industrial;
        _constractionService.ChoosedBuildingType = _choosedBuildingType;
    }
}
