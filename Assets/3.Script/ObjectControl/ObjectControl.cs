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
    Form3,
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


    [Header("Form1 Effect")]
    [SerializeField] private GameObject form1_Effect;

    [Header("Form2 Effect")]
    [SerializeField] private GameObject form2_Effect;
    [SerializeField] private GameObject cardShadow;
    [SerializeField] private Renderer cardRenderer;
    [SerializeField] private Color cardColor;
    [SerializeField] private float cycleDuration = 100f;
    private bool isIncreasing = true;

    [Header("Form3 Effect")]
    [SerializeField] private GameObject form3_Effect_1;
    [SerializeField] private GameObject form3_Effect_2;

    [Header("Summon Card")]
    [SerializeField] private GameObject summon_CardShadow;
    [SerializeField] private Renderer summon_CardRenderer;
    [SerializeField] private Color summon_CardColor;
    [SerializeField] private float summon_CycleDuration = 500f;
    private bool summon_isIncreasing = true;

    [Header("Judgment")]
    public bool isMerge;
    public bool isRight;

    private Camera mainCamera;
    public Action Form1ChangeAction;
    public Action Form2ChangeAction;


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
        ChangeState(ObjectState.Default);
        isMerge = false;
        isRight = false;

        singleTapAction.performed -= _ => OnSingleTap();
        doubleTapAction.performed -= _ => OnDoubleTap();

        singleTapAction.Disable();
        doubleTapAction.Disable();
    }

    private void Update()
    {
        BlinkCard(state == ObjectState.Form3);


        if ((int)state >= (int)ObjectState.Form2)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= range && target.gameObject.activeInHierarchy == true)
            {

                if (transform.position.x > target.transform.position.x) isRight = true;
                else isRight = false;

                if (isRight == false)
                {
                    target.transform.position = targetPoint.transform.position;
                    target.isRight = true;
                }

                if ((int)target.state < (int)ObjectState.Form2)
                {
                    target.Form2ChangeAction?.Invoke();
                    target.ChangeState(ObjectState.Form3);
                }

                isMerge = true;
                ChangeState(ObjectState.Form3);
            }
            else if (state == ObjectState.Form3)
            {
                ChangeState(ObjectState.Form2);
                isMerge = false;
            }
        }


    }


    //==================================================================
    //
    // 구현 메서드
    //
    //==================================================================

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

        if (this.state == ObjectState.Form2 || this.state == ObjectState.Form3) return false;

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
        if (this.state == state) return;

        this.state = state;

        Debug.Log($"Change State : {state}");

        switch(state)
        {
            case ObjectState.Form1:
                cardShadow.SetActive(false);
                summon_CardShadow.SetActive(false);

                form3_Effect_1.SetActive(false);
                form3_Effect_2.SetActive(false);

                form1_Effect.SetActive(true);
                break;

            case ObjectState.Form2:
                form3_Effect_1.SetActive(false);
                form3_Effect_2.SetActive(false);
                summon_CardShadow.SetActive(false);

                form1_Effect.SetActive(true);
                form2_Effect.SetActive(true);
                cardShadow.SetActive(true);
                transform.localPosition = Vector3.zero;
                break;

            case ObjectState.Form3:
                form1_Effect.SetActive(false);
                form2_Effect.SetActive(false);

                if (isRight == false)
                {
                    form3_Effect_1.SetActive(true);
                    form3_Effect_2.SetActive(true);
                    summon_CardShadow.SetActive(true);
                }
                cardShadow.SetActive(false);
                break;

            case ObjectState.Default:
                cardShadow.SetActive(false);
                summon_CardShadow.SetActive(false);

                form1_Effect.SetActive(false);
                form2_Effect.SetActive(false);
                form3_Effect_1.SetActive(false);
                form3_Effect_2.SetActive(false);

                break;
        }
    }

    // 카드 깜빡이는 기능
    private void BlinkCard(bool isSummon)
    {
        
        if (isSummon)
        {
            if (summon_CardShadow.activeInHierarchy == true)
            {

                float alphaChangeSpeed = 255.0f / summon_CycleDuration;

                if (summon_isIncreasing)
                {
                    summon_CardColor.a += alphaChangeSpeed * Time.deltaTime;
                    if (summon_CardColor.a >= 1f)
                    {
                        summon_CardColor.a = 1f;
                        summon_isIncreasing = false;
                    }
                }
                else
                {
                    summon_CardColor.a -= alphaChangeSpeed * Time.deltaTime;
                    if (summon_CardColor.a <= 0f)
                    {
                        summon_CardColor.a = 0f;
                        summon_isIncreasing = true;
                    }
                }

                summon_CardRenderer.material.color = summon_CardColor;
            }
        }
        else
        {
            if (cardShadow.activeInHierarchy == true)
            {
                float alphaChangeSpeed = 255.0f / cycleDuration;

                if (isIncreasing)
                {
                    cardColor.a += alphaChangeSpeed * Time.deltaTime;
                    if (cardColor.a >= 1f)
                    {
                        cardColor.a = 1f;
                        isIncreasing = false;
                    }
                }
                else
                {
                    cardColor.a -= alphaChangeSpeed * Time.deltaTime;
                    if (cardColor.a <= 0f)
                    {
                        cardColor.a = 0f;
                        isIncreasing = true;
                    }
                }

                cardRenderer.material.color = cardColor;
            }
        }
        
        
    }
    
}
