using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartHit : MonoBehaviour
{
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BodyHit(float damage)
    {
		character.GetComponent<Enemy>().Being_shot(damage);
    }
}
