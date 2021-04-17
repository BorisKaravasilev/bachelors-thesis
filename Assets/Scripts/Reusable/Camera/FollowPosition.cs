using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPosition : MonoBehaviour
{
	public Transform target;
	private Vector3 offset;
	private Camera followingCamera;

    // Start is called before the first frame update
    void Start()
    {
	    offset = transform.position - target.position;
	    followingCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
	    transform.position = target.position + offset;
    }

    void OnGUI()
    {
	    followingCamera.fieldOfView -= Input.mouseScrollDelta.y;
    }
}
