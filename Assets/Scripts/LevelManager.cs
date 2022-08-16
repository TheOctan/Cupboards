using System;
using System.Threading.Tasks;
using OctanGames.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OctanGames
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private WinScreenView _winScreenView;
        [SerializeField] private float _effectDelay = 0.5f;

        private void OnEnable()
        {
            _winScreenView.OnResetButtonClicked += OnResetButtonClickedHandler;
        }

        private void OnDisable()
        {
            _winScreenView.OnResetButtonClicked -= OnResetButtonClickedHandler;
        }

        public void OnWin()
        {
            EnableWinScreenByDelay();
        }
        
        private async void EnableWinScreenByDelay()
        {
            await Task.Delay((int)(1000 * _effectDelay));
            _winScreenView.gameObject.SetActive(true);
        }

        private static void OnResetButtonClickedHandler()
        {
            SceneManager.LoadScene(0);
        }
    }
}