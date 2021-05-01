using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInSeconds : MonoBehaviour
{
	[SerializeField] private float seconds = 1f;

	private float timeAtStart = 0f;

    // Start is called before the first frame update
    void Start()
    {
	    timeAtStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
	    if (Time.realtimeSinceStartup - timeAtStart > seconds)
	    {
			gameObject.SetActive(false);
	    }
    }
}
