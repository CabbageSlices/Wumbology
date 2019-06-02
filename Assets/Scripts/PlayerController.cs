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

    private Vector3 currentForwardDirection  {
        get {
            Quaternion rotator = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z);
            var forward = rotator * defaultForward;
            return new Vector3(forward.x, forward.y, 0).normalized;
        }
    }

    //when moving and acceelration is being used, the input direction will specify what the velocity should be
    //this is used to calculate the acceleration direction = targetVelocity - currentVelocity
    private Vector2 targetVelocity;
    private Quaternion targetOrientation;

    private Rigidbody2D body;

    private enum States {
        not_attacking,
        attackStart,
        attacking,
        attackRecovery,
    };

    States state;

    float attackStartTime;
    
    public void setTargetOrientation(Quaternion target) {
        if(state == States.attackRecovery || state == States.attacking)
        return;
        targetOrientation = target;
    }

    //set the direction the player is trying to move in (tilted his joystick in this direction, try to apply velocity)
    public void setTargetMovementDirection(Vector2 target) {

        if(state == States.attackRecovery || state == States.attacking)
        return;

        targetVelocity = target * maxSpeed;
    }

    public void activateMeleeAttack() {
        
        if(state != States.not_attacking)
        return;


        //lunge forward
        body.AddForce(currentForwardDirection * 25, ForceMode2D.Impulse);
        attackStartTime = Time.time;
        state = States.attackStart;
            // createMeleeAttack();
    }

    public void onMeleeAttackFinish() {
        state = States.not_attacking;
    }

    void createMeleeAttack() {

        GameObject attack = GameObject.Instantiate(meleeAttack, transform.localPosition + currentForwardDirection, transform.localRotation, transform);
        attack.GetComponent<MeleeAttack>().owner = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        
        if(body == null)
            Debug.LogWarning("rigidbody2d component missing in " + name);

        if(meleeAttack == null)
            Debug.LogWarning("melee attack missing from " + name);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == States.not_attacking || state == States.attackStart) {

            Quaternion current = transform.localRotation;
            transform.localRotation = Quaternion.Slerp(current, targetOrientation, Time.deltaTime * rotationRate);
        }
    }

    void accelerateTowardsTargetVelocity() {
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

        if(state == States.attacking || state == States.attackRecovery)
            return;

        if(useAcceleration)
            accelerateTowardsTargetVelocity();
        else
            body.velocity = targetVelocity;
        
        // if(body.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        //     body.velocity = body.velocity.normalized * maxSpeed;
    }

    private void LateUpdate() {
        if(state != States.attackStart)
        return;

        if(state == States.attackStart && Time.time - attackStartTime > 0.25) {
            state = States.attacking;
            body.velocity = new Vector2(0,0);
            createMeleeAttack();
        }
    }
}
