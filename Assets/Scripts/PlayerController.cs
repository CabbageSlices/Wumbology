using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //wether or not the character uses acceleration while moving, or always goes full speed
    public bool useAcceleration = true;

    [SerializeField]
    private int playerNumber = 1;

    [SerializeField]
    private float accelerationRate = 3;

    [SerializeField]
    private float rotationRate = 3;

    [SerializeField]
    private float maxSpeed = 12;

    //slash effect to create when player attacks
    [SerializeField]
    private GameObject meleeAttack;

    private Vector3 defaultForward = new Vector3(1,0,0);

    private Vector2 currentForwardDirection  {
        get {
            Quaternion rotator = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z);
            return rotator * defaultForward;
        }
    }

    //when moving and acceelration is being used, the input direction will specify what the velocity should be
    //this is used to calculate the acceleration direction = targetVelocity - currentVelocity
    private Vector2 targetVelocity;
    private Quaternion targetOrientation;

    private Rigidbody2D body;

    private enum States {
        attacking,
        not_attacking
    };

    States state;
    
    public void setTargetOrientation(Quaternion target) {
        targetOrientation = target;
    }

    //set the direction the player is trying to move in (tilted his joystick in this direction, try to apply velocity)
    public void setTargetMovementDirection(Vector2 target) {
        targetVelocity = target * maxSpeed;
    }

    public void activateMeleeAttack() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        
        if(body == null)
            Debug.Log("rigidbody2d component missing in " + name);

        if(meleeAttack == null)
            Debug.Log("melee attack missing from " + name);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion current = transform.localRotation;
        transform.localRotation = Quaternion.Slerp(current, targetOrientation, Time.deltaTime * rotationRate);
        Debug.Log(currentForwardDirection);
    }

    void applyInputAcceleration() {
        float velocityDifferenceSquared = (targetVelocity - body.velocity).sqrMagnitude;

        //accelerate towards input velocity from current velocity
        Vector2 accelerationDirection = (targetVelocity - body.velocity).normalized;

        Vector2 acceleration = accelerationDirection * accelerationRate;

        //if the change in velocity due to acceleration is larger than the velocity difference it means you will overshoot and ossicilate around the target velocity
        //therefore if the difference between current and target velocities is small enough, just set ite velocicty to the target
        if(velocityDifferenceSquared < accelerationRate * accelerationRate)
            body.velocity = targetVelocity;
        else
            body.AddForce(acceleration, ForceMode2D.Impulse);
    }

    private void FixedUpdate() {

         if(useAcceleration)
            applyInputAcceleration();
        else
            body.velocity = targetVelocity;
        
        // if(body.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        //     body.velocity = body.velocity.normalized * maxSpeed;
    }
}
