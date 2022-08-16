using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OctanGames.UI
{
    public class WinScreenView : MonoBehaviour
    {
        public event Action OnResetButtonClicked;

        [SerializeField] private ParticleSystem _winEffect;
        [SerializeField] private Button _resetLevelButton;

        [SerializeField] private float _effectDelay = 0.25f;

        private void OnEnable()
        {
            StartEffectByDelay();
            _resetLevelButton.onClick.AddListener(OnResetButtonClickedHandler);
        }

        private void OnDisable()
        {
            _resetLevelButton.onClick.RemoveListener(OnResetButtonClickedHandler);
        }

        private async void StartEffectByDelay()
        {
            await Task.Delay((int)(1000 * _effectDelay));
            _winEffect.Play();
        }

        private void OnResetButtonClickedHandler()
        {
            OnResetButtonClicked?.Invoke();
        }
    }
}
