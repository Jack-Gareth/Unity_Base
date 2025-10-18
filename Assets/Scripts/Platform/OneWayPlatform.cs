using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private bool _canFallThrough = false;
    private float _fallThroughTimer = 0f;
    private const float _fallThroughTime = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<PlatformEffector2D>().enabled = !_canFallThrough;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canFallThrough = false;
            GetComponent<PlatformEffector2D>().enabled = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                _fallThroughTimer += Time.deltaTime;

                if (_fallThroughTimer >= _fallThroughTime)
                {
                    _canFallThrough = true;
                }
            }
            else
            {
                _fallThroughTimer = 0f;
                _canFallThrough = false;
            }
        }
    }
}