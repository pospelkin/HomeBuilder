using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{

    public float moveLimit = 20;
    public float swapLimit = 0.3f;

    public delegate void TapHandler(Vector3 position);
    public event TapHandler onTap;

    public delegate void SwapHandler(Vector3 position, Vector3 shift);
    public event SwapHandler onSwap;

    public delegate void MoveHandler(Vector3 position, Vector3 shift);
    public event MoveHandler onMove;

    public delegate void MovedHandler(Vector3 position, Vector3 shift);
    public event MoveHandler onMoved;

    public delegate void StartMoveHandler();
    public event StartMoveHandler onStartMove;
 
    public delegate void StartExtendHandler();
    public event StartExtendHandler onStartExtend;

    public delegate void ExtendHandler(Vector3 p1, Vector3 p2, float distance);
    public event ExtendHandler onExtend;

    public delegate void StopExtendHandler();
    public event StopExtendHandler onStopExtend;       

    bool pressed    = false;
    bool moved      = false;
    bool isExtend   = false;

    Vector3 startPos    = Vector3.zero;
    Vector3 tempPos     = Vector3.zero;
    float tempD         = 0;
    float startTime     = 0;
    float startExtendD  = 0;

    void Tap(Vector3 position)
    {
        if (onTap != null)
        {
            onTap(position);
        }
    }

    void Swap(Vector3 position, Vector3 shift)
    {
        if (onSwap != null)
        {
            onSwap(position, shift);
        }
    }

    void Move(Vector3 position, Vector3 shift)
    {
        if (onMove != null)
        {
            onMove(position, shift);
        }
    }

    void StartExtend()
    {
        if (onStartExtend != null)
        {
            onStartExtend();
        }
    }

    void Extend(Vector3 p1, Vector3 p2, float d)
    {
        if (onExtend != null)
        {
            onExtend(p1, p2, d);
        }
    }

    void StopExtend()
    {
        if (onStopExtend != null)
        {
            onStopExtend();
        }

    }

    void StartMove()
    {
        if (onStartMove != null)
        {
            onStartMove();
        }
    }

    void StopMove(Vector3 position, Vector3 shift)
    {
        if (onMoved != null)
        {
            onMoved(position, shift);
        }
    }

    void OnMouseUp()
    {
        if (Input.touchCount > 0)
        {
            EndInteraction(Input.touches);
        } else
        {
            EndInteraction();
        }
    }

    void OnMouseDown()
    {
        if (Input.touchCount > 0)
        {
            StartInteraction(Input.touches);
        }
        else
        {
            StartInteraction();
        }
    }

    void OnMouseOver()
    {
        if (Input.touchCount > 0)
        {
            Interaction(Input.touches);
        }
        else
        {
            Interaction();
        }
    }

    void StartInteraction()
    {
        pressed     = true;
        moved       = false;
        isExtend    = false;

        startPos    = Input.mousePosition;
        startTime   = Time.time;
    }

    void StartInteraction(Touch[] touchs)
    {
        moved       = false;
        pressed     = true;
        isExtend    = false;

        if (touchs.Length == 1)
        {
            startPos = touchs[0].position;
            startTime = Time.time;
        } else if (touchs.Length == 2)
        {
            startExtendD = Vector2.Distance(touchs[0].position, touchs[1].position);
            tempD = startExtendD;
        }
    }

    void Interaction()
    {
        if (!pressed) return;

        if (!Input.mousePosition.Equals(startPos))
        {
            if (IsMoving())
            {
                if (moved)
                {
                    Move(tempPos, Input.mousePosition - tempPos);
                }
                else
                {
                    StartMove();
                    Move(startPos, Input.mousePosition - startPos);
                }

                tempPos = Input.mousePosition;
                moved = true;
            }
        }
    }

    void Interaction(Touch[] touchs)
    {
        if (!pressed) return;

        if (!isExtend && touchs.Length == 1 && !touchs[0].position.Equals((Vector2)startPos))
        {
            Touch touch = touchs[0];
            if (IsMoving(touch))
            {
                if (moved)
                {
                    Move(tempPos, (Vector3)touch.position - tempPos);
                }
                else
                {
                    StartMove();
                    moved = true;
                    Move(startPos, (Vector3)touch.position - startPos);
                }

                tempPos = touch.position;
            }
        } else if (!moved && touchs.Length == 2)
        {
            Touch t1 = Input.touches[0];
            Touch t2 = Input.touches[1];

            float dd = Vector2.Distance(t1.position, t2.position) - startExtendD;
            if (IsExtended(dd))
            {
                if (!isExtend)
                {
                    StartExtend();
                }
                isExtend = true;
                Extend(t1.position, t2.position, dd);
            }

            tempD = dd;
        }
    }

    void EndInteraction()
    {
        pressed = false;

        if (moved && Time.time - startTime < swapLimit)
        {
            Swap(startPos, Input.mousePosition - startPos);
        }
        else if (moved)
        {
            float dx = Input.mousePosition.x - startPos.x;
            float dy = Input.mousePosition.y - startPos.y;

            StopMove(startPos, Input.mousePosition - startPos);
        }
        else
        {
            Tap(Input.mousePosition);
        }
    }

    void EndInteraction(Touch[] touchs)
    {
        pressed = false;

        if (touchs.Length == 1)
        {
            Touch touch = touchs[0];

            if (moved && Time.time - startTime < swapLimit)
            {
                Swap(startPos, (Vector3)touch.position - startPos);
            }
            else if (moved)
            {
                StopMove(startPos, (Vector3)touch.position - startPos);
            }
            else
            {
                Tap(startPos);
            }
        }
        else if (isExtend)
        {
            StopExtend();
        }
    }

    bool IsMoving()
    {
        if (moved) return true;

        float dx = Input.mousePosition.x - startPos.x;
        float dy = Input.mousePosition.y - startPos.y;
        float dz = Input.mousePosition.z - startPos.z;

        return (Mathf.Abs(dx) >= moveLimit || Mathf.Abs(dy) >= moveLimit || Mathf.Abs(dz) >= moveLimit);
    }

    bool IsMoving(Touch touch)
    {
        if (moved) return true;

        float dx = touch.position.x - startPos.x;
        float dy = touch.position.y - startPos.y;

        return (Mathf.Abs(dx) >= moveLimit || Mathf.Abs(dy) >= moveLimit);
    }

    bool IsExtended(float d) {
        return Mathf.Abs(d - tempD) > 0;
    }

    void Notify(string note)
    {
        Debug.Log(note);
    }

}
