using System;
using System.Collections;

namespace HomeBuilder
{
    public interface GCamera
    {

        void UpdatePosition();
        void Reset();

        void Zoom(float d);
        void RotateVertical(float d);
        void RotateHorizontal(float d);

        void Move(float dx, float dy);

    }
}