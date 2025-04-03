using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyar;
using UnityEngine.UIElements.Experimental;
using System.Text;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    #region [싱글톤]
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

    // 감지 가능 오브젝트 갯수 변경 메서드
    public void ChangeSimultaneousNum(int num)
    {
        imageTrackerFrameFilter.SimultaneousNum = num;

        simultaneousNum_now = num;
    }

    //소환 가능한지 여부 변경 메서드
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

    // 색에 따른 UI 변경 메서드
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

    // Summon Target이 활성화 됐는지 체크하는 메서드
    public bool CheckTarget()
    {
        return Target.activeInHierarchy;
    }

    // 기본 세팅
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
