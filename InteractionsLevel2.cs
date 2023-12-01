using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Diner;

public class InteractionsLevel2 : NetworkBehaviour
{

    /// <summary>
    ///  Command for setting the pressureplate boolan syncvar on the server
    /// </summary>
    /// <param name="pressurePlate"></param>
    /// <param name="pressurePlateEnabled"></param>
    [Command]
    public void CmdSetPressurePlateBool(GameObject pressurePlate, bool pressurePlateEnabled)
    {
        pressurePlate.GetComponent<PressurePlate>().pressurePlateEnabled = pressurePlateEnabled;
    }

    /// <summary>
    //  Setting the spinCount syncvar on the server.
    /// </summary>
    /// <param name="arm"></param>
    [Command]
    public void CmdStartSpin(GameObject arm)
    {
        arm.GetComponent<SpinCounter>().spinCount++;
    }

    /// <summary>
    /// Setting the Projectorparts boolean placed on the server, for the lens it calls an RPC so it changes for every client.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="projector"></param>
    [Command]
    public void CmdPlaceProjectorParts(int part, GameObject projector)
    {
        switch (part)
        {
            case 1:
                projector.GetComponent<ProjectorNetwork>().Reel1 = true;
                break;
            case 2:
                projector.GetComponent<ProjectorNetwork>().Reel2 = true;
                break;
            case 3:
                projector.GetComponent<ProjectorNetwork>().Reel3 = true;
                break;
            case 4:
                projector.GetComponent<ProjectorNetwork>().projectorPlaced = true;
                RpcShowProjectorPart(4, projector);
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///  RPC call so the projector lens will be placed for every client
    /// </summary>
    /// <param name="part"></param>
    /// <param name="projector"></param>
    [ClientRpc]
    private void RpcShowProjectorPart(int part, GameObject projector)
    {
        projector.GetComponent<ProjectorNetwork>().projectorPart.SetActive(true);
    }

    /// <summary>
    /// Command for playing the projector
    /// </summary>
    /// <param name="projector"></param>
    [Command]
    public void CmdPlayProjector(GameObject projector)
    {
        projector.GetComponent<Projector>().puzzleComplete = true;
        GameManager.instance.scoreManager.UpdateResearchScore(1000);
        RpcPlayProjector(projector);
    }

    /// <summary>
    /// RPC call for playing the projector on every client
    /// </summary>
    /// <param name="projector"></param>
    [ClientRpc]
    private void RpcPlayProjector(GameObject projector)
    {
        Level2Manager.instance.solvedCinemaPuzzle = " ✔";
        Level2Manager.instance.SetInfoboardText();
        projector.GetComponent<Projector>().puzzleComplete = true;
        projector.GetComponent<ProjectorNetwork>().PlayProjector();
        Level2Manager.instance.ActivateFlamethrowers();
    }

    /// <summary>
    /// Command for setting the key and projector boolean syncvars on the server, so it will be disabled on all the clients
    /// </summary>
    /// <param name="deactivateThis"></param>
    [Command]
    public void CmdDeactivateForEveryone(GameObject deactivateThis)
    {
        switch (deactivateThis.name)
        {
            case "Key":
                deactivateThis.GetComponent<KeyPart>().deactivate = true;
                break;
            case "ProjectorLens":
            case "Reel1":
            case "Reel2":
            case "Reel3":
                deactivateThis.GetComponent<VideoPart>().deactivate = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Command for opening the chest
    /// </summary>
    /// <param name="chest"></param>
    [Command]
    public void CmdOpenChest(GameObject chest)
    {
        GameManager.instance.scoreManager.UpdateResearchScore(2500);
        RpcOpenChest(chest);
    }

    /// <summary>
    /// RPC Call for opening the chest on every client
    /// </summary>
    /// <param name="chest"></param>
    [ClientRpc]
    private void RpcOpenChest(GameObject chest)
    {
        Level2Manager.instance.solvedTreasureRoomPuzzle = " ✔";
        Level2Manager.instance.SetInfoboardText();
        chest.GetComponent<Chest>().OpenChest();
    }

    [Command]
    public void CmdMoveMachineGun(GameObject button, bool doMyThing)
    {
        button.GetComponent<ControlCameraMachineGun>().doMyThing = doMyThing;
    }

    [Command]
    public void CmdSetGunGrabberPos(GameObject gunGrabber)
    {
        RpcSetGunGrabberPos(gunGrabber);
    }

    [ClientRpc]
    public void RpcSetGunGrabberPos(GameObject gunGrabber)
    {
        gunGrabber.GetComponent<MoveGun>().ResetGrabberPos();
    }

    [Command]
    public void CmdSChangeShield()
    {
        Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().shield = 2;
        foreach (GameObject item in Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().enemiesList)
        {
            RpcChangeShield(item);
        }
    }

    [ClientRpc]
    public void RpcChangeShield(GameObject item)
    {
        item.transform.Find("MegaShield").gameObject.SetActive(false);
        item.transform.Find("Shield").gameObject.SetActive(true);
    }

    [Command]
    public void CmdExplodeShell(Vector3 hitPos, int playerNetId)
    {
        GameObject shell = Instantiate((GameObject)Resources.Load("Prefabs/Projectiles/ExplosiveShell"), hitPos, Quaternion.identity);
        shell.GetComponent<ExplosiveShell>().playerNetId = playerNetId;
    }

    [Command]
    public void CmdFireGrenadeLauncher(GameObject launcher, int playerNetId)
    {
        launcher.GetComponent<MoveGun>().DoFiring(playerNetId);
        RpcFireGrenadeLauncher(launcher);
    }

    [ClientRpc]
    public void RpcFireGrenadeLauncher(GameObject launcher)
    {
        if (launcher.GetComponent<MoveGun>().starboard &&
            !Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase1KrakenTentacleDied &&
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase1Tentacle.GetComponent<MeshRenderer>().enabled)
        {
            launcher.GetComponent<MoveGun>().ActivatePoison();
        }
        if (!launcher.GetComponent<MoveGun>().starboard &&
            !Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase2KrakenTentacleDied &&
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase2Tentacle.GetComponent<MeshRenderer>().enabled)
        {
            launcher.GetComponent<MoveGun>().ActivatePoison();
        }
    }

    [Command]
    public void CmdSetReloadTime(GameObject launcher, int timeLeft)
    {
        RpcSetReloadTime(launcher, timeLeft);
    }

    [ClientRpc]
    public void RpcSetReloadTime(GameObject launcher, int timeLeft)
    {
        launcher.GetComponent<MoveGun>().SetTimeLeft(timeLeft);
    }

    [Command]
    public void CmdActivateKrakenPoison(GameObject poisonActivator, bool setPoison)
    {
        RpcActivateKrakenPoison(poisonActivator, setPoison);
    }

    [ClientRpc]
    public void RpcActivateKrakenPoison(GameObject poisonActivator, bool setPoison)
    {
        poisonActivator.GetComponent<KrakenPoisonDamage>().PoisonActivator(setPoison);
    }

    [Command]
    public void CmdStopShip(bool stopShip)
    {
        if (stopShip)
        {
            foreach (GameObject item in Level2Manager.instance.shipControls)
            {
                item.GetComponent<SteerShip>().stopShip = true;
            }
        }
        else
        {
            foreach (GameObject item in Level2Manager.instance.shipControls)
            {
                item.GetComponent<SteerShip>().stopShip = false;
            }
        }
    }

    [Command]
    public void CmdSetDeadKrakententacle(bool starboard, int phase)
    {
        RpcSetDeadKrakententacle(starboard, phase);
    }

    [ClientRpc]
    private void RpcSetDeadKrakententacle(bool starboard, int phase)
    {
        Material[] krakenMaterials = Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().krakenBody.transform.Find("body").GetComponent<Renderer>().materials;
        if (phase == 1)
        {
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase1KrakenTentacleDied = true;
            krakenMaterials[7] = Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().invisible;
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().krakenBody.transform.Find("body").GetComponent<Renderer>().materials = krakenMaterials;
        }
        else if (phase == 2)
        {
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase2KrakenTentacleDied = true;
            krakenMaterials[1] = Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().invisible;
            Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().krakenBody.transform.Find("body").GetComponent<Renderer>().materials = krakenMaterials;
        }
        else if (phase == 3)
        {
            if (starboard)
            {
                Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase3StarboardKrakenTentacleDied = true;
                krakenMaterials[5] = Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().invisible;
                Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().krakenBody.transform.Find("body").GetComponent<Renderer>().materials = krakenMaterials;
            }
            else
            {
                Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().phase3PortKrakenTentacleDied = true;
                krakenMaterials[3] = Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().invisible;
                Level2Manager.instance.bossFightManager.GetComponent<BossFightManager>().krakenBody.transform.Find("body").GetComponent<Renderer>().materials = krakenMaterials;
            }
        }
    }

    [Command]
    public void CmdUngrabLever(GameObject lever)
    {
        lever.GetComponent<SpinCounter>().RpcUngrabLever();
        if (!lever.GetComponent<SpinCounter>().spinTimer.IsActive &&
            lever.GetComponent<SpinCounter>().spinTimer.TimerDone())
        {
            //SpinCount 4 means that it is going back to 1 from "777"
            lever.GetComponent<SpinCounter>().spinCount++;
            if (lever.GetComponent<SpinCounter>().spinCount == 4)
            {
                lever.GetComponent<SpinCounter>().spinCount = 1;
            }
            lever.GetComponent<SpinCounter>().spinTimer.SetTimer(5.3f);
        }
    }

    [Command]
    public void CmdReadyForLevel3(GameObject elevator)
    {
        GameManager.instance.playersReadyForLevel3++;
        if (GameManager.instance.playersReadyForLevel3 == 3)
        {
            GameManager.instance.localPlayer.GetComponent<Player>().CmdStartChangeScene(elevator);
        }
    }

    [ClientRpc]
    public void RpcSetKrakenTentacleHit()
    {
        Level2Manager.instance.krakenGrenadeLauncherHitsDone++;
        if (Level2Manager.instance.krakenGrenadeLauncherHitsDone == 2)
        {
            Level2Manager.instance.allKrakenGrenadeLauncherHitsDone = " ✔";
        }
        Level2Manager.instance.SetInfoboardText();
    }
    [Command]
    public void CmdObjectSnapped(GameObject theObj, GameObject otherObj)
    {
        RpcObjectSnapped(theObj, otherObj);
    }

    [ClientRpc]
    public void RpcObjectSnapped(GameObject theObj, GameObject otherObj)
    {
        theObj.GetComponent<NetworkedSnapDropZone>().DoObjectSnapped(otherObj);
    }
    [Command]
    public void CmdObjectUnsnapped(GameObject theObj)
    {
        RpcObjectUnsnapped(theObj);
    }

    [ClientRpc]
    public void RpcObjectUnsnapped(GameObject theObj)
    {
        theObj.GetComponent<NetworkedSnapDropZone>().DoObjectUnsnapped();
    }

    [Command]
    public void CmdSetSongKey(GameObject theObj)
    {
        RpcSetSongKey(theObj);
    }

    [ClientRpc]
    public void RpcSetSongKey(GameObject theObj)
    {
        theObj.GetComponentInParent<Jukebox>().ButtonPress(theObj.name);
    }

    [Command]
    public void CmdStartMicrowave(GameObject theObj)
    {
        RpcStartMicrowave(theObj);
    }

    [ClientRpc]
    public void RpcStartMicrowave(GameObject theObj)
    {
        theObj.GetComponent<InsideMicrowave>().StartCoroutine("StartMicrowaveTime");

    }

    // Musical Instrument Interaction Commands
    [Command]
    public void CmdPlaySound(string audioClip, Vector3 position, GameObject instrument, string instrumentType)
    {

        if(instrumentType == "Piano")
        {
            instrument.GetComponent<PianoKeyAnimator>().RpcPlaysound();
        }
        else if (instrumentType == "Drum")
        {
            instrument.GetComponent<SoundEffectTrigger>().RpcPlaysound(audioClip, position);
        }
        else if (instrumentType == "Guitar")
        {
            instrument.GetComponent<SoundEffectTrigger>().RpcPlaysound(audioClip, position);
        }
    }

    // Command used in combination with an RPC to sync the rotating of the nob across clients
    [Command]
    public void CmdRotateGuitarNob(int destinationInt, GameObject instrument)
    {
        instrument.GetComponent<SoundTriggerSelector>().RpcRotateKnob(destinationInt);
    }

    [Command]
    public void CmdChangeGuitarAttach(bool _attach, GameObject _guitar)
    {
        if (_attach)
        {
            _guitar.GetComponent<GuitarBodyTrigger>().RpcAttachGuitar();
        }
        else if(!_attach)
        {
            _guitar.GetComponent<GuitarBodyTrigger>().RpcDetachGuitar();
        }
    }

    [Command]
    public void CmdChangeGuitarButtons(GameObject[] buttons, GameObject calledButton)
    {
        Debug.Log("Called command, setting release and press");
        for (int x = 0; x < buttons.Length; x++)
        {
            if (buttons[x] != calledButton)
            {
                buttons[x].GetComponent<GuitarButton>().RpcReleaseButton();
            }
        }
        calledButton.GetComponent<GuitarButton>().RpcPressButton();
    }

    [Command]
    public void CmdGrabBowlingBall(GameObject ball, int playerNetid)
    {
        RpcGrabBowlingBall(ball, playerNetid);
    }

    [ClientRpc]
    public void RpcGrabBowlingBall(GameObject ball, int playerNetId)
    {
        ball.GetComponent<BowlingBallAddForceScript>().GrabTheBowlingBall(playerNetId);
    }

    // Change the score
    /* [Command]
     public void CmdsetScore(int score, int scoreBoardPos, int totalScoreBoardPos, GameObject bowlingBallChecker)
     {
         RpcSetScore(score, scoreBoardPos, totalScoreBoardPos, bowlingBallChecker);
        /* Debug.Log(score);
         Debug.Log(scoreBoardPos);
         Debug.Log(totalScoreBoardPos);
     }*/

    [ClientRpc]
    public void RpcSetScore(int score, int scoreBoardPos, int totalScoreBoardPos, GameObject bowlingBallChecker)
    {
        bowlingBallChecker.GetComponent<BowlingBallChecker>().SetScore(score, scoreBoardPos, totalScoreBoardPos);

    }

    // Change BowlingBall Materials
    [Command]
    public void CmdChangeMaterial(int RandomNumber, GameObject Bowlingball)
    {
        RpcSetChangeMaterial(RandomNumber, Bowlingball);
    }

    [ClientRpc]
    private void RpcSetChangeMaterial(int RandomNumber, GameObject Bowlingball)
    {
        Bowlingball.GetComponent<BowlingBall>().RpcSetBall(RandomNumber);
    }


    // Set the splitboard to be the same
    [Command]
    public void CmdSetBoardText(string BoardText, GameObject bowlingBallChecker)
    {
        RpcSetBoardText(BoardText, bowlingBallChecker);
    }

    [ClientRpc]
    private void RpcSetBoardText(string BoardText, GameObject bowlingBallChecker)
    {
        bowlingBallChecker.GetComponent<BowlingBallChecker>().GetSplits(BoardText);
    }

    // ChangePlayer to player one or two
    [Command]
    public void CmdSetActivePlayer(GameObject bowlingBallChecker)
    {
        RpcSetActivePlayer(bowlingBallChecker);
    }

    [ClientRpc]
    private void RpcSetActivePlayer(GameObject bowlingBallChecker)
    {
        bowlingBallChecker.GetComponent<BowlingBallChecker>().ChangePlayer();
    }

    // Called after player one or player two is set and set the basic variables
    [ClientRpc]
    public void RpcRoll(bool Player1, bool Player2, int Player1Throws, int Player2Throws, int TopboardPos, int TurntotalscoreCounter, GameObject bowlingBallChecker)
    {
        bowlingBallChecker.GetComponent<BowlingBallChecker>().roll(Player1, Player2, Player1Throws, Player2Throws, TopboardPos, TurntotalscoreCounter);
    }

    /* [ClientRpc]
     private void RpcRoll_(bool Player1, bool Player2, int Player1Throws, int Player2Throws, int TopboardPos, int TurntotalscoreCounter, GameObject bowlingBallChecker)
     {
         bowlingBallChecker.GetComponent<BowlingBallChecker>().roll(Player1,Player2, Player1Throws, Player2Throws, TopboardPos, TurntotalscoreCounter);
         Debug.Log(Player1);
         Debug.Log(Player2);
     }*/
}
