using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.Networking;

public class KeyPart : NetworkBehaviour
{
    //Deactivate gets calles whenever the deactivate bool changes on the server.
    [SyncVar(hook = "Deactivate")]
    public bool deactivate;

    /// <summary>
    /// Gets called through the command CmdDeactivateForEveryone method so it changes for every client and deactivates the key when picked up
    /// </summary>
    /// <param name="deact"></param>
    private void Deactivate(bool deact)
    {
        if (deact)
        {
            if (isServer)
            {
                GameManager.instance.scoreManager.UpdateResearchScore(100);
            }
            Level2Manager.instance.pickedUpKey = " ✔";
            Level2Manager.instance.SetInfoboardText();
            gameObject.SetActive(false);
        }
    }
}
