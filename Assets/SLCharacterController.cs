using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SLCharacterController : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    [SerializeField] float _moveSpeedPerSecond;
    [SerializeField] float _jumpVelocity;
    bool _jump;
    bool _cancelJump;

    void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _jump = true;
        if (Input.GetKeyUp(KeyCode.Space))
            _cancelJump = true;
    }

    void FixedUpdate()
    {
        var yVelocity = _rigidbody.velocity.y;
        
        yVelocity = HandleJump(yVelocity);

        var movement = Input.GetAxis("Horizontal") * _moveSpeedPerSecond;
        _rigidbody.velocity = new Vector2(movement, yVelocity);
    }

    float HandleJump(float yVelocity)
    {
        if (_jump)
        {
            yVelocity = _jumpVelocity;
            _jump = false;
        }

        if (!_cancelJump) return yVelocity;
        
        _cancelJump = false;
        if (yVelocity > 0)
            return -yVelocity;
        return yVelocity;
    }
}
