﻿using UnityEngine;
using System.Collections;

namespace HomeBuilder.Designing
{
    public class Cube : MonoBehaviour
    {

        public void SetSize(float w, float h)
        {
            transform.localScale = new Vector3(w, 5, h);
        }

        public void SetPosition(float x, float y)
        {
            transform.localPosition = new Vector3(x, 0, y);
        }

        public Vector3 GetSize()
        {
            return GetComponent<MeshRenderer>().bounds.size;
        }

    }
}
