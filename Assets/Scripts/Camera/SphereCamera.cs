using UnityEngine;
using System.Collections;

namespace HomeBuilder
{
    public class SphereCamera : MonoBehaviour, GCamera
    {

        public static SphereCamera instance;

        public static void Disable() { if (instance != null) instance.enabled = false; }
        public static void Enable() { if (instance != null) instance.enabled = true; }

        [HideInInspector]
        public Transform camTransform;

        // Some constants for resetting
        public float r0 = 40,
                                x0 = 0,
                                y0 = 0,
                                z0 = 0,
                                // Up and down
                                tita0 = 45,
                                // Right and left
                                phi0 = 90;

        // Changes applied to starting values
        private float r = 0,
        x = 0,
        y = 0,
        z = 0,
        tita = 0,
        phi = 0;


        // Changes applied to starting values
        public float rMax = 50,
                        rMin = 20,
                        xMax = 8,
                        xMin = -8,
                        zMax = 5,
                        zMin = -5;

        // The world is turning around this point
        private Vector3 centerPoint;

        void Awake()
        {
            instance = this;
            camTransform = Camera.main.transform;
        }

        public void Reset()
        {
            setCenterPoint(x0, y0, z0);
            setRadius(r0);
            SetPhi(phi0);
            SetTita(tita0);

            camTransform.localPosition = new Vector3(0, 0, 0);
        }

        public void UpdatePosition()
        {
            transform.localPosition = GetPosition();
            transform.LookAt(centerPoint);

            camTransform.LookAt(centerPoint);
        }

        private Vector3 GetPosition()
        {
            return new Vector3(GetX(), GetY(), GetZ());
        }

        private float GetX()
        {
            return centerPoint.x + r * Mathf.Sin(translateDegreesToRadians(tita)) * Mathf.Sin(translateDegreesToRadians(phi));
        }

        private float GetY()
        {
            return centerPoint.y + r * Mathf.Cos(translateDegreesToRadians(tita));
        }

        private float GetZ()
        {
            return centerPoint.z + r * Mathf.Sin(translateDegreesToRadians(tita)) * Mathf.Cos(translateDegreesToRadians(phi));
        }

        public void SetTita(float angle)
        {
            if (angle < 0 || angle > 90)
                return;

            tita = angle;
        }

        public float GetTita()
        {
            return tita;
        }

        public void SetPhi(float angle)
        {
            phi = angle;
        }

        public float GetPhi()
        {
            return tita;
        }

        public void setRadius(float nr)
        {
            if (nr < rMin || nr > rMax)
                return;

            r = nr;
        }

        private void setCenterPoint(float nx, float ny, float nz)
        {
            if (nx < xMin || nx > xMax || nz < zMin || nz > zMax)
                return;

            x = nx;
            y = ny;
            z = nz;

            centerPoint = new Vector3(x, y, z);
        }

        private float translateDegreesToRadians(float degrees)
        {
            return degrees * Mathf.PI / 180;
        }

        public void Zoom(float dr)
        {
            setRadius(r + dr);
        }

        public void RotateVertical(float d)
        {
            SetTita(tita + d);
        }

        public void RotateHorizontal(float d)
        {
            SetPhi(phi + d);
        }

        public void Move(float dx, float dy)
        {
            float x1 = dx * Mathf.Cos(translateDegreesToRadians(phi)) + dy * Mathf.Sin(translateDegreesToRadians(phi));
            float y1 = -dx * Mathf.Sin(translateDegreesToRadians(phi)) + dy * Mathf.Cos(translateDegreesToRadians(phi));

            setCenterPoint(centerPoint.x + x1, centerPoint.y, centerPoint.z + y1);
        }

    }
}