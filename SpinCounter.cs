using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class SpinCounter : NetworkBehaviour
{
    //Spin gets called when spinCount changes, amount of spins done
    [SyncVar(hook = "Spin")]
    public int spinCount;
    //Check if a slotmachine is done
    [SyncVar]
    public bool spinDone;
    
    //timer for Spinning
    public Timer spinTimer;

    //The amount of spins needed to get it to "777"
    private const int maxSpinCount = 3;

    //Animators from the slotmachine
    public Animator[] animators;

    private void Start()
    {
        spinTimer = new Timer();
    }

    /// <summary>
    /// Server checks if timer is done so players can spin again, checks if the arm has been pulled and calls a command for spinning
    /// </summary>
    private void Update()
    {
        if (isServer)
        {
            if (spinTimer.TimerDone() && spinTimer.IsActive)
            {
                spinTimer.StopTimer();
                RpcLeverGrabbable();
            }
            if (spinCount == maxSpinCount)
            {
                spinDone = true;
            }
            else
            {
                spinDone = false;
            }
        }

        if (gameObject.transform.eulerAngles.x > 329)
        {
            GetComponent<Slot>().isGrabbable = false;
            if (GetComponent<Slot>().GetGrabbingObject() != null)
            {
                GetComponent<Slot>().GetGrabbingObject().GetComponent<VRTK_InteractGrab>().ForceRelease();
            }
            GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdUngrabLever(gameObject);
        }
    }
    /// <summary>
    /// Plays the animations for each spin
    /// </summary>
    /// <param name="theSpinCount"></param>
    public void Spin(int theSpinCount)
    {
        spinCount = theSpinCount;
        StartCoroutine(StaggeredSpin(theSpinCount));
    }

    /// <summary>
    /// returns spinDone boolean
    /// </summary>
    /// <returns></returns>
    public bool GetBool()
    {
        return spinDone;
    }

    private IEnumerator StaggeredSpin(int theSpinCount)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetTrigger("Roll" + theSpinCount);
            yield return new WaitForSeconds(1);
        }
    }

    [ClientRpc]
    public void RpcUngrabLever()
    {
        GetComponent<Slot>().isGrabbable = false;
        if (GetComponent<Slot>().GetGrabbingObject() != null)
        {
            GetComponent<Slot>().GetGrabbingObject().GetComponent<VRTK_InteractGrab>().ForceRelease();
        }
    }

    [ClientRpc]
    private void RpcLeverGrabbable()
    {
        GetComponent<Slot>().isGrabbable = true;
    }
}