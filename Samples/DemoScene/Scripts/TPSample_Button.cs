using UnityEngine;
using UnityEngine.Events;

namespace TileProperties
{
    public class TPSample_Button : MonoBehaviour
    {
        public UnityEvent PressedEvent = new UnityEvent();
        public Color PressedColor;

        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<TPSample_PlayerController>())
            {
                sr.color = PressedColor;
                PressedEvent.Invoke();
            }
        }
    }
}
