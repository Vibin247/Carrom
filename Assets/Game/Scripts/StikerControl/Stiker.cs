using Carrom.Board;
using Carrom.GameHanlder;
using System.Collections;
using UnityEngine;

namespace Carrom.StrikerControl
{
    public class Stiker : MonoBehaviour
    {
        bool aim = false;
        bool validStrikerPosition = true;
        bool disabled = false;
        GameObject arrowInstance;
        Vector2 shotDirection;
        float shotPower;

        [Range(100,1000)][SerializeField] float shotPowerFactor = 10f;
        [SerializeField] Animator strikerGlow = null;
        [SerializeField] GameObject power = null;
        [SerializeField] GameObject arrow = null;
        [SerializeField] float maxDragDistance;
        [SerializeField] Canvas invalidStrikerPosWarning;

        CoinHandler collisionCoin;
        private void Start()
        {
            if (strikerGlow != null)
            {
                strikerGlow.SetBool("TriggerGlow", true);
            }
            if(power != null)
            {
                power.SetActive(false);
            }
        }

        internal void EnableStriker()
        {
            disabled = false;
            ToggleStriker();
        }

        internal void DisableStriker()
        {
            disabled = true;
            ToggleStriker();
        }

        private void ToggleStriker()
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = !disabled;
            }
            GetComponent<CircleCollider2D>().enabled = !disabled;
        }

        private void Update()
        {
            AimShot();
        }

        public void RemoveStriker()
        {
            Destroy(gameObject);
        }

        private void AimShot()
        {
            if(Input.touchCount > 0 && !disabled)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                Collider2D collider = Physics2D.OverlapPoint(touchPos);
                if (touch.phase == TouchPhase.Began)
                {
                    if (collider == GetComponent<Collider2D>())
                    {
                        toggleStrikerStatus(true);
                    }
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (aim)
                    {
                        ShowProjection(touchPos);
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (aim)
                    {
                        toggleStrikerStatus(false);
                        shootStriker();
                    }
                }
            }
        }

        internal void ResetStriker(Transform defaultStrikerPosition)
        {
            toggleStrikerStatus(false);
            transform.position = defaultStrikerPosition.position;
            transform.rotation = defaultStrikerPosition.rotation;
        }

        public bool CheckForCoinOverLap()
        {
            Vector2 pointA = new Vector2(transform.position.x + GetComponent<CircleCollider2D>().radius, transform.position.y);
            Vector2 pointB = new Vector2(transform.position.x - GetComponent<CircleCollider2D>().radius, transform.position.y);
            if (Physics2D.OverlapAreaAll(pointA, pointB).Length != 0)
            {
                foreach (Collider2D collider in Physics2D.OverlapAreaAll(pointA, pointB))
                {
                    if (collider.tag == "Coin")
                    {
                        validStrikerPosition = false;
                        strikerGlow.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        validStrikerPosition = true;
                        strikerGlow.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
            else
            {
                strikerGlow.GetComponent<SpriteRenderer>().color = Color.white;
                validStrikerPosition = true;
            }
            invalidStrikerPosWarning.enabled = !validStrikerPosition;
            return validStrikerPosition;
        }

        private void shootStriker()
        {
            if (validStrikerPosition)
            {
                strikerGlow.gameObject.SetActive(false);
                Rigidbody2D rb2d = transform.GetComponent<Rigidbody2D>();
                rb2d.constraints = RigidbodyConstraints2D.None;
                if (collisionCoin != null)
                {
                    collisionCoin.RemoveCollisionTrajectory();
                    collisionCoin = null;
                }
                rb2d.AddForce(shotDirection.normalized * (shotPower * shotPowerFactor));
                FindObjectOfType<GameStateHandler>().SetGameState(GameState.StrikerShot);
            }
        }
            
        private void toggleStrikerStatus(bool aim)
        {
            strikerGlow.gameObject.SetActive(!aim);
            this.aim = aim;
            power.SetActive(aim);
            strikerGlow.SetBool("TriggerGlow", !aim);
            if (aim)
            {
                arrowInstance = Instantiate(arrow, transform);
                power.transform.localScale = new Vector3(0f, 0f, 0f);
                arrowInstance.transform.localScale = new Vector3(0f, 0f, 0f);
            }
            else
            {
                Destroy(arrowInstance);
            }
        }

        private void ShowProjection(Vector2 touchPos)
        {
            float powerScaleValue = CalculatePowerScaleValue(touchPos);
            Vector2 projectionDirection = new Vector2(transform.position.x, transform.position.y) - touchPos;
            float projectionAngle = CalculateProjectionAngle(touchPos, powerScaleValue, projectionDirection);

            ScaleProjectedArrowWithRespectToPower(powerScaleValue);

            transform.rotation = Quaternion.Euler(0f, 0f, projectionAngle);
            ProjectCollision(projectionDirection, powerScaleValue);
        }

        private void ProjectCollision(Vector2 projectionDirection, float powerScaleValue)
        {
            if (collisionCoin != null)
            {
                collisionCoin.RemoveCollisionTrajectory();
                collisionCoin = null;
            }
            Vector2 rayOrigin = new Vector2(transform.position.x, arrowInstance.transform.position.y + 0.1f);
            //RaycastHit2D hit = Physics2D.Raycast(rayOrigin, projectionDirection);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, projectionDirection);
            if (hits.Length > 0)
            {
                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.tag == "Coin")
                    {
                        collisionCoin = hit.collider.GetComponent<CoinHandler>();
                        collisionCoin.PredictCollisionTrajectory(hit.point, powerScaleValue / 1.5f);
                        break;
                    }
                }
            }
        }

        float CalculatePowerScaleValue(Vector2 touchPos)
        {
            Vector2 startPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
            float dragDist = Vector2.Distance(startPos, touchPos);
            if (dragDist > maxDragDistance)
            {
                dragDist = maxDragDistance;
            }
            shotPower = dragDist;
            return Mathf.Max(0.1f, (2 * dragDist + 1) / 10);
        }

        float CalculateProjectionAngle(Vector2 touchPos, float powerScaleValue, Vector2 projectionDirection)
        {
            power.transform.localScale = new Vector3(powerScaleValue, powerScaleValue, powerScaleValue);
            float projectionAngle = Vector2.Angle(projectionDirection, Vector2.up);
            bool isProjectionOnRight = Vector2.Angle(projectionDirection, Vector2.right) < 90f;
            if (isProjectionOnRight)
            {
                projectionAngle *= -1;
            }
            shotDirection = projectionDirection;
            return projectionAngle;
        }

        void ScaleProjectedArrowWithRespectToPower(float powerScaleValue)
        {
            float arrowScaleValue = 0;
            if(powerScaleValue > 0.1f)
            {
                arrowScaleValue = (2 * powerScaleValue - 0.25f) / 1.5f;
            }
            Vector3 arrowScale = new Vector3(arrowScaleValue, arrowScaleValue, arrowScaleValue);
            arrowInstance.transform.localScale = arrowScale;
        }
        public IEnumerator StopStiker()
        {
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            var oldDrag = rigidbody.drag;
            rigidbody.drag = 7;
            yield return new WaitForSeconds(0.5f);
            HideStriker();
            yield return new WaitForSeconds(1.5f);            
            rigidbody.drag = oldDrag;
            Destroy(gameObject);
        }

        private void HideStriker()
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.tag == "StrikerObject")
                    renderer.color = Color.Lerp(renderer.color, Color.clear, 1.5f);
            }
        }

        public void CheckForFoul()
        {
            if (GetComponent<Rigidbody2D>().velocity.magnitude <= 0.25f)
            {
                StartCoroutine(StopStiker());
            }
        }

    }
}
