using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Botton : MonoBehaviour
{
	public void changemenuscene(string scenename)
	{
			Application.LoadLevel(scenename);
	}
}