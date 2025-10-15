using Assets.Scripts.Configs;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public class FireHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI burnTimeText;
        [SerializeField] private TextMeshProUGUI woodText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Slider fuelSlider;
        [SerializeField] private Button addFuelButton;

        [Header("Settings")]
        [SerializeField] private int woodPerClick = 1;

        private IFireService _fireService;
        private IFuelService _fuelService;
        private ICurrencyService _currencyService;
        [Inject]
        private FireConfig _fireConfig;

        [Inject]
        public void Construct(IFireService fireService, IFuelService resourceService, ICurrencyService currencyService)
        {
            _fireService = fireService;
            _fuelService = resourceService;
            _currencyService = currencyService;
            _fireService = fireService;

            fuelSlider.maxValue = _fireConfig.maxBurnTime;
        }

        private void Start()
        {
            _fireService.FireState.TotalBurnTime
                .Subscribe(time => burnTimeText.text = time.ToString())
                .AddTo(this);
            _fireService.FireState.TimeToExtinguish
                .Subscribe(level => fuelSlider.value = level)
                .AddTo(this);
            _fireService.FireState.IsAlive
                .Subscribe(alive =>
                {
                    addFuelButton.interactable = alive;
                })
                .AddTo(this);
            _fuelService.GetFuel(FuelType.Wood)
                .Subscribe(value => woodText.text =  value.ToString())
                .AddTo(this);
            _currencyService.GetCurrency(CurrencyType.Coin)
                .Subscribe(value => coinText.text = value.ToString());

            addFuelButton.onClick.AddListener(OnAddFuelClick);
        }
        private void OnAddFuelClick()
        {
            if (_fuelService.ConsumeFuel(FuelType.Wood, woodPerClick))
                _fireService.AddFuel(FuelType.Wood, woodPerClick);
            else
                Debug.Log("Нет дров");
        }
    }
}
