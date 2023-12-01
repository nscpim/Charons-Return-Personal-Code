using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatePuzzle : MonoBehaviour
{
    [Header("GameObjects")]
    [Tooltip("Door that opens")]
    public GameObject door;
    [Tooltip("Chest that can be opened")]
    public GameObject chest;
    [Tooltip("all the pressureplates")]
    public GameObject[] pressurePlates;

    private bool openedDoor;

    public GameObject teleportArea;

    public GameObject fadeCollider;

    /// <summary>
    /// if all the pressureplates are occupied by a player opens the door and activates the chest.
    /// if someone steps off a pressureplate it will close the door and deactivates the chest.
    /// </summary>
    public void Update()
    {

        if (pressurePlates[0].GetComponent<PressurePlate>().GetBool() && pressurePlates[1].GetComponent<PressurePlate>().GetBool() && !openedDoor)
        {
            openedDoor = true;
            door.GetComponent<Animator>().SetTrigger("doorOpen");
            chest.gameObject.SetActive(true);
            teleportArea.SetActive(true);
            fadeCollider.SetActive(false);
        }
        else if (!(pressurePlates[0].GetComponent<PressurePlate>().GetBool() && pressurePlates[1].GetComponent<PressurePlate>().GetBool()) && openedDoor)
        {
            openedDoor = false;
            door.GetComponent<Animator>().SetTrigger("doorClose");
            chest.gameObject.SetActive(false);
            teleportArea.SetActive(false);
            fadeCollider.SetActive(true);
        }
    }
}
