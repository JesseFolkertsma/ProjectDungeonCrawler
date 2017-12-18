using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {
    public int id;
    public bool enabled;
    public enum Category {Weapon, Usable}
    public Category category;
    public Transform item;
    public int itemID;

    //Hover 
    public float amplitude;
    public float speed;
    public float rotateSpeed;


    //Respawn
    public int respawnTime;

    private void OnTriggerEnter(Collider other) {
        //Give weapon to player
        if (enabled) {
            PickUpActivate(other);
        }
    }
    public void PickUpActivate(Collider player) {
        player.transform.root.GetComponent<NWPlayerCombat>().EquipFromPickup(itemID, (int)category, id);
    }
    public void Update() {
        if (enabled) {
            Hover();    
        }
    }
    public void Toggle(bool enable) {

    }
    void Hover() {
        item.position = new Vector3(item.position.x,item.position.y + amplitude * Mathf.Sin(speed * Time.time),item.position.z);
        item.Rotate(new Vector3(0, rotateSpeed, 0));
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }


    IEnumerator RespawnRoutine()
    {
        enabled = false;
        item.gameObject.SetActive(enabled);
        yield return new WaitForSeconds(respawnTime);
        enabled = true;
        item.gameObject.SetActive(enabled);
    }
}
