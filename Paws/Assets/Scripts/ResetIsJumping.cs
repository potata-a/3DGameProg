/*
Author: Ahmad Aliff
Student ID: 1221309548
Date Created: 23 May 2026
*/
// Adapted from: https://www.youtube.com/watch?v=gdp-O6z8x28&list=PLD_vBJjpCwJsqpD8QRPNPMfVUpPFLVGg4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetBool("isJumping", false);
    }

}
