using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class CameraController : ServerSingleton<CameraController>
{ 
    public Camera cameraObject;
    public Transform target = null;

    [SerializeField] private float Speed = 5.0f;
    [SerializeField] private float Scale = 5.0f;

    private float shakePower = 0.0f;
    private float shakeAmount = 7.5f;

    private bool isXShake = true;
    private bool isYShake = true;

    private float angleAmount = 0.0f;
    private float scaleAmount = 1.0f;

    public Vector3 Offset = Vector3.zero;
    private Vector3 anchorPoint = Vector3.zero;

    private int playerNum = 0;

	private void LateUpdate()
    {
        anchorPoint = Vector3.Lerp(anchorPoint, target.position, Speed * Time.deltaTime);
        shakePower -= shakePower / shakeAmount;
        
        Vector3 shakeVec = Vector3.zero;
        shakeVec.x = (isXShake) ? Random.Range(-shakePower, shakePower) : 0.0f;
        shakeVec.y = (isYShake) ? Random.Range(-shakePower, shakePower) : 0.0f;
        transform.position = anchorPoint + Offset + shakeVec;
        
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0.0f, 0.0f, angleAmount), 5.0f * Time.deltaTime);
        cameraObject.orthographicSize = Mathf.Lerp(cameraObject.orthographicSize, Scale * scaleAmount, 5.0f * Time.deltaTime);
    }
    public void SetTarget(int playerNum, Transform target)
    {
        this.target = target;
        this.playerNum = playerNum;
    }
    public void OnShake(int playerNum, float power, float amount, bool xshake = true, bool yshake = true)
    {
        if(playerNum != this.playerNum)
        {
            return;
        }

        shakePower = power;
        shakeAmount = amount;
        isXShake = xshake;
        isYShake = yshake;
    }
}
