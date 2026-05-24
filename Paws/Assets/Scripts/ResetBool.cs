/*
Author: Ahmad Aliff
Student ID: 1221309548
Date Created: 23 May 2026
*/
// Adapted from: https://www.youtube.com/watch?v=gdp-O6z8x28&list=PLD_vBJjpCwJsqpD8QRPNPMfVUpPFLVGg4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;
    public bool isInteractingStatus;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetBool(isInteractingBool, isInteractingStatus);
    }
}
