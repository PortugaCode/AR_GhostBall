using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyar;
using UnityEngine.UIElements.Experimental;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    #region [ΩÃ±€≈Ê]
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //=========================================

    [SerializeField] private ImageTrackerFrameFilter imageTrackerFrameFilter;
    [SerializeField] private ImageTargetController summonTargetConroller;
    [SerializeField] private GameObject Target;

    [Header("SimultaneousNum")]
    [SerializeField] private int simultaneousNum_now = 1;

    [Header("Panel")]
    [SerializeField] private GameObject redPanel;
    [SerializeField] private GameObject bluePanel;


    public void ChangeSimultaneousNum(int num)
    {
        imageTrackerFrameFilter.SimultaneousNum = num;

        simultaneousNum_now = num;
    }

    public void TrySummon(bool canTrySummon)
    {
        if(canTrySummon == true)
        {
            summonTargetConroller.Tracker = imageTrackerFrameFilter;
            return;
        }
        if (canTrySummon == false)
        {
            summonTargetConroller.Tracker = null;
            return;
        }


    }

    public void OpenPopUp(bool isRed)
    {
        if(isRed == true)
        {
            //todo
            redPanel.SetActive(true);
            return;
        }
        //todo
        bluePanel.SetActive(true);
    }

    public bool CheckTarget()
    {
        return Target.activeInHierarchy;
    }

    public void ResetSetting()
    {
        redPanel.SetActive(false);
        bluePanel.SetActive(false);
        ChangeSimultaneousNum(1);
        TrySummon(canTrySummon: false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
