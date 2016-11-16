using UnityEngine;
using System.Collections;
using HomeBuilder.Core;

public class Loginner : MonoBehaviour
{
    public ScreenController screen;

	public void SignIn()
    {
        Master.SLIDE = true;
        Master.FLOW  = true;

        screen.OpenHistory();
    }
}
