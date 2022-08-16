using System;
using OctanGames.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OctanGames
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private WinScreenView _winScreenView;

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
            _winScreenView.gameObject.SetActive(true);
        }

        private static void OnResetButtonClickedHandler()
        {
            SceneManager.LoadScene(0);
        }
    }
}