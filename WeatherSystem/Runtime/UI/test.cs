using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	public class A
	{
		public int b = 1;
	}
	A a = new A();
	void OnEnable()
	{
		Debug.Log(a.b);
	}
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
