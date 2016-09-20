﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HomeBuilder.Designing
{
    public class PlanRoom : MonoBehaviour
    {

        public delegate void OnSelect(PlanRoom plan);
        public delegate void OnMove(PlanRoom plan, Vector2 change);
        public delegate void OnMoveEnd(PlanRoom plan);
        public delegate void OnResize(PlanRoom plan, Vector2 change);
        public delegate void OnRemove(PlanRoom plan);
        static public OnSelect onSelect;
        static public OnMove onMove;
        static public OnResize onResize;
        static public OnMoveEnd onMoveEnd;
        static public OnRemove onRemove;

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

        public void Resize(Vector2 shift)
        {
            if (onResize != null) onResize(this, new Vector2(shift.x, -shift.y));
        }

        void Start()
        {
            controls.onControlDrag += OnControlDrag;
            controls.onControlClick += OnControlClick;
            controls.onControlDragEnd += OnControlDragEnd;
        }

        void OnDestroy()
        {
            controls.onControlDrag -= OnControlDrag;
            controls.onControlClick -= OnControlClick;
            controls.onControlDragEnd -= OnControlDragEnd;
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
            GetComponent<Image>().color = Color.green;
        }

        public void UnMark()
        {
            GetComponent<Image>().color = Color.white;
        }

        void OnControlDrag(Vector2 type, Vector2 shift)
        {

            if (type.x == 0 && type.y == 0)
            {
                Move(shift);
            } else
            {
                if (shift.x == 0 && shift.y == 0) return;
                Resize(new Vector2(type.x * shift.x, type.y * shift.y));
            }
        }
    }
}