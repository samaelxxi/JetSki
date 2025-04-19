using UnityEngine;

public class PlayerControls : MonoBehaviour, IJetSkiControls
{
    [SerializeField] float _verticalSmoothing = 0.1f;
    [SerializeField] float _horizontalSmoothing = 0.1f;
    [SerializeField, Range(0, 1)] float _deadZone = 0.2f;

    PlayerInputs _playerInputs;
    Vector2 _moveInput;

    float _xSmoothVelocity;
    float _ySmoothVelocity;

    public void Awake()
    {
        _playerInputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
    }

    private void OnDisable()
    {
        _playerInputs.Disable();
    }

    public void Update()
    {
        var input = _playerInputs.Player.Move.ReadValue<Vector2>();

        if (input.magnitude < _deadZone)
        {
            _moveInput = Vector2.zero;
            _xSmoothVelocity = 0f;
            _ySmoothVelocity = 0f;
        }
        else
        {
            _moveInput.x = Mathf.SmoothDamp(_moveInput.x, input.x, ref _xSmoothVelocity, _horizontalSmoothing);
            _moveInput.y = Mathf.SmoothDamp(_moveInput.y, input.y, ref _ySmoothVelocity, _verticalSmoothing);
        }

        // Debug.Log(_moveInput);
    }

    public float GetHorizontalInput()
    {
        return _moveInput.x;
    }

    public float GetVerticalInput()
    {
        return _moveInput.y;
    }
}