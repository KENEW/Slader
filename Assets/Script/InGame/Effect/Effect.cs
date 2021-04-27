using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] private string poolName;
    [SerializeField] protected float endTime = 1.0f;

    public bool autoDestroy = true;
    private bool poolInit = false;

    private void OnEnable()
    {
        if(poolInit)
        {
            if (autoDestroy)
            {
                CoroutineManager.Instance.CallWaitForSeconds(endTime, () => {
                    ObjectManager.Instance.effectPool.ObjectEnqueue(poolName, gameObject);
                });
            }
        }
    }
    private void OnDisable()
    {
        if (!poolInit)
            poolInit = true;
    }
}