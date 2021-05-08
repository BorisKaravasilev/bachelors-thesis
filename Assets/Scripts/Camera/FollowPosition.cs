using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPosition : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float cameraZoomSpeed = 1f;
	[SerializeField] private float minFOV = 15f;
	[SerializeField] private float maxFOV = 70f;

	private Vector3 offset;
	private Camera followingCamera;
	private float startingFOV;

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
	    followingCamera.fieldOfView -= Input.mouseScrollDelta.y * cameraZoomSpeed;
	}

    void FixedUpdate()
    {
	    HandleZoom();
	}

    private void HandleZoom()
    {
	    bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

		if (shiftPressed && followingCamera.fieldOfView > minFOV)
	    {
		    followingCamera.fieldOfView -= cameraZoomSpeed;
		}

		if (ctrlPressed && followingCamera.fieldOfView < maxFOV)
		{
			followingCamera.fieldOfView += cameraZoomSpeed;
		}
	}
}
