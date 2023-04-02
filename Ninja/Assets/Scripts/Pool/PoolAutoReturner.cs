using System.Collections;
using UnityEngine;

public class PoolAutoReturner : MonoBehaviour
{
    public void SelfReturn(Pool pool, float delay)
    {
        StartCoroutine(DelayReturn(pool, delay));
    }

    private IEnumerator DelayReturn(Pool pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Return(gameObject);
    }
}
