using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SLDiverController : MonoBehaviour
{
    [SerializeField] Slider _airSlider;
    [SerializeField] float _descentSpeed;
    [SerializeField] float _ascentSpeed;
    [SerializeField] float _swimSpeed;
    [SerializeField] float _oxygenRecoveryRate = 20f;
    [SerializeField] float _oxygenDecayRate = 5f;
    
    Transform _transform;
    bool _hasAir;
    float _oxygen;
    int _maxOxygen = 100;
    
    const int AIR_LAYER = 8;
    const float OCEAN_SURFACE = -0.25f;

    void Awake()
    {
        _transform = transform;
        _oxygen = _maxOxygen;
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleAir();
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        move += Vector3.down * (_descentSpeed * Time.deltaTime);
        move += Vector3.right * (Input.GetAxis("Horizontal") * _swimSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up * (_ascentSpeed * Time.deltaTime);

        _transform.position += move;
        if (_transform.position.y > OCEAN_SURFACE)
            _transform.position = new Vector3(_transform.position.x, OCEAN_SURFACE, 0f);
    }

    void HandleAir()
    {
        _oxygen += (_hasAir ? _oxygenRecoveryRate : -_oxygenDecayRate) * Time.deltaTime;
        _oxygen = Mathf.Clamp(_oxygen, 0f, _maxOxygen);
        
        UpdateOxygenSlider();
    }

    void UpdateOxygenSlider() => _airSlider.value = (_oxygen <= 0 ? 0f : _oxygen / _maxOxygen);
    void OnTriggerExit2D(Collider2D other) { if (other.gameObject.layer == AIR_LAYER) _hasAir = false; }
    void OnTriggerEnter2D(Collider2D other) { if (other.gameObject.layer == AIR_LAYER) _hasAir = true; }
}
