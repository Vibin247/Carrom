using Assets.Game.Scripts.Menus;
using Carrom.Board;
using Carrom.Player;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Carrom.GameHanlder
{
    public class GameStateHandler : MonoBehaviour
    {
        GameState currentState = GameState.Initial;
        bool noCoinScored = true;
        PlayerHandler activePlayer;
        [SerializeField] PlayerHandler[] allPlayers;
        PlayerHandler[] playerHandlers;
        [SerializeField] Canvas victoryCanvas;

        private void Start()
        {
            StartGame(FindObjectOfType<GameplayManager>().numberOfPlayers);
            victoryCanvas.enabled = false;
            currentState = GameState.AimPhase;
        }

        public void StartGame(int numberOfPlayers)
        {
            Array.Resize(ref playerHandlers, numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerHandlers[i] = Instantiate(allPlayers[i], transform);
                playerHandlers[i].name = "Player " + (i + 1).ToString();
                if (i == 0)
                {
                    activePlayer = playerHandlers[i];
                    playerHandlers[i].SelectPlayer();
                }
                else
                {
                    playerHandlers[i].DeselectPlayer();
                }
            }
            if(numberOfPlayers == 1)
            {
                foreach(SpriteRenderer sprite in activePlayer.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (sprite.name.Equals("Coin"))
                    {
                        sprite.enabled = false;
                    }
                }
            }
        }

        private void Update()
        {
            if (currentState == GameState.StrikerShot)
            {
                CheckForMovingCoins();
            }
        }

        private void CheckForMovingCoins()
        {
            bool coinsStopped = true;
            foreach(CoinHandler coin in FindObjectsOfType<CoinHandler>())
            {
                if (!coin.GetComponent<Rigidbody2D>().IsSleeping())
                {
                    coinsStopped = false;
                    break;
                }
            }
            if(coinsStopped && activePlayer.IsStrikerSleep())
            {
                EndTurn();
            }
        }

        public void HandleCoinScore(CoinColor type, int coinPoints)
        {
            noCoinScored = false;
            if (type == activePlayer.GetPlayerType() || playerHandlers.Length == 1)
            {
                activePlayer.AddScore(coinPoints);
            }
            bool whiteCoinsAllScored = true;
            bool blackCoinsAllScored = true;
            print(FindObjectsOfType<CoinHandler>().Length);
            foreach (CoinHandler coin in FindObjectsOfType<CoinHandler>())
            {
                if(coin.GetCoinColor() == CoinColor.White) { whiteCoinsAllScored = false; }
                if (coin.GetCoinColor() == CoinColor.Black) { blackCoinsAllScored = false; }
            }
            print(whiteCoinsAllScored + "\t" + blackCoinsAllScored);
            if(playerHandlers.Length == 1)
            {
                if(whiteCoinsAllScored && blackCoinsAllScored)
                {
                    print(activePlayer);
                    GameOver(activePlayer);
                }
            }
            else if(whiteCoinsAllScored || blackCoinsAllScored)
            {
                if ( (whiteCoinsAllScored && activePlayer.GetPlayerType() == CoinColor.White) || (blackCoinsAllScored && activePlayer.GetPlayerType() == CoinColor.Black))
                {
                    GameOver(activePlayer);
                }
                else
                {
                    SwitchPlayer(Array.IndexOf(playerHandlers, activePlayer) + 1);
                    GameOver(activePlayer);
                }
            }
        }

        private void GameOver(PlayerHandler activePlayer)
        {
            if(playerHandlers.Length == 1)
            {
                victoryCanvas.GetComponentInChildren<Text>().text = "You have completed the game!";
            }
            else
            {
                victoryCanvas.GetComponentInChildren<Text>().text = activePlayer.gameObject.name + " has won!";
            }
            victoryCanvas.enabled = true;
            FindObjectOfType<MenuHandler>().PauseGame();
        }

        private void EndTurn()
        {
            if (noCoinScored)
            {
                if (playerHandlers.Length > 1)
                {
                    SetGameState(GameState.TrasitToNextPlayer);
                    SwitchPlayer(Array.IndexOf(playerHandlers, activePlayer) + 1);
                }
                else
                {
                    activePlayer.ResetPlayer();
                    SetGameState(GameState.AimPhase);
                }
            }
            else
            {
                noCoinScored = true;
                SetGameState(GameState.AimPhase);
                activePlayer.ResetPlayer();
            }
        }

        public void SetGameState(GameState newState)
        {
            currentState = newState;
        }

        public GameState GetCurrentGameState()
        {
            return currentState;
        }

        public void SwitchPlayer(int index)
        {
            if(index >= playerHandlers.Length)
            {
                index = 0;
            }
            SwitchPlayer(playerHandlers[index]);
        }
        public void SwitchPlayer(PlayerHandler newPlayer)
        {
            activePlayer.DeselectPlayer();
            activePlayer = newPlayer;
            activePlayer.SelectPlayer();
            SetGameState(GameState.AimPhase);
        }
    }
}
