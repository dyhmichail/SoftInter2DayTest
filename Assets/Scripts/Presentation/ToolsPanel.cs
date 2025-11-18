using TestTaskMike.Application;
using UnityEngine;
using VContainer;

public class ToolsPanel : MonoBehaviour
{
    [SerializeField]
    ToolType _choosedToolType;

    [Inject]
    private ToolsServiceLocator _toolsService;

    public void ChooseBuildMode()
    {
        _choosedToolType = ToolType.Constraction;
        _toolsService.selectedTool = _choosedToolType;
    }

    public void ChooseUpgradeMode()
    {
        _choosedToolType = ToolType.Upgrade;
        _toolsService.selectedTool = _choosedToolType;
    }

    public void ChooseDestroyMode()
    {
        _choosedToolType = ToolType.Remove;
        _toolsService.selectedTool = _choosedToolType;
    }
}
