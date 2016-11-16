using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	void Start ()
	{
        DontDestroyOnLoad(gameObject);
	}

}
