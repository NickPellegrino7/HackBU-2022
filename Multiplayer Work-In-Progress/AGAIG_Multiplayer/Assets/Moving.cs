using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Moving : NetworkBehaviour
{

    public float MoveSpeed = 5f;
    private CharacterController _characterController;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        // If the client's owner doesn't own this object, then don't proceed
        if (!base.IsOwner) {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * (MoveSpeed * Time.deltaTime);

        _characterController.Move(offset);

        // Move();
    }

    /*
    [Client(RequireOwnership = true)]
    private void Move() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * (MoveSpeed * Time.deltaTime);

        _characterController.Move(offset);
    }
    */
}
