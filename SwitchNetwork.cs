using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SwitchNetwork : NetworkBehaviour
{
    //ChangeSwitchLight gets called when switchBool changes
    [SyncVar(hook = "ChangeSwitchLight")]
    private bool switchBool;

    [Tooltip("GameObject for turning on Light")]
    public GameObject onLight;
    [Tooltip("GameObject for turning off Light")]
    public GameObject offLight;

    private bool setTargetPosition;

    private bool firstTimeDone;

   /// <summary>
   /// server checks if the switches have been pulled
   /// </summary>
    void Update()
    {
        if (gameObject.transform.eulerAngles.x >= 270 && gameObject.transform.eulerAngles.x <= 359)
        {
            if (!setTargetPosition)
            {
                JointSpring hingeSpring = GetComponent<HingeJoint>().spring;
                hingeSpring.targetPosition = 180;
                GetComponent<HingeJoint>().spring = hingeSpring;
                setTargetPosition = true;
            }
        }
        else if (gameObject.transform.eulerAngles.x >= 1 && gameObject.transform.eulerAngles.x <= 90)
        {
            if (setTargetPosition)
            {
                JointSpring hingeSpring = GetComponent<HingeJoint>().spring;
                hingeSpring.targetPosition = 0;
                GetComponent<HingeJoint>().spring = hingeSpring;
                setTargetPosition = false;
            }
        }

        if (isServer)
        {
            if (gameObject.transform.eulerAngles.x >= 270 && gameObject.transform.eulerAngles.x <= 359)
            {
                if (!switchBool)
                {
                    switchBool = true;
                }
            }
            else if (gameObject.transform.eulerAngles.x >= 1 && gameObject.transform.eulerAngles.x <= 90)
            {
                if (switchBool)
                {
                    switchBool = false;
                }
            }
        }
    }

    /// <summary>
    /// returns switchBool boolean
    /// </summary>
    /// <returns></returns>
    public bool GetBool()
    {
        return switchBool;
    }

    /// <summary>
    /// Changes the light when switchBool changes
    /// </summary>
    /// <param name="switchSetting"></param>
    private void ChangeSwitchLight(bool switchSetting)
    {
        switchBool = switchSetting;
        if (switchSetting)
        {
            onLight.SetActive(true);
            offLight.SetActive(false);
            if (!firstTimeDone)
            {
                firstTimeDone = true;
                if (isServer)
                {
                    GameManager.instance.scoreManager.UpdateResearchScore(100);
                }
                Level2Manager.instance.switchesThrown++;
                if (Level2Manager.instance.switchesThrown == 3)
                {
                    Level2Manager.instance.allSwitchesThrown = " ✔";
                }
                Level2Manager.instance.SetInfoboardText();
            }
        }
        else
        {
            onLight.SetActive(false);
            offLight.SetActive(true);
        }
    }
}
