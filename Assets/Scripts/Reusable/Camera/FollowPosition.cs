using UnityEngine;

public class FollowPosition : MonoBehaviour
{
	public Transform target;
	private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
	    offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
	    transform.position = target.position + offset;
    }
}
