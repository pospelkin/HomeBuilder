using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HomeBuilder.Designing
{
    public class PlanRoom : MonoBehaviour
    {

        public delegate void OnSelect(PlanRoom plan);
        public delegate void OnMove(PlanRoom plan, Vector2 change);
        public delegate void OnMoveEnd(PlanRoom plan);
        public delegate void OnResize(PlanRoom plan, Vector2 type, Vector2 change);
        public delegate void OnRemove(PlanRoom plan);
        public static OnSelect onSelect;
        public static OnMove onMove;
        public static OnResize onResize;
        public static OnMoveEnd onMoveEnd;
        public static OnRemove onRemove;

        public static int id = 0;

        public bool moving = false;

        public Text squareText;
        public PlanRoomControls controls;
        public LayoutElement layoutElement;
        
        public RectTransform rectTransform {
            get { return GetComponent<RectTransform>(); }
        }

        public void SetLayoutElement(LayoutElement le)
        {
            layoutElement = le;
        }

        public void SetSquare(float sq)
        {
            squareText.text = "" + sq + " m2";
        }

        public void SetSize(Vector2 size)
        {
            rectTransform.sizeDelta = size;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = new Vector2(position.x, -position.y);
        }

        public void Move(Vector2 shift)
        {
            if (moving)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + shift.x, rectTransform.anchoredPosition.y + shift.y);
            }
            else
            {
                if (onMove != null) onMove(this, shift);
            }
        }

        public void Resize(Vector2 type, Vector2 shift)
        {
            if (onResize != null) onResize(this, type, new Vector2(shift.x, -shift.y));
        }

        void Start()
        {
            controls.onControlDrag += OnControlDrag;
            controls.onControlClick += OnControlClick;
            controls.onControlDragEnd += OnControlDragEnd;

            name += "_" + id++;
        }

        void OnDestroy()
        {
            controls.onControlDrag -= OnControlDrag;
            controls.onControlClick -= OnControlClick;
            controls.onControlDragEnd -= OnControlDragEnd;

            Destroy(controls);
            layoutElement = null;
        }

        void OnControlDragEnd()
        {
            if (onMoveEnd != null) onMoveEnd(this);
        }

        void Update() {
            
        }

        float time = 0;
        void OnMouseDown()
        {
            if (Time.time - time < 0.5)
            {
                if (onRemove != null) onRemove(this);
            }
            time = Time.time;
        }

        void OnControlClick()
        {
            if (onSelect != null) onSelect(this);
        }

        public void Mark()
        {
            controls.drawn.SetActive(true);
        }

        public void MarkRed()
        {
            controls.drawn.SetActive(true);
        }

        public void UnMark()
        {
            controls.drawn.SetActive(false);
        }

        void OnControlDrag(Vector2 type, Vector2 shift)
        {

            if (type.x == 0 && type.y == 0)
            {
                Move(shift);
            } else
            {
                if (shift.x == 0 && shift.y == 0) return;
                Resize(type, shift);
            }
        }
    }
}