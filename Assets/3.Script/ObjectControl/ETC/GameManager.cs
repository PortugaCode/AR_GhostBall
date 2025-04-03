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
    [SerializeField] private string summon_path;

    public void ChangeSimultaneousNum(int num)
    {
        imageTrackerFrameFilter.SimultaneousNum = num;
    }

    public void TrySummon(bool canTrySummon)
    {
        if(canTrySummon)
        {
            summonTargetConroller.ImageFileSource.Path = summon_path;
            return;
        }

        summonTargetConroller.ImageFileSource.Path = string.Empty;
    }

    public void OpenPopUp(bool isRed)
    {
        if(isRed == true)
        {
            //todo
            return;
        }
        //todo
    }
}
