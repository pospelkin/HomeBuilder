using UnityEngine;
using UnityEngine.EventSystems;

namespace HomeBuilder.Designing
{
    public class DragOperator : MonoBehaviour
    {

        public delegate void OnClickHandler(DragOperator dragOperator);
        static public OnClickHandler onClickHandler;

        public delegate void OnDragHandler(DragOperator dragOperator);
        static public OnDragHandler onDragHandler;

        public delegate void OnDragEndHandler(DragOperator dragOperator);
        static public OnDragEndHandler onDragEndHandler;

        public RectTransform reference;
        public bool adaptiveCollider = false;
        public float longTapSec = 2;

        bool drag = false;
        float time = -1;
        Vector2 initPos;

        void OnMouseDown()
        {
            initPos = Input.mousePosition;

            time = Time.time;
        }

        void OnMouseUp()
        {
            if (drag && onDragEndHandler != null) onDragEndHandler(this);
            if (Time.time - time < 0.4f)
            {
                if (onClickHandler != null) onClickHandler(this);
            }
            drag = false;
            time    = -1;
        }

        public bool IsLongTapped()
        {
            return time > 0 ? (Time.time - time > longTapSec) : false;
        }
        
        public Vector2 GetShift()
        {
            return (Vector2) Input.mousePosition - initPos;
        }

        void Update()
        {
            if (!adaptiveCollider) return;

            BoxCollider2D box   = GetComponent<BoxCollider2D>();
            RectTransform rect  = (RectTransform) transform;
            if (box != null)
            {
                Vector2 size = Vector2.zero;
                if (rect.rect.width < rect.rect.height)
                {
                    size = new Vector2(rect.rect.width + 50, rect.rect.height);
                }
                else
                {
                    size = new Vector2(rect.rect.width, rect.rect.height + 50);
                }
                box.size    = size;
                box.offset  = new Vector2(0, 0);
            }
        }

        void OnMouseDrag()
        {
            drag = true;
            if (onDragHandler != null) onDragHandler(this);

            initPos = Input.mousePosition;
        }

    }
}