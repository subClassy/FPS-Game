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
    public GameObject gun;
    public GameObject head;
    private int targetIndex = 0;
    private bool isPlayerDetected = false;
    private float gunShotTime = 0.2f;
    private float health = 100;
    private bool isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // animator.SetTrigger("die");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(head.transform.position, head.transform.forward * 100.0f, Color.red, 10);
        if (!isDead) 
        {
            var isGameOver = player.GetComponent<Gun>().isDead == true;
            
            if (isGameOver) 
            {
                GetComponent<Animator>().SetBool("over", true);
            }

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

            if (isPlayerDetected == false && d2Player < 15.0f && Math.Abs(angleWithPlayer) < 40.0f && IsPlayerInVision(playerPosition, myPosition)) {
                isPlayerDetected = true;
            }

            if (isPlayerDetected && !isGameOver) 
            {
                var movePosition = new Vector3(playerPosition.x, myPosition.y, playerPosition.z);
                var desiredRotation = Quaternion.LookRotation(movePosition - myPosition);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 6.0f);
                
                var gunDirection = Vector3.Angle(-playerDirection, end.transform.forward);   
                
                if (d2Player <= 10.0f) 
                {
                    if (gunShotTime <= 0) 
                    {
                        GetComponent<Animator>().SetBool("shoot", true);
                        
                        if (Math.Abs(gunDirection) <= 10.0f) 
                        {
                            AddEffects();
                            ShotDetection();
                            gunShotTime = 0.2f;
                        }
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
        var rifleEnd = end.transform.position + (end.transform.forward * UnityEngine.Random.Range(-1f, 1f)) + (end.transform.right * UnityEngine.Random.Range(-1f, 1f));

        if (Physics.Raycast(rifleEnd, (rifleEnd - start.transform.position).normalized, out rayHit, 10.0f))
        {   
            if(rayHit.collider.tag == "head")
            {
                player.GetComponent<Gun>().Being_shot(80.0f);
            }
            else if(rayHit.collider.tag == "chest")
            {
                player.GetComponent<Gun>().Being_shot(20.0f);
            }
            else if(rayHit.collider.tag == "arm")
            {
                player.GetComponent<Gun>().Being_shot(5.0f);
            }
            else if(rayHit.collider.tag == "leg")
            {
                player.GetComponent<Gun>().Being_shot(15.0f);
            }
            else
            {
                GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                Destroy(bulletHoleObject, 2.0f);
            }
            
        }
    }

    public bool IsPlayerInVision(Vector3 playerPosition, Vector3 myPosition)
    {
        RaycastHit rayHit;
        Ray ray = new Ray(myPosition + new Vector3(0, 1, 0), playerPosition - myPosition);

        if(Physics.Raycast(ray, out rayHit, 100.0f))
        {   
            if(rayHit.transform.name == "player")
            {
                return true;
            }
        }
        
        return false;
    }

    public void Being_shot(float damage)
    {
        if (!isDead)
        {
            isPlayerDetected = true;
            health -= damage;
            
            if (health <= 0)
            {
                isDead = true;
            }

            if (isDead == true)
            {
                GetComponent<Animator>().SetTrigger("isDead");
                GetComponent<CharacterController>().enabled = false;

                gun.GetComponent<Animator>().enabled = false;
                gun.transform.parent = null;
                var gunRigidBody = gun.AddComponent<Rigidbody>(); // Add the rigidbody.
                
                gunRigidBody.mass = 5; // Set the GO's mass to 5 via the Rigidbod
                gunRigidBody.GetComponent<Rigidbody>().isKinematic = false;
                gunRigidBody.GetComponent<Rigidbody>().detectCollisions = true;
                gunRigidBody.GetComponent<Rigidbody>().useGravity = true;

                var gunCollider = gun.AddComponent<CapsuleCollider>();
                gunCollider.center = new Vector3(0, 0, 0.1f);
                gunCollider.direction = 2;
                gunCollider.height = 1;
                gunCollider.radius = 0.2f;
            }
        }
    }
}
