using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class ObjectControl : MonoBehaviour
{
    public enum ObjectState
    {
        Default,
        Form1,
        Form2,
    }

    [Header("Unit Form")]
    [SerializeField] private ObjectState state;

    [Header("Input Action")]
    [SerializeField] private InputAction singleTapAction;
    [SerializeField] private InputAction doubleTapAction;


    [Header("Layer Mask")]
    [SerializeField] private LayerMask touchableLayer; // 감지할 오브젝트 레이어

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        singleTapAction.Enable();
        doubleTapAction.Enable();

        singleTapAction.performed += _ => OnSingleTap();
        doubleTapAction.performed += _ => OnDoubleTap();
    }

    private void OnDisable()
    {
        this.state = ObjectState.Default;

        singleTapAction.performed -= _ => OnSingleTap();
        doubleTapAction.performed -= _ => OnDoubleTap();

        singleTapAction.Disable();
        doubleTapAction.Disable();
    }

    private void OnSingleTap()
    {
        if(IsTouchingObject(out GameObject hitObject))
        {
            ChangeState(ObjectState.Form1);
        }
        
    }

    private void OnDoubleTap()
    {
        if (IsTouchingObject(out GameObject hitObject))
        {
            ChangeState(ObjectState.Form2);
        }
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

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f); // Ray를 그려서 확인

        

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchableLayer))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }
        return false;
    }

    private void ChangeState(ObjectState state)
    {
        if (this.state == ObjectState.Form2) return; 
        this.state = state;
    }


}
