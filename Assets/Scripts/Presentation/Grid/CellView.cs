using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using TestTaskMike.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace TestTaskMike.Presentation
{
    public class CellView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        int x, y;

        [SerializeField]
        SpriteRenderer sprite;
        [SerializeField]
        SpriteRenderer buildingSprite;
        [SerializeField]
        SpriteRenderer buildingLvlSprite;

        [SerializeField]
        Sprite house, farm, mine;
        [SerializeField]
        Sprite lvl1, lvl2;

        [Inject]
        private IAsyncPublisher<CellClicked> _cellClickPublisher;

        [Inject]
        private IAsyncSubscriber<BuildingSet> _buildingSetSubscriber;

        [Inject]
        private ConstractionService _constractionService;

        private IDisposable _buildingSetSubscription;

        public void Initialize(Vector2Int posInGrid)
        {
            this.x = posInGrid.x;
            this.y = posInGrid.y;

            if (_buildingSetSubscriber != null)
            {
                _buildingSetSubscription = _buildingSetSubscriber.Subscribe((msg, ct) =>
                {
                    OnBuildingSet(msg);
                    return UniTask.CompletedTask;
                });
            }
            else
            {
                Debug.LogWarning("ConstractionService: cellClickedSubscriber is null.");
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            bool hasBuilding = _constractionService.HasBuilding(new Vector2Int(x, y));
            sprite.color = hasBuilding ? Color.red : Color.green;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            sprite.color = Color.white;
        }

        async void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {

            await _cellClickPublisher.PublishAsync(new CellClicked(x, y), CancellationToken.None);
        }

        private void OnBuildingSet(BuildingSet msg)
        {
            if (msg.x != x || msg.y != y)
                return;

            buildingLvlSprite.sprite =
             msg.Level switch
             {
                 1 => lvl1,
                 2 => lvl2,
                 _ => null
             };

            buildingSprite.sprite =
             msg.BuildingType switch
             {
                 BuildingTypeEnum.Residential => house,
                 BuildingTypeEnum.Commercial => farm,
                 BuildingTypeEnum.Industrial => mine,
                 _ => null
             };
        }

        public void Dispose()
        {
            _buildingSetSubscription?.Dispose();
        }
    }
}