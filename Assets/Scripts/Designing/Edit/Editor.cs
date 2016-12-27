using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Designing
{
    public class Editor : MonoBehaviour
    {

        public Canvas canvas;
        public Transform container;

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

        public void SetContainer(Transform c)
        {
            this.container = c;
        }

        public void SetCanvas(Canvas c)
        {
            this.canvas = c;
        }

        public void TurnOn()
        {
            container.parent.gameObject.GetComponent<CanvasGroup>().alpha = 1;

            if (plans == null)
            {
                CreatePlans();
            }
        }

        public void RecreatePlans()
        {
            foreach (PlanRoom p in plans)
            {
                Destroy(p.gameObject);
            }
            CreatePlans();

            UpdateAll();
        }

        public void TurnOff()
        {
            container.parent.gameObject.GetComponent<CanvasGroup>().alpha = 0;
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
        }

        void OnEnable()
        {
            PlanRoom.onResize += OnResize;
            PlanRoom.onMove += OnMove;
            PlanRoom.onMoveEnd += OnMoveEnd;
            PlanRoom.onRemove += OnRemove;
            PlanRoom.onSelect += OnSelect;
        }

        void OnDisable()
        {
            PlanRoom.onResize -= OnResize;
            PlanRoom.onMove -= OnMove;
            PlanRoom.onMoveEnd -= OnMoveEnd;
            PlanRoom.onRemove -= OnRemove;
            PlanRoom.onSelect -= OnSelect;
        }

        public void ClearAllSigns()
        {
            selected = null;
            UpdateControls();
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

            selected.transform.SetAsLastSibling();
        }

        float GetCoef(LayoutElement el)
        {
            float height = layout.GetAppSize()[1];
            
            return (1536) * 0.60f / height;
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
            //if (plan != selected) return;

            if (movePlan == null)
            {
                movePlan = CreatePlan(plan.layoutElement);
                movePlan.squareText.text = plan.squareText.text;
                movePlan.rectTransform.sizeDelta = plan.rectTransform.sizeDelta;
                movePlan.rectTransform.anchoredPosition = plan.rectTransform.anchoredPosition;
                movePlan.controls.SetActive(false);
                movePlan.controls.SetDragging(true);
                movePlan.moving = true;

                movePlan.transform.SetAsLastSibling();
                plan.Mark();
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
                        ) && AllowChange(plan, plans[i])
                        )
                {
                    changedFlag = true;
                    changed = plans[i];
                    changedPos = movePlan.rectTransform.anchoredPosition/* + movePlan.rectTransform.sizeDelta / 2*/;
                    Debug.Log("Interchange ready");
                    //plan.Mark();
                    //movePlan.UnMark();
                    Interchange(plan, changed);
                    break;
                }
            }
            if (!changedFlag)
            {
                if (changed != null && !IsNear(movePlan, plan) && !IsNear(movePlan, changedPos) )
                {
                    //movePlan.UnMark();
                    //plan.UnMark();
                    Interchange(plan, changed);
                    changed = null;
                }
            }

            //if (changed == null) movePlan.MarkRed();
        }

        bool AllowChange(PlanRoom p1, PlanRoom p2)
        {
            if (p1.layoutElement.GetPosition().x == p2.layoutElement.GetPosition().x
                && p1.layoutElement.GetSize().x == p2.layoutElement.GetSize().x) return true;
            if (p1.layoutElement.GetPosition().y == p2.layoutElement.GetPosition().y
                && p1.layoutElement.GetSize().y == p2.layoutElement.GetSize().y) return true;
            if (p1.layoutElement.GetSize().x == p2.layoutElement.GetSize().x
                && p1.layoutElement.GetSize().y == p2.layoutElement.GetSize().y) return true;
            return false;
        }

        bool IsNear(PlanRoom p1, PlanRoom p2)
        {
            return IsNear(p1, p2.rectTransform.anchoredPosition/* + p2.rectTransform.sizeDelta / 2*/);
        }

        bool IsNear(PlanRoom p1, Vector2 p2)
        {
            return Vector2.Distance(
                        p1.rectTransform.anchoredPosition/* + p1.rectTransform.sizeDelta / 2*/,
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

        Dictionary<PlanRoom, Vector2> dictionary = new Dictionary<PlanRoom, Vector2>();
        void OnResize(PlanRoom plan, Vector2 type, Vector2 change)
        {
            if (!dictionary.ContainsKey(plan))
            {
                dictionary.Add(plan, new Vector2(0,0));
            }
            Vector2 v = dictionary[plan];

            v.x += change.x;
            v.y += change.y;

            Vector2 oldSize = plan.layoutElement.GetSize();
            float   xChange = v.x / coef * type.x,
                    yChange = v.y / coef * type.y;

            float limit = 0.05f;
            if (Mathf.Abs( xChange ) > 0 && Mathf.Abs( yChange )> limit)
            {
                plan.layoutElement.RequestSizeChage(type, new Vector2(xChange, yChange));

                v.y = 0;
                v.x = 0;
            } else if (Mathf.Abs( xChange ) > limit)
            {
                plan.layoutElement.RequestSizeChage(type, new Vector2(xChange, 0));
                v.x = 0;
            }
            else if (Mathf.Abs(yChange) > limit)
            {
                plan.layoutElement.RequestSizeChage(type, new Vector2(0, yChange));
                v.y = 0;
            }
            else
            {
                Debug.Log("limit " + limit + " and we have " + xChange + " and " + yChange);
            }
            dictionary[plan] = v;

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

            if (layout.Remove(plan.layoutElement))
            {
                plans.Remove(plan);
                MonoBehaviour.Destroy(plan.gameObject);
            }
        }

        void OnDestroy()
        {
            layout.onChange -= UpdateAll;

            plans  = null;
            layout = null;
        }

    }
}