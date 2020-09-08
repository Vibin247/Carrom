using System;
using UnityEngine;

namespace Carrom.StrikerControl
{
    public class StrikerHandler : MonoBehaviour
    {
        [SerializeField] float maxRange;

        [SerializeField] Stiker striker;
        bool handleStriker = false;
        bool disabled = false;

        void Update()
        {
            InteractWithStrikerHandle();
        }

        private void InteractWithStrikerHandle()
        {
            if (Input.touchCount > 0 && !disabled)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Collider2D[] collider = Physics2D.OverlapPointAll(touchPos);
                if (touch.phase == TouchPhase.Began)
                {
                    if (Array.IndexOf(collider, GetComponent<Collider2D>()) != -1)
                    {
                        handleStriker = true;
                    }
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (handleStriker)
                    {
                        striker.GetComponent<CircleCollider2D>().enabled = false;
                        MoveStriker(touchPos);
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (striker.CheckForCoinOverLap())
                    {
                        striker.GetComponent<CircleCollider2D>().enabled = true;
                    }
                    handleStriker = false;
                }
            }
        }

        public void ResetHandler()
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        }

        private void MoveStriker(Vector2 touchPos)
        {
            float x = maxRange;
            if (Mathf.Abs(touchPos.x) < maxRange)
            {
                x = touchPos.x;
            }
            else if(touchPos.x < -1 * maxRange)
            {
                x = -1 * maxRange;
            }
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            striker.transform.localPosition = new Vector3(x, striker.transform.localPosition.y, striker.transform.localPosition.z);
            striker.CheckForCoinOverLap();
        }
        public void DisableStrikerHandler()
        {
            disabled = true;
        }
        public void EnableStrikerHandler()
        {
            disabled = false;
        }
    }
}