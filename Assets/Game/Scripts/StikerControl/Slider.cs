using UnityEngine;

namespace Carrom.StrikerControl
{
    public class Slider : MonoBehaviour
    {
        [SerializeField] Stiker striker;
        [SerializeField] StrikerHandler strikerHandle;
        [SerializeField] float maxRange;
        bool sliderSelected = false;
        bool disabled = false;

        void Update()
        {
            CheckForTouch();
        }
        private void CheckForTouch()
        {
            if (Input.touchCount > 0 && !disabled)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Collider2D collider = Physics2D.OverlapPoint(touchPos);
                if (touch.phase == TouchPhase.Began)
                {
                    if (collider == GetComponent<Collider2D>())
                    {
                        sliderSelected = true;
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if(sliderSelected)
                    {
                        MoveStriker(touchPos);
                    }
                }
            }
        }

        private void MoveStriker(Vector2 touchPos)
        {
            float x = maxRange;
            if(Mathf.Abs(touchPos.x) < maxRange)
            {
                x = touchPos.x;
            }
            else if (touchPos.x < -1 * maxRange)
            {
                x = -1 * maxRange;
            }
            strikerHandle.GetComponent<Rigidbody2D>().MovePosition(new Vector2(x, strikerHandle.transform.localPosition.y));
            striker.GetComponent<Rigidbody2D>().MovePosition(new Vector2(x, striker.transform.localPosition.y));
            striker.CheckForCoinOverLap();
            sliderSelected = false;
        }

        public void DisableSlider()
        {
            disabled = true;
        }
        public void EnableSlider()
        {
            disabled = false;
        }
    }
}