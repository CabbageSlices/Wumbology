using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{

    public PlayerController owner;

    public void OnAnimationFinish() {
        Destroy(gameObject);
        owner?.onMeleeAttackFinish();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(owner == null)
        owner = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
