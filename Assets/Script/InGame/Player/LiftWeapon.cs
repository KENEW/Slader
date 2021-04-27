using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class LiftWeapon : ServerSingleton<LiftWeapon>
{
    public SpriteRenderer sprRend;
    public GameObject handWeapon;
    public GameObject hand;

    public VariableJoystick joyStick;
    public Player player;

    private int flipNum = 1;
    private float ContinuousAngle = 0f;
    public bool ContinuousAttack = false;

    float weaponAngle;

    public Sprite[] oneHandSpr;
    public Dictionary<string, int> oneHandWeapon = new Dictionary<string, int>();
    public GameObject[] weaponObj;

    public int curWeapon = 0;
    public Animator bowAnimator;
    private bool weaponFlip = false;

    private const float weaponAngleOffsetOn = -145;
    private const float weaponAngleOffsetOff = 5;


    private void Start()
    {
        joyStick = GameObject.Find("AttackJoyStick").GetComponent<VariableJoystick>();

        oneHandWeapon.Add("검", 0);
        oneHandWeapon.Add("스태프", 1);

        Quaternion t_euler = Quaternion.Euler(0, 0, 120);
        handWeapon.transform.rotation = t_euler;
    }
    public void SetPlayer(int playerNum)
    {
        player = PlayerManager.Instance.GetPlayer(playerNum);
    }
    private void Update()
    {
        if (!photonView.IsMine || !joyStick.dragCheck)
        {
            return;
        }
        switch (curWeapon)
        {
            case 0:
            case 2:
                weaponAngle = (((joyStick.Direction.y + 1.0f) * 90) + ContinuousAngle) * flipNum;
                if (joyStick.Direction.x >= 0.0f)
                {
                    weaponFlip = false;
                }
                else
                {
                    weaponFlip = true;
                }
                Quaternion eulerOne = Quaternion.Euler(0, 0, weaponAngle);
                handWeapon.transform.rotation = eulerOne;
                if (weaponFlip)
                {
                    player.transform.localScale = new Vector3(-1, 1, 1);
                    flipNum = -1;
                }
                else
                {
                    player.transform.localScale = new Vector3(1, 1, 1);
                    flipNum = 1;
                }

                break;
            case 1:
                weaponAngle = Mathf.Atan2(joyStick.Direction.y, joyStick.Direction.x) * Mathf.Rad2Deg;
                if (joyStick.Direction.x >= 0.0f)
                {
                    weaponFlip = false;
                }
                else
                {
                    weaponFlip = true;
                }
                
                Quaternion eulerTwo = Quaternion.Euler(0, 0, weaponAngle);
                weaponObj[1].transform.rotation = eulerTwo;
                if (weaponFlip)
                {
                    player.transform.localScale = new Vector3(-1, 1, 1);
                    weaponObj[1].transform.localScale = new Vector3(-1, 1, 1);
                    flipNum = -1;
                }
                else
                {
                    player.transform.localScale = new Vector3(1, 1, 1);
                    weaponObj[1].transform.localScale = new Vector3(1, 1, 1);
                    flipNum = 1;
                }
                break;
        }
    }
    public void LiftWeaponAttack()
    {
        if(curWeapon == 1)
        {
            //GetComponent<PhotonView>().RPC("OnBowAnimation", RpcTarget.All, false);
            bowAnimator.SetBool("OnBend", false);
        }
        else
        {
            ContinuousAttack = !ContinuousAttack;
            hand.transform.localRotation = Quaternion.Euler(0, 0, ContinuousAttack ? weaponAngleOffsetOn : weaponAngleOffsetOff);
        }
    }
    public void LiftWeaponReady()
    {
        if (curWeapon == 1)
        {
            //GetComponent<PhotonView>().RPC("OnBowAnimation", RpcTarget.All, true);
            bowAnimator.SetBool("OnBend", true);
        }
    }
    public void WeaponChange(int weaponIndex)
    {
        curWeapon = weaponIndex;

        foreach (GameObject t_obj in weaponObj)
        {
            t_obj.SetActive(false);
        }
        switch (MyData.Instance.weaponInfo[curWeapon].type)
        {
            case "ONE_HAND":
                weaponObj[0].SetActive(true);
                sprRend.sprite = oneHandSpr[oneHandWeapon[MyData.Instance.weaponInfo[curWeapon].name]];
                break;
            case "BOW":
                weaponObj[1].SetActive(true);
                break;
        }
    }
}