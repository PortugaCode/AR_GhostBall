using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rotatable : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputAction pressed, axis;

    [Header("Rotation Speed")]
    [SerializeField] private float speed = 1;

    [Header("Inverted")]
    [SerializeField] private bool inverted;

    //private Transform cam;
    private Quaternion baseRotation;
    private Vector2 rotation;
    private bool isRotateAllowed;

    private Coroutine rotationRoutine;



    private void Awake()
    {
        //cam = Camera.main.transform;
        baseRotation = gameObject.transform.rotation;

    }

    private void OnEnable()
    {
        pressed.performed += _ => { StartRoutine(); };
        pressed.canceled += _ => { isRotateAllowed = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };

        pressed.Enable();
        axis.Enable();
    }

    private void OnDisable()
    {
        if (rotationRoutine != null) StopCoroutine(rotationRoutine);

        transform.rotation = baseRotation;
        pressed.performed -= _ => { StartRoutine(); };
        pressed.canceled -= _ => { isRotateAllowed = false; };
        axis.performed -= context => { rotation = context.ReadValue<Vector2>(); };

        pressed.Disable();
        axis.Disable();
    }

    private void StartRoutine()
    {
        if(rotationRoutine != null) StopCoroutine(rotationRoutine);

        rotationRoutine = StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        isRotateAllowed = true;

        while (isRotateAllowed)
        {
            //로테이션 로직
            rotation *= speed;
            transform.Rotate(Vector3.up*(inverted? 1 : -1), rotation.x, Space.Self);

            // 위 아래 로테이션 로직
            //transform.Rotate(cam.right*(inverted? -1 : 1), rotation.y, Space.Self);
            yield return null;
        }

        while(!isRotateAllowed)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, (speed * 5) * Time.deltaTime);
            yield return null;
        }
    }

    private void OnForm1_Rotation()
    {
       
    }

    private void OnForm2_Rotation()
    {

    }
}

