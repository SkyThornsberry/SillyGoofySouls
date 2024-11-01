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
     float MaxHealth = 500;
     float MaxStamina = 400;

    //health/stam bar hud control
    public Slider HealthSlider;
    public Slider StaminaSlider;
    public Slider MaxHealthSlider;
    public Slider MaxStaminaSlider;
    public Slider HealthLossSlider;
    public Slider StaminaLossSlider;
    Coroutine HealthLossSliderCoroutine;
    Coroutine StaminaLossSliderCoroutine;
    bool readyforCoroutine = true;
    public float lossBarDelayTimer = 0f;
    public float lossBarDelayTimerStam = 0f;
    bool stamReadyForCoroutine = true;
    bool maxHealthChanging = false;
    bool maxStamChanging = false;
    public float stamRegen = 65f;
    public float stamRegenDelayTimer = 0f;

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

        /*MaxHealth = 500;
        MaxStamina = 400f;*/
        setMaxHealth(MaxHealth);
        setMaxStam(MaxStamina);
        
    }

    // Update is called once per frame
    void Update()
    {
        //jump needs to be called from update since fixedUpdate is bad for taking inputs
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

        DelayTimersMeth();
        staminaRegen();
        
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
        if(Stamina >= 50f)
        {
            playerRigBod.linearVelocity = new Vector3(playerRigBod.linearVelocity.x, 0f, playerRigBod.linearVelocity.z);
            playerRigBod.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerRigBod.AddForce(body.transform.right * jumpForce, ForceMode.Impulse);
            setCurStam(Stamina - 50f);
        }
        
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
        maxHealthChanging = true;
        MaxHealthSlider.value = (maxHealth < 300) ? maxHealth/ 1000f + 005f: ( (maxHealth<650) ? (maxHealth/1000f)+.0025f : maxHealth/1000f +0f ); //little extra wiggle room so that the fill doesnt overflow the bar 1000-995 = +0, 995-400 = +.005, 400-0 = + 0.0025
        if (Health > maxHealth)
            setCurHealth(maxHealth);
        setHealthLossValue();
        maxHealthChanging = false;
    }

    void setMaxStam(float maxStam)
    {
        maxStamChanging = true;
        MaxStaminaSlider.value = (maxStam < 300) ? maxStam / 1000f + 005f : ((maxStam < 650) ? (maxStam / 1000f) + .0025f : maxStam / 1000f + 0f); //little extra wiggle room so that the fill doesnt overflow the bar
        if (Stamina > maxStam)
            setCurStam(maxStam);
        setStamLossValue();
        maxStamChanging = false;
    }

    void setCurHealth(float NewHealth)
    {
        float oldHealthValue = Health;
        Health = NewHealth;
        HealthSlider.value = NewHealth / 1000f;
        if (oldHealthValue > NewHealth & !maxHealthChanging)//new health is less = damaged not healed
        {
            
            lossBarDelayTimer = 0f;//reset the delay timer. do this whether or not co routine is already running
            HealthLossSliderCoroutine = StartCoroutine(GradualFloatSetHealthLossValue(oldHealthValue, NewHealth , -2.5f));// if damaged slowly lower the health loss bar to cur health
            
            //add a conditional w/ a timer to stop the coroutine if this method is called again before the co routine finishes - may cause issues otherwise
            //add a conditional to not trigger when setting max health
        }
        else
        {
            
            setHealthLossValue();//if healed, just set the health loss bar to the new health.
        }

    }

    void setCurStam(float newStam)
    {
        float oldStamValue = Stamina;
        Stamina = newStam;
        StaminaSlider.value = newStam / 1000f;
        if (oldStamValue > newStam & !maxStamChanging)//new stam is less = damaged not healed, dont activate coroutine if setting max stam either
        {
            Debug.Log("EnteredSetCurStamIf");
            lossBarDelayTimerStam = 0f;//reset the delay timer. do this whether or not co routine is already running
            stamRegenDelayTimer = 0f;//reset the delay timer for stamina regen
            StaminaLossSliderCoroutine = StartCoroutine(GradualFloatSetStamLossValue(oldStamValue, newStam, -2.5f));// if damaged slowly lower the health loss bar to cur health
        }
        else
        {
            
            //setStamLossValue();//if healed, just set the health loss bar to the new health.
        }
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

    public IEnumerator GradualFloatSetStamLossValue(float startValue, float endValue, float rateChange)//gradually lower the health loss bar to a target value
    {
        if (stamReadyForCoroutine)//prevents multiple coroutines running at the same time, that causes funky behavior
        {
            setStamLossValue(startValue);
            stamReadyForCoroutine = false;
            Debug.Log("entered the coroutine");
            //start with an int that constantly counts down and is reset to 5 when damage is taken only proceed once damage has not been taken for a few seconds NEEDS IMPLEMENTING
            float curValue = startValue;
            if (startValue < Stamina & rateChange > 0)//if the goal end value is greater than the start value
            {
                while (!(curValue + rateChange < Stamina))//increment by rate of change once every .01s until cur value is within one ratechange of end value
                {
                    if (lossBarDelayTimerStam > 1.25f)//adds delay, delay resets if damage is taken again
                    {
                        setStamLossValue(curValue);
                        curValue += rateChange;
                    }
                    yield return new WaitForSeconds(.01f);

                }
                setStamLossValue(Stamina);//round to end value
            }
            else if (startValue > Stamina & rateChange < 0)//if the goal end value is less than the start value
            {
                Debug.Log("first loop entered");
                while (!(curValue + rateChange < Stamina))//increment by rate of change once ever hundredth of a second untilthe curValue is within one rateChange of the end value
                {
                    if (lossBarDelayTimerStam > 1.25f)//adds delay, delay resets if damage is taken again
                    {
                        Debug.Log("second loop entered");
                        setStamLossValue(curValue);
                        curValue += rateChange;
                    }
                    yield return new WaitForSeconds(.01f);

                }
                Debug.Log("coroutine end-setting stam to " + Stamina);
                setStamLossValue(Stamina);//round to end value
                
            }
            else
                Debug.Log("invalid input Combo");//only accessible if rate change would take startValue away from endValue
            stamReadyForCoroutine = true;
        }
    }

    public IEnumerator GradualFloatSetHealthLossValue(float startValue, float endValue, float rateChange)//gradually lower the health loss bar to a target value
    {
        if (readyforCoroutine)//prevents multiple coroutines running at the same time, that causes funky behavior
        {
            setHealthLossValue(startValue);
            readyforCoroutine = false;
            //start with an int that constantly counts down and is reset to 5 when damage is taken only proceed once damage has not been taken for a few seconds NEEDS IMPLEMENTING
            float curValue = startValue;
            if (startValue < Health & rateChange > 0)//if the goal end value is greater than the start value
            {
                while (!(curValue + rateChange < Health))//increment by rate of change once every .01s until cur value is within one ratechange of end value
                {
                    if(lossBarDelayTimer > 1.25f)//adds delay, delay resets if damage is taken again
                    { 
                        setHealthLossValue(curValue);
                        curValue += rateChange;
                    }
                    yield return new WaitForSeconds(.01f);
                    
                }
                setHealthLossValue(Health);//round to end value
            }
            else if (startValue > Health & rateChange < 0)//if the goal end value is less than the start value
            {
                while (!(curValue + rateChange < Health))//increment by rate of change once ever hundredth of a second untilthe curValue is within one rateChange of the end value
                {
                    if(lossBarDelayTimer > 1.25f)//adds delay, delay resets if damage is taken again
                    { 
                        setHealthLossValue(curValue);
                        curValue += rateChange;
                    }
                        yield return new WaitForSeconds(.01f);
                    
                }
                setHealthLossValue(Health);//round to end value
            }
            else
                Debug.Log("invalid input Combo");//only accessible if rate change would take startValue away from endValue
            readyforCoroutine = true;
        }
    }

    public void DelayTimersMeth()
    {
        if (lossBarDelayTimer < 5)
        {
            lossBarDelayTimer += Time.deltaTime;
        }
        if(lossBarDelayTimerStam < 5)
        {
            lossBarDelayTimerStam += Time.deltaTime;
        }
        if (stamRegenDelayTimer < 5)
        {
            stamRegenDelayTimer += Time.deltaTime;
        }
    }

    public void staminaRegen()
    {
        if(Stamina + Time.deltaTime * stamRegen < MaxStamina & stamRegenDelayTimer > 1.75f)
        {
            setCurStam(Stamina += Time.deltaTime * stamRegen);

        }
        else if( Stamina < MaxStamina & Stamina + Time.deltaTime * stamRegen > MaxStamina & stamRegenDelayTimer > 1.75f)
        {
            setCurStam(MaxStamina);
        }
    }

}
