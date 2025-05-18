using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float horizontalMove;
    public float verticalMove;
    public CharacterController player;
    public float PlayerSpeed;

    private Vector3 playerInput;
    private Vector3 movePlayer;
    public Camera MainCamera;
    private Vector3 camForward;
    private Vector3 camRigth;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal"); // "Horizontal" con 'H' mayúscula
        verticalMove = Input.GetAxis("Vertical");   // "Vertical" con 'V' mayúscula
     

        playerInput = new Vector3(horizontalMove,0,verticalMove);
        playerInput = Vector3.ClampMagnitude(playerInput, 1);

        camDirection();
        movePlayer = playerInput.x * camRigth + playerInput.z * camForward;

        player.transform.LookAt(player.transform.position +movePlayer);

        player.Move(movePlayer * PlayerSpeed * Time.deltaTime);
      
        

        Debug.Log(player.velocity.magnitude);
    }
    void camDirection()
    {
        camForward = MainCamera.transform.forward;
        camRigth = MainCamera.transform.right;

        camForward.y = 0;
        camRigth.y = 0;

        camForward = camForward.normalized;
        camRigth= camRigth.normalized;
    }
    private void FixedUpdate()
    {
       
    }
}
