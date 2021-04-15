using UnityEngine;

public class FollowPosition : MonoBehaviour
{
	public Transform target;
	private Vector3 offset;
	private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
	    offset = transform.position - target.position;
	    camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
	    transform.position = target.position + offset;
    }

    void OnGUI()
    {
	    camera.fieldOfView -= Input.mouseScrollDelta.y;
    }
}
