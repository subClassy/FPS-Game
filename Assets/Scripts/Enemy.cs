using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject[] target;
    public GameObject player;
    private int targetIndex = 0;
    private bool isPlayerDetected = false;
    // Start is called before the first frame update
    void Start()
    {
        // animator.SetTrigger("die");
    }

    // Update is called once per frame
    void Update()
    {
        var playerPosition = player.transform.position;
        var myPosition = transform.position;
        
        var d2Player = Vector3.Distance(playerPosition, myPosition);
        var playerDirection = new Vector3((playerPosition.x - myPosition.x), (playerPosition.y - myPosition.y), (playerPosition.z - myPosition.z)).normalized;
        var angleWithPlayer = Vector3.Angle(playerDirection, transform.forward);
        
        if (isPlayerDetected == false)
        {
            MoveOnPath(myPosition);
        }

        if (isPlayerDetected == false && d2Player < 15.0f && Math.Abs(angleWithPlayer) < 30.0f) {
            isPlayerDetected = true;
        }

        if (isPlayerDetected) 
        {
            var movePosition = new Vector3(playerPosition.x, myPosition.y, playerPosition.z);
            var desiredRotation = Quaternion.LookRotation(movePosition - myPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);
            
            if (d2Player <= 10.0f) 
            {
                GetComponent<Animator>().SetBool("run", false);
                GetComponent<Animator>().SetBool("shoot", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("run", true);
                GetComponent<Animator>().SetBool("shoot", false);
            }
        }
        
    }

    void MoveOnPath(Vector3 myPosition)
    {
        var movePosition = new Vector3(target[targetIndex].transform.position.x, myPosition.y, target[targetIndex].transform.position.z);
        var desiredRotation = Quaternion.LookRotation(movePosition - myPosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);

        if (Vector3.Distance(target[targetIndex].transform.position, myPosition) <= 0.5f)
        {
            targetIndex = (targetIndex + 1) % target.Length;
        }
    }

    // void DetectPlayer()
    // {
    //     var dist_player = Vector3.Distance(transfor.position - player.tranform.position);
    //     angle_with_player = Vector3.Angle(player_direction, transform.forward);
    // }
}
