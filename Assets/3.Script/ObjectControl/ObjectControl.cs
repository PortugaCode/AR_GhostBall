using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public enum ObjectState
{
    Default,
    Form1,
    Form2,
}

public class ObjectControl : MonoBehaviour
{
    [Header("Unit Form")]
    [SerializeField] private ObjectState state;
    public ObjectState p_State => state;

    [Header("Input Action")]
    [SerializeField] private InputAction singleTapAction;
    [SerializeField] private InputAction doubleTapAction;


    [Header("Layer Mask")]
    [SerializeField] private LayerMask touchableLayer;

    [Header("TargetData")]
    [SerializeField] private float range = 10f;
    [SerializeField] private ObjectControl target;
    [SerializeField] private Transform targetPoint;

    private Camera mainCamera;

    public Action Form1ChangeAction;
    public Action Form2ChangeAction;

    public bool isMerge;
    public bool isRight;

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
        isMerge = false;
        isRight = false;

        singleTapAction.performed -= _ => OnSingleTap();
        doubleTapAction.performed -= _ => OnDoubleTap();

        singleTapAction.Disable();
        doubleTapAction.Disable();
    }

    private void Update()
    {
        if (target.gameObject.activeInHierarchy == false || state != ObjectState.Form2)
        {
            isMerge = false;
            isRight = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= range)
        {
            if (target.state != ObjectState.Form2)
            {
                target.ChangeState(ObjectState.Form2);
                target.Form2ChangeAction?.Invoke();
            }

            if (transform.position.x > target.transform.position.x) isRight = true;
            else isRight = false;

            if(isRight == false)
            {
                target.transform.position = targetPoint.transform.position;
            }

            isMerge = true;
        }
        else isMerge = false;
    }

    private void OnSingleTap()
    {
        if(IsTouchingObject(out GameObject hitObject))
        {
            ChangeState(ObjectState.Form1);
            Form1ChangeAction?.Invoke();
        }
        
    }

    private void OnDoubleTap()
    {
        if (IsTouchingObject(out GameObject hitObject))
        {
            ChangeState(ObjectState.Form2);
            Form2ChangeAction?.Invoke();
        }
    }


    private bool IsTouchingObject(out GameObject hitObject)
    {
        hitObject = null;

        if (this.state == ObjectState.Form2) return false;

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

    private void ChangeState(ObjectState state)
    {
        this.state = state;
    }


}
