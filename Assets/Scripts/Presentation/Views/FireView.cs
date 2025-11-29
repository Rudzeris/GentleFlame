using Assets.Scripts.Presentation.ViewModels;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Presentation.Views
{
    public class FireView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fuelText;
        [SerializeField] private TextMeshProUGUI _tempText;

        [Header("Buttons")]
        [SerializeField] private Button _addFuel; 

        [Inject]
        public void Construct(FireViewModel viewModel)
        {
            viewModel.FuelAmount.Subscribe(value => _fuelText.text = value.ToString());
            viewModel.Temperature.Subscribe(value => _tempText.text = value.ToString("F1"));
            _addFuel.onClick.AddListener(() =>
            {
                var fuels = Random.Range(1, 3);
                var temperature = Random.Range(20, 100);
                Debug.Log($"Add fuels: {fuels}, temperature: {temperature}");
                viewModel.AddFuel(fuels, temperature);
            });
        }
    }
}
