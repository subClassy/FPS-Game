using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject[] target;
    private int targetIndex = 0;
    public GameObject player;
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
        
        var movePosition = new Vector3(target[targetIndex].transform.position.x, myPosition.y, target[targetIndex].transform.position.z);
        var desiredRotation = Quaternion.LookRotation(movePosition - myPosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);

        if (Vector3.Distance(target[targetIndex].transform.position, myPosition) <= 0.5f)
        {
            targetIndex = (targetIndex + 1) % target.Length;
        }
        
    }

    void Reloading()
    {
        // animator.SetTrigger("reload_trigger");
    }

    // void DetectPlayer()
    // {
    //     var dist_player = Vector3.Distance(transfor.position - player.tranform.position);
    //     angle_with_player = Vector3.Angle(player_direction, transform.forward);
    // }
}
