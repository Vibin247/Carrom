using Carrom.StrikerControl;
using UnityEngine;

namespace Carrom.Board
{
    public class Pocket : MonoBehaviour
    {
        const string strikerTag = "Striker";
        const string coinTag = "Coin";
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == strikerTag)
            {
                Stiker strikerController = other.GetComponent<Stiker>();
                strikerController.CheckForFoul();
            }
            if (other.tag == coinTag)
            {
                CoinHandler coinHandler = other.GetComponent<CoinHandler>();
                StartCoroutine(coinHandler.CoinScored());
            }
        }
    }
}
