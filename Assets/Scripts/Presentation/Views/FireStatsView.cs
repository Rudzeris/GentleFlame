using Assets.Scripts.Core.Enums;
using Assets.Scripts.Presentation.ViewModels;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Presentation.Views
{
    public class FireStatsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fuelText;
        [SerializeField] private TextMeshProUGUI _tempText;

        [Header("Buttons")]
        [SerializeField] private Button _addFuel;

        [Header("Какое топливо кладёт кнопка (временно, до появления UI выбора)")]
        [SerializeField] private FuelType _fuelToAdd = FuelType.Twigs;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        private FireStatsViewModel _viewModel;

        [Inject]
        public void Construct(FireStatsViewModel viewModel)
        {
            _viewModel = viewModel;

            // AddTo: без отписки подписки переживали перезагрузку сцены и падали на мёртвых ссылках (T-12).
            _viewModel.FuelAmount.Subscribe(UpdateFuelText).AddTo(_subscriptions);
            _viewModel.MaxFuelCapacity.Subscribe(_ => UpdateFuelText(_viewModel.FuelAmount.Value)).AddTo(_subscriptions);
            _viewModel.Temperature.Subscribe(value => _tempText.text = $"{value:F1}°").AddTo(_subscriptions);

            _addFuel.onClick.AddListener(OnAddFuelClicked);
        }

        private void UpdateFuelText(int amount)
            => _fuelText.text = $"{amount} / {_viewModel.MaxFuelCapacity.Value}";

        private void OnAddFuelClicked()
            => _viewModel.AddFuelFromStorage(_fuelToAdd, 1);

        private void OnDestroy()
        {
            _addFuel.onClick.RemoveListener(OnAddFuelClicked);
            _subscriptions.Dispose();
        }
    }
}
