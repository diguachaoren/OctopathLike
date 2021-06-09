using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;
    bool up;
    bool down;
    bool left;
    bool right;
    [SerializeField]
    private float playerSpeed;

    string currentState = "Idle";
    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Z)){
            up = true;
        }
        else if(Input.GetKey(KeyCode.S)){
            down = true;
        }
        else if(Input.GetKey(KeyCode.D)){
            right = true;
        }
        else if(Input.GetKey(KeyCode.Q)){
            left = true;
        }
    }

    private void FixedUpdate() {

        if(up){
            ChangeAnimState("WalkBack");
            transform.Translate(transform.forward * Time.deltaTime * playerSpeed); 
            up = false;
        }
        else if(down){
            ChangeAnimState("WalkForward");
            transform.Translate(-transform.forward * Time.deltaTime * playerSpeed); 
            down = false;
        }
        else if(left){
            ChangeAnimState("WalkLeft");
            transform.Translate(-transform.right * Time.deltaTime * playerSpeed); 
            left = false;
        }
        else if(right){
            ChangeAnimState("WalkRight");
            transform.Translate(transform.right * Time.deltaTime * playerSpeed); 
            right = false;
        }
        else {
            ChangeAnimState("Idle");
        }

    }

    void ChangeAnimState(string state){
        if(state == "Idle"){
            switch(currentState){
                case("WalkForward"):
                    state = "IdleForward";
                    break;
                case("WalkBack"):
                    state = "IdleBack";
                    break;
                case("WalkRight"):
                    state = "IdleRight";
                    break;
                case("WalkLeft"):
                    state = "IdleLeft";
                    break;
            }
        }
        if(currentState == state){
            return;
        }
        currentState = state;
        _animator.Play(state);
    }
}
