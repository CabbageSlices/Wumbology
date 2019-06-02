using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField]
    private int playerNumber = 1;

    //button number on the joypad that corresponds to the attack button
    //https://wiki.unity3d.com/index.php/Xbox360Controller reference
    [SerializeField]
    private KeyCode swordAttackButton = KeyCode.Joystick1Button0;

    [SerializeField]
    private KeyCode shootButton = KeyCode.Joystick1Button1;

    [SerializeField]
    private PlayerController playerController;

    private string horizontalAxisName = "Horizontal1";
    private string verticalAxisName = "Vertical1";

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        horizontalAxisName = "Horizontal" + playerNumber;
        verticalAxisName = "Vertical" + playerNumber;
        
        if(playerController == null)
            Debug.Log("playerController component missing in " + name);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis(horizontalAxisName);
        float verticalInput = Input.GetAxis(verticalAxisName);

        Vector2 inputOrientation = new Vector2(horizontalInput, verticalInput);

        Quaternion targetOrientation = transform.localRotation;

        //if input values are too small then don't rotate player because if values are 0 there is no valid orientation
        if(inputOrientation.sqrMagnitude > 0) {
            float angleAroundzAxis = Mathf.Atan2(inputOrientation.y, inputOrientation.x);
            targetOrientation = Quaternion.Euler(0, 0, angleAroundzAxis * Mathf.Rad2Deg);
        }

        Vector2 targetMovementDirection = new Vector2(horizontalInput, verticalInput).normalized;

        
        playerController.setTargetOrientation(targetOrientation);
        playerController.setTargetMovementDirection(targetMovementDirection);

        if(Input.GetKeyDown(swordAttackButton))
        playerController.activateMeleeAttack();

        if(Input.GetKeyDown(shootButton))
        Debug.Log("gun");
    }
}
