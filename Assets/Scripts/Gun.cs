using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Gun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun_1;
    public GameObject gun_2;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;
    public GameObject gun2Mag;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    float gunChangeTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public float initialHealth = 100;

    public Text currentHealth;
    public Text maxHealth;
    public bool isDead;
    public Text magBullets;
    public Text remainingBullets;
    public Text gameInfo;
    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int gun2magBulletsVal = 2;
    int gun2remainingBulletsVal = 12;
    int curMagBulletsVal = 30;
    int curRemainingBulletsVal = 90;
    int magSize = 30;
    int gun2MagSize = 2;
    int curMagSize = 30;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }
    public GameObject bulletHole;
    public GameObject bloodStream;
    public GameObject muzzleFlash;
    public GameObject shotSound;

    // Use this for initialization
    void Start() {
        headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
        gun_1.gameObject.SetActive(true);
        gun_2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
        if (gun_1.gameObject.activeSelf) 
        {
            curMagBulletsVal = magBulletsVal;
            curRemainingBulletsVal = remainingBulletsVal;
            curMagSize = magSize;
        }
        else 
        {
            curMagBulletsVal = gun2magBulletsVal;
            curRemainingBulletsVal = gun2remainingBulletsVal;
            curMagSize = gun2MagSize;
        }
        
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }
        if (gunChangeTime >= 0.0f)
        {
            gunChangeTime -= Time.deltaTime;
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && curMagBulletsVal > 0 && !isDead)
        { 
            shotDetection(); // Should be completed

            addEffects(); // Should be completed

            animator.SetBool("fire", true);
            gunShotTime = 0.5f;
            
            // Instantiating the muzzle prefab and shot sound
            
            curMagBulletsVal = curMagBulletsVal - 1;
            if (curMagBulletsVal <= 0 && curRemainingBulletsVal > 0)
            {
                animator.SetBool("reloadAfterFire", true);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.5f);
            }
        }
        else
        {
            animator.SetBool("fire", false);
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && curRemainingBulletsVal > 0 && curMagBulletsVal < curMagSize && !isDead )
        {
            animator.SetBool("reload", true);
            gunReloadTime = 2.5f;
            Invoke("reloaded", 2.0f);
        }
        else
        {
            animator.SetBool("reload", false);
        }

        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q)) && gunChangeTime <= 0.0f && !isDead )
        {
            animator.SetBool("gunSwitch", true);
            gunChangeTime = 2.5f;
            Invoke("SwitchGuns", 2.0f);
            
            if (gun_1.gameObject.activeSelf) 
            {
                curMagBulletsVal = magBulletsVal;
                curRemainingBulletsVal = remainingBulletsVal;
                curMagSize = magSize;
            }
            else 
            {
                curMagBulletsVal = gun2magBulletsVal;
                curRemainingBulletsVal = gun2remainingBulletsVal;
                curMagSize = gun2MagSize;
            }
        }
        else
        {
            animator.SetBool("gunSwitch", false);
        }

        updateText();
        updateHealthText();
       
        if (gun_1.gameObject.activeSelf) 
        {
            magBulletsVal = curMagBulletsVal;
            remainingBulletsVal = curRemainingBulletsVal;
        }
        else 
        {
            gun2magBulletsVal = curMagBulletsVal;
            gun2remainingBulletsVal = curRemainingBulletsVal;
        }
    }

    public void SwitchGuns()
    {
        gun_1.gameObject.SetActive(!gun_1.gameObject.activeSelf);
        gun_2.gameObject.SetActive(!gun_2.gameObject.activeSelf);
    }

    public void Being_shot(float damage) // getting hit from enemy
    {
        health -= damage;
        
        if (health <= 0)
        {
            isDead = true;
        }

        if (isDead == true)
        {
            updateGameInfo("You Died!");
            GetComponent<Animator>().SetBool("dead", true);
            GetComponent<CharacterMovement>().isDead = true;
            GetComponent<CharacterController>().enabled = false;
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        GameObject cMag;

        if (gun_1.gameObject.activeSelf) 
        {
            cMag = gunMag;
        }
        else 
        {
            cMag = gun2Mag;
        }

        if (eventNumber == 1) 
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            cMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        
        if (eventNumber == 2) 
        {
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            cMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
    }

    void reloaded()
    {
        int newMagBulletsVal = Mathf.Min(curRemainingBulletsVal + curMagBulletsVal, curMagSize);
        int addedBullets = newMagBulletsVal - curMagBulletsVal;
        curMagBulletsVal = newMagBulletsVal;
        curRemainingBulletsVal = Mathf.Max(0, curRemainingBulletsVal - addedBullets);
        animator.SetBool("reloadAfterFire", false);

        if (gun_1.gameObject.activeSelf) 
        {
            magBulletsVal = curMagBulletsVal;
            remainingBulletsVal = curRemainingBulletsVal;
        }
        else 
        {
            gun2magBulletsVal = curMagBulletsVal;
            gun2remainingBulletsVal = curRemainingBulletsVal;
        }
    }

    void updateText()
    {
        magBullets.text = curMagBulletsVal.ToString() ;
        remainingBullets.text = curRemainingBulletsVal.ToString();
    }

    void updateHealthText()
    {
        maxHealth.text = initialHealth.ToString();
        var textHealth = Mathf.Max(0, health);
        currentHealth.text = textHealth.ToString();
    }

    void updateGameInfo(string text)
    {
        gameInfo.text = text;
    }

    void shotDetection() // Detecting the object which player shot 
    {
        RaycastHit rayHit;
        if (gun_1.gameObject.activeSelf) 
        {
            if (Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 15.0f))
            {
                if(rayHit.collider.tag == "head")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(100.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "chest")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(30.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "arm")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(10.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "leg")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(20.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else
                {
                    GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    Destroy(bulletHoleObject, 2.0f);
                }
            }
        }
        else 
        {
            if (Physics.Raycast(end.transform.position, (end.transform.position - start.transform.position).normalized, out rayHit, 5.0f))
            {
                if(rayHit.collider.tag == "head")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(100.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "chest")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(60.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "arm")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(20.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else if(rayHit.collider.tag == "leg")
                {
                    rayHit.transform.GetComponent<BodyPartHit>().BodyHit(40.0f);
                    GameObject bloodObject = Instantiate(bloodStream, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    bloodObject.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodObject, 1.0f);
                }
                else
                {
                    GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
                    Destroy(bulletHoleObject, 2.0f);
                }
            }
        }
    }

    void addEffects() // Adding muzzle flash, shoot sound and bullet hole on the wall
    {
        GameObject muzzleFlashObject = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObject.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObject, 1.0f);

        Destroy(Instantiate(shotSound, transform.position, transform.rotation), 1.0f);
    }

    void OnTriggerEnter(Collider collision)
    {   
        if(collision.gameObject.tag == "ammo")
        {
            var firstChild = collision.gameObject.transform.GetChild(0);
            if (firstChild.tag == "shells" && firstChild.gameObject.activeSelf)
            {
                firstChild.gameObject.SetActive(!firstChild.gameObject.activeSelf);
                
                remainingBulletsVal = 90;
                gun2remainingBulletsVal = 12;
                updateText();
            }
        }

        if (collision.gameObject.tag == "door")
        {
            print("Game won!");
            updateGameInfo("You Win!!");
            Invoke("RestartGame", 10.0f);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        print("Game Restarted");
    }
}
