﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model; 
    public PlayerInput pi;
    public float walkSpeed=2.4f;
    public float runMultiplier=2.7f;
    public float jumpVelocity=5.0f;
    public float rollVelocity=1.0f;
    
   [Space(10)]
   [Header("=====Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlanar=false;
    private CapsuleCollider col;
    private float lerpTarget;
    private Vector3 deltaPos;
     void Awake()
    {
        
        pi=GetComponent<PlayerInput>();
        anim=model.GetComponent<Animator>();
        rigid=GetComponent<Rigidbody>();
        col=GetComponent<CapsuleCollider>();
    
    }

   
    void Update()//Time.deltaTime 1/60
    {
      
        //print(pi.Dup);
        anim.SetFloat("forward",pi.Dmag*Mathf.Lerp(anim.GetFloat("forward"),((pi.run)?2.0f:1.0f),0.5f));
        if(rigid.velocity.magnitude>0.1f&&pi.jump)
        {
          anim.SetTrigger("roll");
        }
        if(pi.jump)
        {
         anim.SetTrigger("jump");
         canAttack=false;
        }
        if(pi.attack&&CheckState("ground")&&canAttack)
        {
          anim.SetTrigger("attack");
        }
        if(pi.Dmag>0.1f)
        {
          model.transform.forward=  Vector3.Slerp(model.transform.forward,pi.Dvec,0.3f);
        }
       if(lockPlanar==false)
       {
        planarVec=pi.Dmag*model.transform.forward*walkSpeed*((pi.run)?runMultiplier:1.0f);
       }
     
    }

    void FixedUpdate() //fixedDeltaTime 1/50
    {
      rigid.position+=deltaPos;
      //rigid.position+=movingVec*Time.fixedDeltaTime;
      rigid.velocity=new Vector3(planarVec.x,rigid.velocity.y,planarVec.z)+thrustVec;
      thrustVec=Vector3.zero;
      deltaPos=Vector3.zero;    
    }
    
    private bool CheckState(string stateName,string layerName="Base Layer")
    {
       return   anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName); 
    }


  ///
  ///Meaage processing block
  ///


    public void OnJumpEnter()
    {
      thrustVec=new Vector3(0,jumpVelocity,0);
      pi.inputEnabled=false;
      lockPlanar=true;
    }
    
    public void IsGround()
    {
      anim.SetBool("isGround",true);
    }
    public void IsNotGround()
    {
      anim.SetBool("isGround",false);
    }
    public void OnGroundEnter()
    {
      pi.inputEnabled=true;
      lockPlanar=false;
      canAttack=true;
      col.material=frictionOne;
    }

    public void OnGroundExit()
    {
          col.material=frictionZero;
    }
        public void OnFallEnter()
    {
      pi.inputEnabled=false;
      lockPlanar=true;
    }
    public void OnRollEnter()
    {
       thrustVec=new Vector3(0,rollVelocity,0);
      pi.inputEnabled=false;
      lockPlanar=true;
    }
    public void OnJabEnter()
    {
      pi.inputEnabled=false;
      lockPlanar=true;
    }
    public void OnJabUpdate()
    {
      thrustVec=model.transform.forward*anim.GetFloat("jabVelocity");
    }
    public void OnAttack1hAEnter()
    {
      pi.inputEnabled=false;
      //lockPlanar=true;
      lerpTarget=1.0f;
    }
    public void OnAttack1hAUpdate()
    {
      thrustVec=model.transform.forward*anim.GetFloat("attack1hAVelocity");
      anim.SetLayerWeight(anim.GetLayerIndex("attack"),Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")),lerpTarget,0.4f));
    }
    public void OnAttackIdleEnter()
    {
      pi.inputEnabled=true;
      //lockPlanar=false;
      //anim.SetLayerWeight(anim.GetLayerIndex("attack"),0f);
       lerpTarget=0f;
    }
    public void OnAttackIdleUpdate()
    {
       anim.SetLayerWeight(anim.GetLayerIndex("attack"),Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")),lerpTarget,0.4f));
    }
    public void OnUpdateRM(object _deltaPos)
    {
      if(CheckState("attack1hC","attack"))
      deltaPos+=(Vector3)_deltaPos;
    }
}
