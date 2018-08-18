using UnityEngine;

public class CameraEntity : MonoBehaviour
{
    public float MoveSpeed;
    public float FastMoveSpeed;
    public float ZoomSpeed;
    public float RotationSpeed;

    private Vector3 _rotationAngles;

    private void Start()
    {
        _rotationAngles = gameObject.transform.eulerAngles;
    }

    private void Update()
    {
        Input();
    }

    private void Input()
    {
        HandleWsad();
        HandleMouseWheel();
        HandleCameraRotation();
    }

    private void HandleWsad()
    {
        var delta = Vector3.zero;
        var moveSpeed = UnityEngine.Input.GetKey(KeyCode.LeftShift) ? FastMoveSpeed : MoveSpeed;

        if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            delta += new Vector3(moveSpeed, 0, 0);
        }

        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            delta += new Vector3(-moveSpeed, 0, 0);
        }

        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            delta += new Vector3(0, 0, moveSpeed);
        }

        if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            delta += new Vector3(0, 0, -moveSpeed);
        }

        delta *= Time.deltaTime;
        gameObject.transform.Translate(delta, Space.Self);
    }

    private void HandleMouseWheel()
    {
        var wheelDelta = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        if (wheelDelta != 0)
        {
            var delta = new Vector3(0, 0, 1) * wheelDelta * Time.deltaTime * ZoomSpeed;
            gameObject.transform.Translate(delta, Space.Self);
        }
    }

    private void HandleCameraRotation()
    {
        if (UnityEngine.Input.GetMouseButton(1))
        {
            var xRotation = UnityEngine.Input.GetAxis("Mouse X") * Time.deltaTime * RotationSpeed;
            var yRotation = -UnityEngine.Input.GetAxis("Mouse Y") * Time.deltaTime * RotationSpeed;

            _rotationAngles += new Vector3(yRotation, xRotation, 0);
            gameObject.transform.eulerAngles = _rotationAngles;
        }
    }
}
