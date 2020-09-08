using UnityEngine;
using System.Collections;
using Carrom.GameHanlder;

namespace Carrom.Board
{
    public class CoinHandler : MonoBehaviour
    {
        [SerializeField] CoinColor type;
        [SerializeField] int coinPoints;
        [SerializeField] GameObject aimArrow;
        GameObject aimArrowInstance = null;
        public IEnumerator CoinScored()
        {
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.drag = 25;
            rigidbody.angularDrag = 25;
            GetComponent<CircleCollider2D>().enabled = false;
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            while (renderer.color.a > 0)
            {
                yield return new WaitForSeconds(0.075f);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a - 0.1f);
            }
            Destroy(gameObject);
            yield return new WaitForSeconds(0.5f);
            FindObjectOfType<GameStateHandler>().HandleCoinScore(type, coinPoints);
        }

        public void PredictCollisionTrajectory(Vector2 point, float powerScaleValue)
        {
            Vector2 direction = (Vector2)transform.position - point;
            float projectionAngle = Vector2.Angle(direction, Vector2.up);
            bool isProjectionOnRight = Vector2.Angle(direction, Vector2.right) < 90f;
            if (isProjectionOnRight)
            {
                projectionAngle *= -1;
            }
            if(aimArrowInstance == null)
            {
                aimArrowInstance = Instantiate(aimArrow, transform);
            }
            aimArrowInstance.transform.localScale = new Vector3(powerScaleValue, powerScaleValue, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, projectionAngle);

        }

        public void RemoveCollisionTrajectory()
        {
            Destroy(aimArrowInstance);
            aimArrowInstance = null;
        }
        public CoinColor GetCoinColor()
        {
            return type;
        }
    }
}