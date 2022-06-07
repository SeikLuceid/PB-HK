using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class SLCharacterController : MonoBehaviour
{
    [SerializeField] float _moveSpeedPerSecond;
    [SerializeField] float _jumpVelocity;
    [SerializeField] float _stepDelay;
    [SerializeField] AudioClip _stepFX;
    [SerializeField] AudioClip _jumpFX;
    [SerializeField] AudioClip _landFX;
    [SerializeField] LayerMask _groundMask;
    
    public bool _jump;
    public bool _isJumping;
    public bool _cancelJump;
    public bool _isGrounded;
    
    Rigidbody2D _rigidbody;
    AudioSource _audioSource;
    Coroutine _steppingFX;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _jump = true;
        if (Input.GetKeyUp(KeyCode.Space))
            _cancelJump = true;
    }

    void FixedUpdate()
    {
        var wasGrounded = _isGrounded;
        _isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 1.15f, _groundMask);

        var yVelocity = _rigidbody.velocity.y;
        
        if (!wasGrounded && _isGrounded && yVelocity < 0)
        {
            _isJumping = false;
            _audioSource.PlayOneShot(_landFX);
        }
        
        
        yVelocity = HandleJump(yVelocity);

        var movement = Input.GetAxis("Horizontal") * _moveSpeedPerSecond;
        var deltaVelocity = new Vector2(movement, yVelocity);
        _rigidbody.velocity = deltaVelocity;

        if (_steppingFX == null && _isGrounded && Mathf.Abs(movement) > 0.05f)
            _steppingFX = StartCoroutine(PlaySteps());
        if (_steppingFX != null && (Mathf.Abs(movement) < 0.05f || !_isGrounded))
        {
            StopCoroutine(_steppingFX);
            _steppingFX = null;
        }
    }

    float HandleJump(float yVelocity)
    {
        if (_isGrounded && !_isJumping && _jump)
        {
            yVelocity = _jumpVelocity;
            _isJumping = true;
            if (!_cancelJump)
                _audioSource.PlayOneShot(_jumpFX);
        }
        
        if (_cancelJump && _isJumping && yVelocity > 0)
            yVelocity = -yVelocity;
        
        _jump = false;
        _cancelJump = false;
        
        return yVelocity;
    }

    public IEnumerator PlaySteps()
    {
        WaitForSeconds stepDelay = new WaitForSeconds(_stepDelay);
        while (true)
        {
            _audioSource.PlayOneShot(_stepFX);
            yield return stepDelay;
        }
    }
}
