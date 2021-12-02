using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject[] target;
    public GameObject player;
    public GameObject muzzleFlash;
    public GameObject shotSound;
    public GameObject start;
    public GameObject end;
    public GameObject bulletHole;
    private int targetIndex = 0;
    private bool isPlayerDetected = false;
    private float gunShotTime = 0.2f;
    
    // Start is called before the first frame update
    void Start()
    {
        // animator.SetTrigger("die");
    }

    // Update is called once per frame
    void Update()
    {
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }

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
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 6.0f);
            
            if (d2Player <= 10.0f) 
            {
                if (gunShotTime <= 0) 
                {
                    AddEffects();
                    ShotDetection();
                    GetComponent<Animator>().SetBool("shoot", true);
                    gunShotTime = 0.4f;
                }
    
                GetComponent<Animator>().SetBool("run", false);
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

    void AddEffects()
    {
        GameObject muzzleFlashObject = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObject.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObject, 1.0f);

        Destroy(Instantiate(shotSound, transform.position, transform.rotation), 1.0f);
    }

    void ShotDetection()
    {
        RaycastHit rayHit;
        var rifleEnd = end.transform.position + (end.transform.up * UnityEngine.Random.Range(-0.2f, 0.2f)) + (end.transform.right * UnityEngine.Random.Range(-0.2f, 0.2f));

        if (Physics.Raycast(rifleEnd, (rifleEnd - start.transform.position).normalized, out rayHit, 100.0f))
        {
            if(rayHit.transform.tag == "Player")
            {
                player.GetComponent<Gun>().Being_shot(20.0f);
                // rayHit.transform.GetComponent<Gun>().Being_shot(20);
            }
            else
            {
                GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                Destroy(bulletHoleObject, 2.0f);
            }
            
        }
    }
}
