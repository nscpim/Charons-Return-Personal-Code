using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.Networking;

public class VideoPart : NetworkBehaviour
{
    //Deactivate gets called when deactivate boolean changes
    [SyncVar(hook = "Deactivate")]
    public bool deactivate;
    
    /// <summary>
    /// Deactivates the Projector items when picked up
    /// </summary>
    /// <param name="deact"></param>
    private void Deactivate(bool deact)
    {
        if (deact)
        {
            if (name == "Reel1" || name == "Reel2" || name == "Reel3")
            {
                if (isServer)
                {
                    GameManager.instance.scoreManager.UpdateResearchScore(100);
                }
                Level2Manager.instance.reelsPickedUp++;
                if (Level2Manager.instance.reelsPickedUp == 3)
                {
                    Level2Manager.instance.allReelsPickedUp = " ✔";
                }
                Level2Manager.instance.SetInfoboardText();
            }
            if (name == "ProjectorLens")
            {
                if (isServer)
                {
                    GameManager.instance.scoreManager.UpdateResearchScore(100);
                }
                Level2Manager.instance.projectorLensPickedUp = " ✔";
                Level2Manager.instance.SetInfoboardText();
            }
            gameObject.SetActive(false);
        }
    }
}
