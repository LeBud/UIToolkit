using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
   public static UserInput instance{get ; private set;}

   private PlayerInput playerInput;
   private InputAction moveAction;
   private InputAction attackAction;
   private InputAction crouchAction;
   private InputAction jumpAction;
   
   private void Awake() {
      if(instance == null)
         instance = this;
      
      playerInput = GetComponent<PlayerInput>();
      SetupInputActions();
   }

   private void SetupInputActions() {
      moveAction = playerInput.actions["Move"];
      attackAction = playerInput.actions["Attack"];
      crouchAction = playerInput.actions["Crouch"];
      jumpAction = playerInput.actions["Jump"];
   }
}
