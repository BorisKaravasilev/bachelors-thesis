using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		transform.RotateAround(transform.position, transform.up, Time.deltaTime * 2f);
	}
}
