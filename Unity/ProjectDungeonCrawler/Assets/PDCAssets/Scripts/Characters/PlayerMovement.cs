using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    Rigidbody rb;

    Vector3 direction;
    float verticalInput;
    Vector3 rotation;
    Vector3 camRotation;

    [SerializeField] float speed = 5f;
    [SerializeField] Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool SetMove(float _xInput, float _yInput)
    {
        verticalInput = _yInput;

        Vector3 _moveHorizontal = transform.right * _xInput;
        Vector3 _moveVertical = transform.forward * _yInput;

        Vector3 _dir = (_moveHorizontal + _moveVertical).normalized * speed;

        direction = _dir;

        if (_dir != Vector3.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetRotation(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void SetCamRotation(Vector3 _rotation)
    {
        camRotation = _rotation;
    }

    public void Jump(float _height)
    {
        Vector3 _jumpVector = Vector3.up * _height;
        rb.velocity += _jumpVector;
    }

    void FixedUpdate()
    {
        PreformMovement();
        PreformRotation();
    }

    void PreformMovement()
    {
        if (direction != Vector3.zero)
        {
            Vector3 _direction = direction;
            if(verticalInput < 0)
            {
                _direction /= 2;
            }
            rb.MovePosition(rb.position + _direction * Time.fixedDeltaTime);
        }
    }

    void PreformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            cam.transform.Rotate(camRotation);
        }
        else
        {
            Debug.LogError("Camera in PlayerMovement component is not setup!");
        }
    }
}
