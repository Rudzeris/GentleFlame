using Assets.Scripts.Signals;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Handlers
{
    public class FireDiedHandler : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer fireRenderer;
        [SerializeField] private ParticleSystem fireParticle;
        [SerializeField] private ParticleSystem smokeParticle;
        [SerializeField] private Color deadFireColor = Color.gray;
        private Color originalColor;

        private SignalBus _bus;

        [Inject]
        public void Construct(SignalBus bus)
        {
            _bus = bus;
        }

        private void Start()
        {
            originalColor = fireRenderer.color;
            _bus.Subscribe<FireDiedSignal>(OnDead);
        }

        private void OnDestroy()
        {
            _bus.Unsubscribe<FireDiedSignal>(OnDead);
        }

        private void OnDead()
        {
            fireRenderer.color = deadFireColor;
            fireParticle.Stop(true,ParticleSystemStopBehavior.StopEmitting);

            FadeOutSmoke(2f).Forget();
        }

        private async UniTaskVoid FadeOutSmoke(float duration)
        {
            var emissin = smokeParticle.emission;
            float startRate = emissin.rateOverTime.constant;
            float time = 0f;

            while(time < duration)
            {
                time += Time.deltaTime;
                float newRate = Mathf.Lerp(startRate, 0f, time / duration);
                emissin.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);
                await UniTask.Yield();
            }

            emissin.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
        }
    }
}
