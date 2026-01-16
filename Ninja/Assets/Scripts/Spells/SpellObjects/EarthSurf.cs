using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSurf : SpellObject
{
    [SerializeField] private float emergeTime = .5f;
    [SerializeField] private float surfDuration = 5f;

    [SerializeField] private float startY;
    [SerializeField] private float emergedY;
    [SerializeField] private float speed;
    [SerializeField] private Transform platform;
    [SerializeField] private new Collider collider;
    [SerializeField] private ParticleSystem[] vfxCollide;

    [SerializeField] private SpellManager m;

    Rigidbody rb;

    private void Start() 
    {
        Activate(m);
    }

    public override void Activate(SpellManager manager)
    {
        rb = manager.GetComponent<Rigidbody>();
        StartCoroutine("MoveWave");
    }

    private IEnumerator MoveWave()
    {
        float start = Time.time;
        Vector3 startPos = transform.position;
        Vector3 rigidOffset = rb.position - platform.position;
        collider.enabled = true;

        while(Time.time - start < surfDuration)
        {
            float emerge = (Time.time - start) / emergeTime;
            float y = Mathf.Lerp(startY, emergedY, emerge);

            platform.localPosition = new Vector3(0, y, 0);
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
            rb.transform.position = platform.position + rigidOffset;

            yield return null;
        }

        rb.velocity = Vector3.zero;
        collider.enabled = false;

        foreach(ParticleSystem ps in vfxCollide)
        {
            var col = ps.collision;
            col.enabled = true;
        }
    }
}
