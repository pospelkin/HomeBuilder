using System;
using UnityEngine;

namespace HomeBuilder
{
    public class GCameraController : MonoBehaviour
    {

        public static GCameraController instance;

        public GCamera gCamera;

        // Define if camera is moving or fixed to a position
        public bool fixedCam = true;

        public float tSensibility = 1f,
                        tRotateSensibility = 0.5f,
                        tRotateRatio = 0.5f,
                        tZoomRatio = 0.2f,

                        mZoomRatio = 5,
                        mRotateRatio = 5;

        private float oldAngle = 0;
        private bool doubleTouchStarted = false;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            gCamera = SphereCamera.instance;

            gCamera.Reset();
            gCamera.UpdatePosition();
        }

        void Update()
        {
            if (!enabled) return;

            ControlMouse();
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY

            ControlTouch();

#endif

#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)

		ControlMouse ();

#endif
        }

        //private void ControlTouch()
        //{
        //    if (Input.touchCount == 2)
        //    {
        //        Touch touchZero = Input.GetTouch(0);
        //        Touch touchOne = Input.GetTouch(1);

        //        if (!doubleTouchStarted)
        //        {
        //            Vector2 dPos = touchZero.position - touchOne.position;
        //            oldAngle = getTwistAngle(touchZero, touchOne);
        //        }
        //        doubleTouchStarted = true;

        //        float newAngle = getTwistAngle(touchZero, touchOne);
        //        float twist = Mathf.DeltaAngle(newAngle, oldAngle) * 180 / Mathf.PI;
        //        oldAngle = newAngle;

        //        float zoom = getZoomLength(touchZero, touchOne);

        //        float absZ = Mathf.Abs(zoom * 1f);

        //        if (Mathf.Abs(twist) > tRotateSensibility)
        //        {
        //            gCamera.RotateHorizontal(-twist);

        //            gCamera.UpdatePosition();
        //        }
        //        else if (absZ > tSensibility)
        //        {
        //            if (fixedCam)
        //            {
        //                gCamera.RotateVertical(zoom * tRotateRatio);
        //            }
        //            else {
        //                gCamera.Zoom(zoom * tZoomRatio);
        //            }

        //            gCamera.UpdatePosition();
        //        }

        //    }
        //    else if (Input.touchCount == 1 && fixedCam == false)
        //    {
        //        doubleTouchStarted = false;

        //        Touch touch = Input.GetTouch(0);

        //        if (Mathf.Abs(touch.deltaPosition.x) > tSensibility
        //            || Mathf.Abs(touch.deltaPosition.y) > tSensibility)
        //        {
        //            gCamera.Move(touch.deltaPosition.x / 25, touch.deltaPosition.y / 25);
        //        }
        //        gCamera.UpdatePosition();
        //    }
        //    else {
        //        doubleTouchStarted = false;
        //    }
        //}

        private void ControlTouch()
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (!doubleTouchStarted)
                {
                    Vector2 dPos = touchZero.position - touchOne.position;
                    oldAngle = getTwistAngle(touchZero, touchOne);
                }
                doubleTouchStarted = true;

                float newAngle = getTwistAngle(touchZero, touchOne);
                float twist = Mathf.DeltaAngle(newAngle, oldAngle) * 180 / Mathf.PI;
                oldAngle = newAngle;

                float zoom = getZoomLength(touchZero, touchOne);

                float absZ = Mathf.Abs(zoom * 1f);

                if (Mathf.Abs(twist) > tRotateSensibility)
                {
                    gCamera.RotateHorizontal(-twist);

                    gCamera.UpdatePosition();
                }
                else if (absZ > tSensibility)
                {
                    if (fixedCam)
                    {
                        gCamera.RotateVertical(zoom * tRotateRatio);
                    }
                    else
                    {
                        gCamera.Zoom(zoom * tZoomRatio);
                    }

                    gCamera.UpdatePosition();
                }

            }
            else if (Input.touchCount == 1)
            {
                doubleTouchStarted = false;

                Touch touch = Input.GetTouch(0);

                //if (fixedCam == false)
                //{
                //    if (Mathf.Abs(touch.deltaPosition.x) > tSensibility
                //        || Mathf.Abs(touch.deltaPosition.y) > tSensibility)
                //    {
                //        gCamera.Move(touch.deltaPosition.x / 25, touch.deltaPosition.y / 25);
                //    }
                //    gCamera.UpdatePosition();
                //}
                //else
                //{

                gCamera.RotateHorizontal(touch.deltaPosition.x * tRotateRatio);
                gCamera.RotateVertical(touch.deltaPosition.y * tRotateRatio);

                gCamera.UpdatePosition();
                //}
            }
            else
            {
                doubleTouchStarted = false;
            }
        }

        private float getTwistAngle(Touch touchZero, Touch touchOne)
        {
            Vector2 dPos = touchZero.position - touchOne.position;
            return Mathf.Atan2(dPos.y, dPos.x);
        }

        private float getZoomLength(Touch touchZero, Touch touchOne)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            return deltaMagnitudeDiff;
        }

        private float getRotateLength(Touch touchZero, Touch touchOne)
        {
            return Mathf.Max(Math.Abs(touchZero.deltaPosition.x), Math.Abs(touchOne.deltaPosition.x)) == Math.Abs(touchZero.deltaPosition.x) ?
                touchZero.deltaPosition.x : touchOne.deltaPosition.x;
        }

        private void ControlMouse()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                if (fixedCam)
                {
                    gCamera.RotateVertical(scroll * mRotateRatio);
                }
                else {
                    gCamera.Zoom(scroll * mZoomRatio);
                }

                gCamera.UpdatePosition();
            }
            else if (Input.GetMouseButton(0) && fixedCam == false)
            {
                float x = Input.GetAxis("Mouse X"),
                        y = Input.GetAxis("Mouse Y");

                if (x != 0 || y != 0)
                {
                    gCamera.Move(x, y);

                    gCamera.UpdatePosition();
                }
            }
            else if (Input.GetMouseButton(1))
            {
                float move = Input.GetAxis("Mouse X");

                if (move != 0)
                {
                    gCamera.RotateHorizontal(move * mRotateRatio);

                    gCamera.UpdatePosition();
                }
            }
        }

    }

}