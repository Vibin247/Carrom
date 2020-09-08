using UnityEngine;
using Carrom.StrikerControl;
using Carrom.Board;
using System;
using UnityEngine.UI;

namespace Carrom.Player
{
    public class PlayerHandler : MonoBehaviour
    {
        [SerializeField] CoinColor playerType;
        bool playerSelected = false;
        int scoreNum = 0;
        [SerializeField] Text score;
        [SerializeField] Stiker striker;
        [SerializeField] StrikerControl.Slider slider;
        [SerializeField] StrikerHandler handler;
        [SerializeField] Transform defaultStrikerPosition;


        private void Awake()
        {
            playerType = CoinColor.White;
        }

        public CoinColor GetPlayerType()
        {
            return playerType;
        }

        public void SelectPlayer()
        {
            slider.EnableSlider();
            handler.EnableStrikerHandler();
            striker.EnableStriker();
            playerSelected = true;
        }
        public void DeselectPlayer()
        {
            ResetPlayer();
            slider.DisableSlider();
            handler.DisableStrikerHandler();
            striker.DisableStriker();
            playerSelected = false;
        }

        public bool ActivePlayer()
        {
            return playerSelected;
        }

        public bool IsStrikerSleep()
        {
            return striker.GetComponent<Rigidbody2D>().IsSleeping();
        }

        public void ResetPlayer()
        {
            handler.ResetHandler();
            striker.ResetStriker(defaultStrikerPosition);
        }

        public void AddScore(int coinPoints)
        {
            scoreNum += coinPoints;
            score.text = scoreNum.ToString();
        }

        internal void PlayerWin()
        {
            DeselectPlayer();
        }
    }
}