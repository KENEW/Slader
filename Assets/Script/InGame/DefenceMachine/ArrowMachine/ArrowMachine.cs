using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArrowMachine : DefenceMachine
{
    public GameObject arrowReady;
	
    [SerializeField] private Vector3 posOffSet;
    [SerializeField] Direction arrowDir = Direction.Down;
	
    private float speed = 4.5f;

    private void Start()
    {
        operatefunc = ArrowFire;
        readyfunc = ArrowReady;
    }
    [PunRPC]
    private void ArrowFire()
    {
        arrowReady.SetActive(false);
		
        GameObject arrow = ObjectManager.Instance.projectilePool.ObjectDequeue("DefenceArrow");
        arrow.transform.position = transform.position + posOffSet;
        arrow.GetComponent<DefenceArrow>().GetInfo(100, false, GameUtility.DirConv(arrowDir), speed, 1,
            GameUtility.angleConv(arrowDir) + 180, 1.0f);
    }
    private void ArrowReady()
    {
        arrowReady.SetActive(true);
    }
}