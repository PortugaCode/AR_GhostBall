using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Rotatable : MonoBehaviour
{
    [Header("Pivot")]
    [SerializeField] private Transform pivot;

    [Header("Input System")]
    [SerializeField] private InputAction pressed, axis;

    [Header("Rotation Speed")]
    [SerializeField] private float speed = 1;

    [Header("Inverted")]
    [SerializeField] private bool inverted;

    [Header("Object Controller")]
    [SerializeField] private ObjectControl objectControl;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask touchableLayer;


    private Camera mainCamera;
    private Quaternion baseRotation;
    private Vector3 basePosition;
    private Vector2 rotation;
    private bool isRotateAllowed;

    private Coroutine rotationRoutine;
    private Coroutine rotationRoutine_Form1;
    private Coroutine rotationRoutine_Form2;



    private void Awake()
    {
        mainCamera = Camera.main;
        baseRotation = gameObject.transform.localRotation;
        basePosition = gameObject.transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localRotation = baseRotation;
        transform.localPosition = basePosition;
        pivot.localRotation = Quaternion.Euler(0, 0f, 0f);

        objectControl.Form1ChangeAction += OnForm1Change;
        objectControl.Form2ChangeAction += OnForm2Change;

        pressed.performed += _ => { StartRoutine(); };
        pressed.canceled += _ => { isRotateAllowed = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };

        pressed.Enable();
        axis.Enable();
    }

    private void OnDisable()
    {

        StopAllCoroutines();
        transform.localRotation = baseRotation;
        transform.localPosition = basePosition;
        pivot.localRotation = Quaternion.Euler(0, 0f, 0f);


        objectControl.Form1ChangeAction -= OnForm1Change;
        objectControl.Form2ChangeAction -= OnForm2Change;
        pressed.performed -= _ => { StartRoutine(); };
        pressed.canceled -= _ => { isRotateAllowed = false; };
        axis.performed -= context => { rotation = context.ReadValue<Vector2>(); };

        pressed.Disable();
        axis.Disable();
    }

    //==================================================================
    //
    // 구현 메서드
    //
    //==================================================================
    private void OnForm1Change()
    {
        if (rotationRoutine_Form1 != null) StopCoroutine(rotationRoutine_Form1);

        rotationRoutine_Form1 = StartCoroutine(Form1_Rotation());
    }

    private void OnForm2Change()
    {
        if (rotationRoutine_Form2 != null) StopCoroutine(rotationRoutine_Form2);

        rotationRoutine_Form2 = StartCoroutine(Form2_Rotation());
    }


    private void StartRoutine()
    {
        if (IsTouchingObject(out GameObject hitObject) == false) return;

        if(rotationRoutine != null) StopCoroutine(rotationRoutine);

        rotationRoutine = StartCoroutine(Rotate());
    }

    private bool IsTouchingObject(out GameObject hitObject)
    {
        hitObject = null;

        Vector2 touchPosition;
        if (Touchscreen.current != null)
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            touchPosition = Mouse.current.position.ReadValue();
        }

        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchableLayer))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }
        return false;
    }


    // 스와이프 제스처에 따른 로테이션 기능 메서드
    private IEnumerator Rotate()
    {
        isRotateAllowed = true;

        while (isRotateAllowed)
        {
            // 로테이션 로직
            rotation *= speed;

            if(objectControl.p_State == ObjectState.Default)
            {
                transform.Rotate(Vector3.up * (inverted ? 1 : -1), rotation.x, Space.World);
            }
            else if(objectControl.p_State == ObjectState.Form1)
            {
                transform.Rotate(Vector3.forward * (inverted? -1 : 1), rotation.x, Space.Self);
            }


            yield return null;
        }
        // 터치가 끊겼을 때 원래 로테이션으로 돌아가는 로직
        while(!isRotateAllowed)
        {

            if (objectControl.p_State == ObjectState.Default)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, baseRotation, (speed * 5) * Time.deltaTime);
            }
            else if (objectControl.p_State == ObjectState.Form1)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(180, 180, 0), (speed * 5) * Time.deltaTime);
            }
            
            yield return null;
        }
    }

    private IEnumerator Form1_Rotation()
    {
        while(objectControl.p_State == ObjectState.Form1)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(180, 180, 0), (speed * 5) * Time.deltaTime);
            yield return null;
        }

        yield break;
    }

    private IEnumerator Form2_Rotation()
    {
        while (true)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(180, 180, 0), (speed * 5) * Time.deltaTime);


            if (objectControl.isMerge)
            {
                pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(0, 90 * (objectControl.isRight ? 1 : -1), 0), (speed * 5) * Time.deltaTime);
            }
            else
            {
                pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(0, 0, 0), (speed * 5) * Time.deltaTime);
            }

            yield return null;
        }


    }
}

