using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// 플레이어가 장착한 무기를 관리합니다.
/// </summary>
public class LiftWeapon : ServerSingleton<LiftWeapon>
{
    private const float weaponAngleOffsetOn = -145;
    private const float weaponAngleOffsetOff = 5;
    
    public SpriteRenderer sprRend;
    public GameObject handWeapon;
    public GameObject hand;
    public Animator bowAnimator;
    
    public VariableJoystick joyStick;
    public Player player;

    public int curWeapon = 0;
    public bool continuousAttack = false;
    
    private bool weaponFlip = false;
    private float continuousAngle = 0f;
    private float weaponAngle;
    private int flipNum = 1;

    public Sprite[] oneHandSpr;
    public Dictionary<string, int> oneHandWeapon = new Dictionary<string, int>();
    public GameObject[] weaponObj;
    
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
                weaponAngle = (((joyStick.Direction.y + 1.0f) * 90) + continuousAngle) * flipNum;
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
    /// <summary>
    /// 무기를 가지고 공격을 처리
    /// </summary>
    public void LiftWeaponAttack()
    {
        if(curWeapon == 1)
        {
            bowAnimator.SetBool("OnBend", false);
        }
        else
        {
            continuousAttack = !continuousAttack;
            hand.transform.localRotation = Quaternion.Euler(0, 0, continuousAttack ? weaponAngleOffsetOn : weaponAngleOffsetOff);
        }
    }
    /// <summary>
    /// 무기를 준비를 처리
    /// </summary>
    public void LiftWeaponReady()
    {
        if (curWeapon == 1)
        {
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