﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Designing
{
    public class Editor : MonoBehaviour
    {

        public Canvas canvas;
        public Transform container;
        Button removeB {
            get { return GetComponentInChildren<Button>(); }
        }

        Layout layout;
        List<PlanRoom> plans = null;
        float coef;
        PlanRoom selected = null;
        PlanRoom movePlan = null;

        public void SetLayout(Layout l)
        {
            layout  = l;

            l.onChange += UpdateAll;
        }

        public void TurnOn()
        {
            container.gameObject.SetActive(true);

            if (plans == null)
            {
                CreatePlans();
            }
        }

        public void TurnOff()
        {
            container.gameObject.SetActive(false);
        }

        void Start()
        {
        }

        void CreatePlans()
        {
            LayoutElement[] elements    = layout.GetElements();
            coef                        = GetCoef(elements[0]);

            plans = new List<PlanRoom>();
            for (int i = 0; i < elements.Length; i++)
            {
                plans.Add( CreatePlan(elements[i]) );
            }

            UpdateAll();

            PlanRoom.onResize   += OnResize;
            PlanRoom.onMove += OnMove;
            PlanRoom.onMoveEnd += OnMoveEnd;
            PlanRoom.onRemove += OnRemove;
            PlanRoom.onSelect += OnSelect;
        }

        public void Remove()
        {
            if (selected != null)
            {
                OnRemove(selected);
            }
        }

        void OnSelect(PlanRoom plan)
        {
            selected = plan;
            UpdateControls();
        }

        float GetCoef(LayoutElement el)
        {
            return Mathf.Min( Screen.width, Screen.height) / 30f;
        }

        PlanRoom CreatePlan(LayoutElement element)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.planRoom), container) as GameObject;
            obj.transform.localScale = new Vector3(1,1,1);

            PlanRoom plan = obj.GetComponent<PlanRoom>();
            plan.SetLayoutElement(element);

            return plan;
        }

        public void UpdateControls()
        {
            removeB.interactable = selected != null;

            int length = plans.Count;
            for (int i = 0; i < length; i++)
            {
                PlanRoom plan = plans[i];
                int x = plan.layoutElement.i;
                int y = plan.layoutElement.j;
                
                plan.controls.SetActive(false);
                plan.controls.center.gameObject.SetActive(true);
                if (plan == selected)
                {
                    plan.controls.SetActive(true);
                    if (x == 0)
                    {
                        plan.controls.left.gameObject.SetActive(false);
                        plan.controls.leftBottom.gameObject.SetActive(false);
                        plan.controls.leftUp.gameObject.SetActive(false);
                    }
                    if (y == 0)
                    {
                        plan.controls.up.gameObject.SetActive(false);
                        plan.controls.leftUp.gameObject.SetActive(false);
                        plan.controls.rightUp.gameObject.SetActive(false);
                    }
                    if (x >= layout.matrix[y] - 1)
                    {
                        plan.controls.right.gameObject.SetActive(false);
                        plan.controls.rightBottom.gameObject.SetActive(false);
                        plan.controls.rightUp.gameObject.SetActive(false);
                    }
                    if (y >= layout.matrix.Length - 1)
                    {
                        plan.controls.bottom.gameObject.SetActive(false);
                        plan.controls.rightBottom.gameObject.SetActive(false);
                        plan.controls.leftBottom.gameObject.SetActive(false);
                    }
                }
                
            }
        }

        PlanRoom changed;
        Vector2 changedPos;
        void OnMove(PlanRoom plan, Vector2 change)
        {
            if (plan != selected) return;

            if (movePlan == null)
            {
                movePlan = CreatePlan(plan.layoutElement);
                movePlan.squareText.text = plan.squareText.text;
                movePlan.rectTransform.sizeDelta = plan.rectTransform.sizeDelta;
                movePlan.rectTransform.anchoredPosition = plan.rectTransform.anchoredPosition;
                movePlan.controls.SetActive(false);
                movePlan.moving = true;
            }

            movePlan.Move(change);

            int length = plans.Count;
            bool changedFlag = false;
            for (int i = 0; i < length; i++)
            {
                if (
                    plans[i] != plan
                    && IsNear(plans[i],
                        movePlan
                        )
                        )
                {
                    changedFlag = true;
                    changed = plans[i];
                    changedPos = movePlan.rectTransform.anchoredPosition + movePlan.rectTransform.sizeDelta / 2;
                    Debug.Log("Interchange ready");
                    plan.Mark();
                    Interchange(plan, changed);
                    break;
                }
            }
            if (!changedFlag)
            {
                if (changed != null && !IsNear(movePlan, plan) && !IsNear(movePlan, changedPos))
                {
                    plan.UnMark();
                    Interchange(plan, changed);
                    changed = null;
                }
            }
        }

        bool IsNear(PlanRoom p1, PlanRoom p2)
        {
            return IsNear(p1, p2.rectTransform.anchoredPosition + p2.rectTransform.sizeDelta / 2);
        }

        bool IsNear(PlanRoom p1, Vector2 p2)
        {
            return Vector2.Distance(
                        p1.rectTransform.anchoredPosition + p1.rectTransform.sizeDelta / 2,
                        p2
                        ) < Mathf.Min(p1.rectTransform.sizeDelta.x, p1.rectTransform.sizeDelta.y) / 3;
        }

        void Interchange(PlanRoom p1, PlanRoom p2)
        {
            layout.Interchange(p1.layoutElement, p2.layoutElement);
        }

        void OnMoveEnd(PlanRoom plan)
        {
            if (movePlan == null) return;

            int length = plans.Count;
            for (int i = 0; i < length; i++)
            {
                plans[i].UnMark();
            }
            
            Destroy(movePlan.gameObject);
            movePlan    = null;
            changed     = null;
        }

        void OnResize(PlanRoom plan, Vector2 change)
        {
            Vector2 oldSize = plan.layoutElement.GetSize();
            plan.layoutElement.SetSize(oldSize.x + change.x / coef, oldSize.y + change.y / coef);
        }

        void UpdateAll()
        {
            int length = plans.Count;
            for (int i = 0; i < length; i++)
            {
                UpdatePlan(plans[i]);
            }

            UpdateControls();
        }

        void UpdatePlan(PlanRoom plan)
        {
            LayoutElement element = plan.layoutElement;

            plan.SetSquare(element.GetSquare());
            plan.SetSize(element.GetSize() * coef);
            plan.SetPosition((element.GetPosition() - layout.GetAppSize() / 2) * coef);
        }

        void OnRemove(PlanRoom plan)
        {
            if (plans.Count <= 1) return;

            plans.Remove(plan);
            MonoBehaviour.Destroy(plan.gameObject);
            layout.Remove(plan.layoutElement);
        }

        void OnDestroy()
        {
            PlanRoom.onResize -= OnResize;
            PlanRoom.onMove -= OnMove;
            PlanRoom.onRemove -= OnRemove;
            PlanRoom.onSelect -= OnSelect;

            layout.onChange -= UpdateAll;

            layout = null;
        }

    }
}