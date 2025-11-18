using System;
using TestTaskMike.Application;
using TMPro;
using UnityEngine;
using VContainer;
using R3;
using VContainer.Unity;

namespace TestTaskMike.Presentation
{
    public class GoldView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI goldValueText;

        private EconomyService _economyService;
        private System.IDisposable _goldSubscription;

        [Inject]
        public void Construct(EconomyService economyService)
        {
            _economyService = economyService;
            Initialize();
        }

        public void Initialize()
        {
            _goldSubscription = _economyService.Gold.Subscribe(gold =>
            {
                UpdateGoldDisplay(gold);
            });
        }

        private void UpdateGoldDisplay(int goldAmount)
        {
            if (goldValueText != null)
            {
                goldValueText.text = $"{goldAmount}";
            }
        }

        public void Dispose()
        {
            _goldSubscription?.Dispose();
        }
    }
}