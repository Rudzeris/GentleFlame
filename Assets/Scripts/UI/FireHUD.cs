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
        [SerializeField] private Slider fuelSlider;
        [SerializeField] private Button addFuelButton;

        [Header("Settings")]
        [SerializeField] private int woodPerClick = 1;
        [SerializeField] private float fuelGainPerWood = 0.3f;

        private IFireService _fireService;
        private IResourceService _resourceService;

        [Inject]
        public void Construct(IFireService fireService, IResourceService resourceService)
        {
            _fireService = fireService;
            _resourceService = resourceService;
        }

        private void Start()
        {
            _fireService.FireState.BurnTime
                .Subscribe(time => burnTimeText.text = time.ToString())
                .AddTo(this);
            _fireService.FireState.FuelLevel
                .Subscribe(level => fuelSlider.value = level)
                .AddTo(this);
            _fireService.FireState.IsAlive
                .Subscribe(alive =>
                {
                    addFuelButton.interactable = alive;
                })
                .AddTo(this);

            addFuelButton.onClick.AddListener(OnAddFuelClick);
        }
        private void OnAddFuelClick()
        {
            if (_resourceService.ConsumeWood(woodPerClick))
                _fireService.AddFuel(fuelGainPerWood);
            else
                Debug.Log("Нет дров");
        }
    }
}
