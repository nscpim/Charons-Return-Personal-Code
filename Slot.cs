using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;


public class Slot : VRTK_InteractableObject
{
    public GameObject grabHint;

    /// <summary>
    /// When you grab an object
    /// </summary>
    /// <param name="grabbingObject"></param>
    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);

        if (grabHint != null && grabHint.activeSelf)
        {
            StartCoroutine(DeactivateGrabHint());
        }

        //Command for grabbing to set client authority
        GameManager.instance.localPlayer.GetComponent<GrabbedObjectDistribution>().CmdDoGrab(gameObject, GameManager.instance.localPlayer.gameObject, grabbingObject.gameObject);
    }

    IEnumerator DeactivateGrabHint()
    {
        yield return new WaitForSeconds(3);
        grabHint.SetActive(false);
    }

    /// <summary>
    /// Called on release.
    /// </summary>
    /// <param name="previousGrabbingObject"></param>
    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);

        //Command for when you release the grab button to remove client authority
        GameManager.instance.localPlayer.GetComponent<GrabbedObjectDistribution>().CmdDoUngrab(gameObject, GameManager.instance.localPlayer.gameObject);
    }
}


