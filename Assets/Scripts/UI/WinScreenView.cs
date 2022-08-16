using System;
using UnityEngine;
using UnityEngine.UI;

namespace OctanGames.UI
{
    public class WinScreenView : MonoBehaviour
    {
        public event Action OnResetButtonClicked;
        
        [SerializeField] private Button _resetLevelButton;

        private void OnEnable()
        {
            _resetLevelButton.onClick.AddListener(OnResetButtonClickedHandler);
        }

        private void OnDisable()
        {
            _resetLevelButton.onClick.RemoveListener(OnResetButtonClickedHandler);
        }

        private void OnResetButtonClickedHandler()
        {
            OnResetButtonClicked?.Invoke();
        }
    }
}
