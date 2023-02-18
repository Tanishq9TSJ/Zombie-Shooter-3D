using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;


    CapsuleCollider capsule;

    bool cursorIsLocked = true;
    bool lockCursor = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //Cursor Lock
        void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0)){
                cursorIsLocked = true;
            }
            if (cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if(!cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
