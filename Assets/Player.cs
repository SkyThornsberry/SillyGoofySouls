using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject body;
    public Rigidbody playerRigBod;
    public Animator animator;
    


    //player stats
    public float Health =1000f;
    public float Stamina=1000f;
    public float MaxHealth = 350;
    public float MaxStamina = 225;
    public Slider HealthSlider;
    public Slider StaminaSlider;
    public Slider MaxHealthSlider;
    public Slider MaxStaminaSlider;
    public Slider HealthLossSlider;
    public Slider StaminaLossSlider;
    Coroutine HealthLossSliderCoroutine;
    Coroutine StaminaLossSliderCoroutine;


    //movement vars
    public float speed = 175f;
    public float jumpForce = 7.5f;
    public float groundDrag = 20f;
    public float maxMoveSpeed = 7f;
    public float maxSlopeAngle =40f;
    private RaycastHit slopeHit;
    Vector3 moveDirection;

    //groundcheck vars
    float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;
    Vector3 rayCastPos;

    //camera rotation
    float rotationX = 0f;
    public float sensitivity = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        body = GameObject.FindGameObjectWithTag("Body");
        playerRigBod = body.GetComponent<Rigidbody>();
        maxSlopeAngle = 40f;

        MaxHealth = 200f;
        MaxStamina = 150f;
        setMaxHealth(MaxHealth);
        setMaxStam(MaxStamina);
        
    }

    // Update is called once per frame
    void Update()
    {
        //jump needs to be called from update for whatever reason
        if ((Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("space pressed");
            if (grounded)
            {
                Jump();
                Debug.Log("jumped");
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Attack Controls

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LightAttack");
            Debug.Log("mousebuttondowntrue");
            
        }

    }

    void FixedUpdate() {



        //Movement Controls
        //Vector3 newVelocity = Vector3 (0,1,0) * playerRigBod.linearVelocity.y; //set x+z velocity to 0 while maintaing vertical velocity




        float zDirection = Input.GetAxis("Vertical");
        float xDirection = Input.GetAxis("Horizontal");


        moveDirection = body.transform.forward * -xDirection + body.transform.right * zDirection;


        if (onSlope())
        {
            Debug.Log("onSlopeTrue");
            playerRigBod.AddForce(GetSlopeMoveDirection() * speed * 1f, ForceMode.Force);
        }

        //apply movement force
        else if (grounded)
        {
            
            playerRigBod.AddForce(moveDirection.normalized * speed);
        }

        

        

        groundcheck();// give appropiate drag + keep from running on air

        speedControl(); //add max speed



        

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Camera Controls

        //rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        body.transform.localEulerAngles = new Vector3(0, rotationX, 0);


    }


    void groundcheck()
    {
        //Debug.Log("groundcheck performed");
        //rayCastPos = body.transform.position - Vector3.up;
        grounded = Physics.Raycast(body.transform.position, Vector3.down, playerHeight *.5f + .1f, whatIsGround);
        //Debug.DrawRay(body.transform.position, Vector3.down, Color.red);
        if (grounded)
        {
            playerRigBod.linearDamping = groundDrag;
           // 
        }
        else
        {
            
            playerRigBod.linearDamping = .5f;
        }
            
    }

    void speedControl()
    {
        Vector3 flatVel = new Vector3(playerRigBod.linearVelocity.x, 0f, playerRigBod.linearVelocity.z);
        //find current velo

        //limit if over speed cap
        if (flatVel.magnitude > maxMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxMoveSpeed;
            playerRigBod.linearVelocity = new Vector3(limitedVel.x, playerRigBod.linearVelocity.y, limitedVel.z);
        }


    }

    void Jump()
    {
        playerRigBod.linearVelocity = new Vector3(playerRigBod.linearVelocity.x, 0f, playerRigBod.linearVelocity.z);
        playerRigBod.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerRigBod.AddForce(body.transform.right * jumpForce, ForceMode.Impulse);
    }

    private bool onSlope()
    {

        if (Physics.Raycast(body.transform.position, Vector3.down, out slopeHit, playerHeight * .5f + .15f))
        {
            //Debug.DrawRay(body.transform.position, Vector3.down, Color.green, .1f);
            
            float angle = Mathf.Abs(Vector3.Angle(Vector3.down, slopeHit.normal) -180);
            //Debug.Log(angle);
            return (angle) < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void setMaxHealth(float maxHealth)
    {
        MaxHealthSlider.value = (maxHealth > 995) ? maxHealth/ 1000f : (maxHealth/1000f)+.005f; //little extra wiggle room so that the fill doesnt overflow the bar
        if (Health > maxHealth)
            setCurHealth(maxHealth);
    }

    void setMaxStam(float maxStam)
    {
        MaxStaminaSlider.value = (maxStam > 995) ? maxStam / 1000f : maxStam/1000f + .005f; //little extra wiggle room so that the fill doesnt overflow the bar
        if (Stamina > maxStam)
            setCurStam(maxStam);
    }

    void setCurHealth(float curHealth)
    {
        HealthSlider.value = curHealth / 1000f;
        if (Health > curHealth)//new health is less = damaged not healed
        {
            Debug.Log("EnteredSetCurHealthLoop");
            HealthLossSliderCoroutine = StartCoroutine(GradualFloatSetHealthLossValue(Health, curHealth, -1f));// if damaged slowly lower the health loss bar to cur health
            Health = curHealth;
            //add a conditional w/ a timer to stop the coroutine if this method is called again before the co routine finishes - may cause issues otherwise
            //add a conditional to not trigger when setting max health
        }
        else
        {
            Debug.Log("entered the wrong loop in setcurhealth");
            Health = curHealth;
            setHealthLossValue();//if healed, just set the health loss bar to the new health.
        }

    }

    void setCurStam(float curStam)
    {
        StaminaSlider.value = curStam / 1000f;
        Stamina = curStam;
        Invoke("setStamLossValue", .5f);
    }

    void setHealthLossValue()//sets the health loss slider value to current health
    {
        HealthLossSlider.value = Health / 1000f;
    }

    void setStamLossValue()//sets the stamloss slider value to current stamina
    {
        StaminaLossSlider.value = Stamina / 1000f;
    }

    void setStamLossValue(float newStam)//sets stamina loss slider to a specified value
    {
        StaminaLossSlider.value = newStam / 1000f;
    }

    void setHealthLossValue(float newHealth)//sets the health loss slider to a specified float
    {
        HealthLossSlider.value = newHealth / 1000f;
    }

    public void playerHurt(float dmg, string type)
    {
        //do damage calculations based off defenses and stuff, + do any visual/ audio indicators
        //Health = Health - dmg;
        setCurHealth(Health -dmg);
    }

    public IEnumerator GradualFloatSetStamLossValue(float startValue, float endValue, float rateChange)//gradually lower the stam loss bar to a target value
    {
        //start with an int that constantly counts down and is reset to 5 when damage is taken only proceed once damage has not been taken for a few seconds NEEDS IMPLEMENTING
        float curValue = startValue;
        if (startValue < endValue & rateChange > 0)//if the goal end value is greater than the start value
        {
            while (curValue + rateChange < endValue)//increment by rate of change once every .01s until cur value is within one ratechange of end value
            {
                setStamLossValue(curValue);
                curValue += rateChange;
                yield return new WaitForSeconds(.01f);
            }
            setStamLossValue(endValue);//round to end value
        }
        else if (startValue > endValue & rateChange < 0)//if the goal end value is less than the start value
        {
            while (curValue + rateChange < endValue)//increment by rate of change once ever hundredth of a second untilthe curValue is within one rateChange of the end value
            {
                setStamLossValue(curValue);
                curValue += rateChange;
                yield return new WaitForSeconds(.01f);
            }
            setStamLossValue(endValue);//round to end value
        }
        else
            Debug.Log("invalid input Combo");//only accessible if rate change would take startValue away from endValue
    }

    public IEnumerator GradualFloatSetHealthLossValue(float startValue, float endValue, float rateChange)//gradually lower the health loss bar to a target value
    {
        Debug.Log("entered the coroutine");
        //start with an int that constantly counts down and is reset to 5 when damage is taken only proceed once damage has not been taken for a few seconds NEEDS IMPLEMENTING
        float curValue = startValue;
        if (startValue < endValue & rateChange > 0)//if the goal end value is greater than the start value
        {
            while (curValue + rateChange < endValue)//increment by rate of change once every .01s until cur value is within one ratechange of end value
            {
                setHealthLossValue(curValue);
                curValue += rateChange;
                yield return new WaitForSeconds(.5f);
            }
            setHealthLossValue(endValue);//round to end value
        }
        else if (startValue > endValue & rateChange < 0)//if the goal end value is less than the start value
        {
            Debug.Log("first loop entered");
            while (!(curValue + rateChange < endValue))//increment by rate of change once ever hundredth of a second untilthe curValue is within one rateChange of the end value
            {
                Debug.Log("second loop entered");
                setHealthLossValue(curValue);
                curValue += rateChange;
                yield return new WaitForSeconds(.01f);
            }
            setHealthLossValue(endValue);//round to end value
        }
        else
            Debug.Log("invalid input Combo");//only accessible if rate change would take startValue away from endValue
    }


}
