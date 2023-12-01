using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using UnityEngine.UI;

public class ProjectorNetwork : NetworkBehaviour
{
    [Header("Public Objects")]
    //The Video that will be played on the projector
    [Tooltip("the video that plays when the projector is activated")]
    public VideoPlayer videoClip;
    [Tooltip("The lens part of the projector that will be activated")]
    public GameObject projectorPart;

    [Header("Booleans for testing")]
    //Bools to check if everything has been placed, public for testing
    [SyncVar]
    public bool projectorPlaced;
    [SyncVar(hook = "Reel1Actions")]
    public bool Reel1;
    [SyncVar(hook = "Reel2Actions")]
    public bool Reel2;
    [SyncVar(hook = "Reel3Actions")]
    public bool Reel3;

    public GameObject reel;
    public GameObject lidTopOpen;
    public GameObject lidTopClosed;
    public GameObject lidBottomOpen;
    public GameObject lidBottomClosed;

    /// <summary>
    /// Plays the video on the cinema screen
    /// </summary>
    public void PlayProjector()
    {
        videoClip.Play();

        StartCoroutine(WaitUntilHanZorroFinished());
    }
    /// <summary>
    ///  If an item is in the collider range it will check for the item and place it in the projector
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        //checks if the items are reels, switch for the reels and calls a command to set the boolean syncvars
        if (other.name == "Reel1" || other.name == "Reel2" || other.name == "Reel3" || other.name == "ProjectorLens")
        {
            //Sets the hand model to true so it shows your hand when you placed the item
            GameManager.instance.VRTKSDKManager.transform.Find("SDKSetups").transform.Find("SteamVR").transform.Find("[CameraRig]").transform.Find("Controller (right)").transform.Find("RightController").transform.Find("hand_1").gameObject.SetActive(true);
            //Unparent and reset to item from inventory to startposition
            GameManager.instance.localPlayer.GetComponent<Player>().CmdUnSetAParent(other.gameObject, other.gameObject.GetComponent<Item>().startPosition);
            //Removes the item from the inventory
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Remove(other.gameObject.name);
            //Set equippedWeapon to null so you dont see your hand when you pick up an item
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon = null;

            //Switch with the item name and calls a command that sets the syncvar for the reels, for the projectorLens it also calls an RPC so it changes for every client
            switch (other.name)
            {
                case "Reel1":
                    GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdPlaceProjectorParts(1, gameObject);
                    break;
                case "Reel2":
                    GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdPlaceProjectorParts(2, gameObject);
                    break;
                case "Reel3":
                    GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdPlaceProjectorParts(3, gameObject);
                    break;
                case "ProjectorLens":
                    GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdPlaceProjectorParts(4, gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    private void Reel1Actions(bool reel1Bool)
    {
        Reel1 = reel1Bool;
        reel.SetActive(true);
        if (Reel2)
        {
            lidTopOpen.SetActive(false);
            lidTopClosed.SetActive(true);
        }
    }

    private void Reel2Actions(bool reel2Bool)
    {
        Reel2 = reel2Bool;
        reel.SetActive(true);
        if (Reel1)
        {
            lidTopOpen.SetActive(false);
            lidTopClosed.SetActive(true);
        }
    }

    private void Reel3Actions(bool reel3Bool)
    {
        Reel3 = reel3Bool;
        lidBottomOpen.SetActive(false);
        lidBottomClosed.SetActive(true);
    }

    void Update()
	{
        Camera camera = GameManager.instance.localPlayer.GetComponent<Camera>();
        Vector3 CameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));

        if(Physics.Raycast(CameraCenter, transform.forward, 100))
		{
            Debug.Log("Ou yeah!");
		}
    }

    IEnumerator WaitUntilHanZorroFinished()
	{
		while(videoClip.isPlaying)
		{
            yield return null;
		}

        GameManager.instance.achievemensTracker.HanZorroFilmFinished();
	}
}
