using UnityEngine;

namespace Carrom.GameHanlder
{
    public class GameplayManager : MonoBehaviour
    {
        public int numberOfPlayers;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

    }

}