using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float MoveSpeed;
    public float MoveSmoothRatio;
    public float ZoomSpeed;
    public float ZoomSmoothRatio;

    private Camera _camera;
    private Vector3 _targetPosition;
    private float _targetFieldOfView;

    private void Start ()
    {
        _camera = gameObject.GetComponent<Camera>();
        _targetPosition = gameObject.transform.position;
        _targetFieldOfView = _camera.fieldOfView;
    }

    private void Update ()
    {
        Input();
    }

    private void Input()
    {
        HandleWsad();
        HandleMouseWheel();
    }

    private void HandleWsad()
    {
        var delta = Vector3.zero;

        if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            delta += new Vector3(MoveSpeed, 0, 0);
        }

        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            delta += new Vector3(-MoveSpeed, 0, 0);
        }

        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            delta += new Vector3(0, 0, MoveSpeed);
        }

        if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            delta += new Vector3(0, 0, -MoveSpeed);
        }

        if (delta != Vector3.zero)
        {
            _targetPosition = gameObject.transform.position + delta;
        }

        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _targetPosition, Time.deltaTime * MoveSmoothRatio);
    }

    private void HandleMouseWheel()
    {
        var wheelDelta = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        var delta = 0.0f;

        if (wheelDelta != 0)
        {
            if (wheelDelta > 0)
            {
                delta -= ZoomSpeed;
            }
            else if (wheelDelta < 0)
            {
                delta += ZoomSpeed;
            }

            if (delta != 0)
            {
                _targetFieldOfView = _camera.fieldOfView + delta;
            }
        }

        gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFieldOfView, Time.deltaTime * ZoomSmoothRatio);
    }
}
