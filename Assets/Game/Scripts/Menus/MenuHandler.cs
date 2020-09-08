using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Carrom.GameHanlder;
using System;

namespace Assets.Game.Scripts.Menus
{
    public class MenuHandler : MonoBehaviour
    {
        private void Awake()
        {
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                GetComponent<Canvas>().enabled = false;
            }
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == 1)
            {
                PauseGame();
            }
        }
        public void StartSinglePlayer()
        {
            FindObjectOfType<GameplayManager>().numberOfPlayers = 1;
            StartGame();
        }
        public void StartDoublePlayer()
        {
            FindObjectOfType<GameplayManager>().numberOfPlayers = 2;
            StartGame();
        }
        public void StartGame()
        {
            Time.timeScale = 1;
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadSceneAsync(1);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void PauseGame()
        {
            GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            GetComponent<Canvas>().enabled = false;
            Time.timeScale = 1;
        }
    }
}