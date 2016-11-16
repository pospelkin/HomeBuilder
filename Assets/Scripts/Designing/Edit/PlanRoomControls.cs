using UnityEngine;

namespace HomeBuilder.Designing
{
    public class PlanRoomControls : MonoBehaviour
    {
        public delegate void OnControlDrag(Vector2 type, Vector2 shift);
        public OnControlDrag onControlDrag;

        public delegate void OnControlDragEnd();
        public OnControlDragEnd onControlDragEnd;

        public delegate void OnControlClick();
        public OnControlClick onControlClick;

        public DragOperator leftUp;
        public DragOperator leftBottom;
        public DragOperator rightUp;
        public DragOperator rightBottom;
        public DragOperator center;
        public DragOperator up;
        public DragOperator bottom;
        public DragOperator left;
        public DragOperator right;

        public GameObject seleted;

        public void SetActive(bool active)
        {
            seleted.SetActive(active);
            leftUp.gameObject.SetActive(active);
            leftBottom.gameObject.SetActive(active);
            rightUp.gameObject.SetActive(active);
            rightBottom.gameObject.SetActive(active);
            center.gameObject.SetActive(active);
            up.gameObject.SetActive(active);
            bottom.gameObject.SetActive(active);
            left.gameObject.SetActive(active);
            right.gameObject.SetActive(active);
        }

        void Start()
        {
            DragOperator.onDragHandler += OnDragHandler;
            DragOperator.onClickHandler += OnClickHandler;
            DragOperator.onDragEndHandler += OnDragEndHandler;
        }

        void OnClickHandler(DragOperator dragOp)
        {
            if (onControlClick != null && dragOp == center) onControlClick();
        }

        void OnDragEndHandler(DragOperator dragOp)
        {
            if (onControlDragEnd != null) onControlDragEnd();
        }

        void OnDragHandler(DragOperator dragOp)
        {
            if (onControlDrag == null) return;

            if (dragOp == leftUp)
            {
                onControlDrag(new Vector2(-1, -1), dragOp.GetShift());
            } else if (dragOp == leftBottom)
            {
                onControlDrag(new Vector2(-1, 1), dragOp.GetShift());
            }
            else if (dragOp == rightBottom)
            {
                onControlDrag(new Vector2(1, 1), dragOp.GetShift());
            }
            else if (dragOp == rightUp)
            {
                onControlDrag(new Vector2(1, -1), dragOp.GetShift());
            }
            else if (dragOp == center && center.IsLongTapped())
            {
                onControlDrag(new Vector2(0, 0), dragOp.GetShift());
            }
            else if (dragOp == left)
            {
                onControlDrag(new Vector2(-1, 0), dragOp.GetShift());
            }
            else if (dragOp == right)
            {
                onControlDrag(new Vector2(1, 0), dragOp.GetShift());
            }
            else if (dragOp == up)
            {
                onControlDrag(new Vector2(0, -1), dragOp.GetShift());
            }
            else if (dragOp == bottom)
            {
                onControlDrag(new Vector2(0, 1), dragOp.GetShift());
            }
        }

        void OnDestroy()
        {
            DragOperator.onDragHandler -= OnDragHandler;
            DragOperator.onClickHandler -= OnClickHandler;
            DragOperator.onDragEndHandler -= OnDragEndHandler;
        }
    }
}