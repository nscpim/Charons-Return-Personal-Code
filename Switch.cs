using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Switch : VRTK_InteractableObject
{
    public GameObject grabHint;

    /// <summary>
    ///  When you grab an object
    /// </summary>
    /// <param name="usingObject"></param>
    public override void Grabbed(VRTK_InteractGrab usingObject = null)
    {
        base.Grabbed(usingObject);

        if (grabHint != null && grabHint.activeSelf)
        {
            StartCoroutine(DeactivateGrabHint());
        }

        //Command for grabbing to set client authority
        GameManager.instance.localPlayer.GetComponent<GrabbedObjectDistribution>().CmdDoGrab(gameObject, GameManager.instance.localPlayer.gameObject, usingObject.gameObject);
    }

    IEnumerator DeactivateGrabHint()
    {
        yield return new WaitForSeconds(5);
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
