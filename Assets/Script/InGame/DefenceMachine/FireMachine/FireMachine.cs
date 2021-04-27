using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireMachine : DefenceMachine
{
    [SerializeField] private Vector3 posOffSet;
    [SerializeField] private Direction fireDir = Direction.Down;

    private float speed = 3.5f;

    private void Start()
    {
        operatefunc = OnFire;
        readyfunc = FireReady;
    }
    [PunRPC]
    private void OnFire()
    {
        GameObject fire = ObjectManager.Instance.projectilePool.ObjectDequeue("DefenceFire");

        fire.transform.position = transform.position + posOffSet;
        fire.GetComponent<DefenceFire>().GetInfo(70, false, GameUtility.DirConv(fireDir), speed, 100, 
            GameUtility.angleConv(fireDir) + 180, 2.0f);
    }
    private void FireReady(){ }
}