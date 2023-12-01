using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Projector : VRTK_InteractableObject
{
    [Tooltip("Amount of points the player is rewarded for completing this puzzle")]
    public int pointsReward;
    public bool puzzleComplete;
    /// <summary>
    /// if all the switches have been activated and all the parts have been put in the projector it plays the video and gives a reward.
    /// </summary>
    /// <param name="usingObject"></param>
    public override void StartUsing(VRTK_InteractUse usingObject = null)
    {
        base.StartUsing(usingObject);
        
        //check if all switches have been activated
        if (GetComponent<SwitchPuzzle>().GetProjectorBool())
        {
            ProjectorNetwork projectorNetwork = GetComponent<ProjectorNetwork>();
            
            //check if all the parts have been placed in the projector
            if (!puzzleComplete && projectorNetwork.Reel1 && projectorNetwork.Reel2 && projectorNetwork.Reel3 && projectorNetwork.projectorPlaced)
            {
                puzzleComplete = true;
                //Command for the video so it plays for every client
                GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdPlayProjector(gameObject);

                GameManager.instance.scoreManager.IncreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, pointsReward);
            }

        }
    }
}
