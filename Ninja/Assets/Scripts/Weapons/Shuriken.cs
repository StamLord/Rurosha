using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : WeaponObject
{
    public GameObject prefab;
    [SerializeField] private WeaponManager _weaponManager;

    public int burstAmount = 3;
    public float burstRange = 3f;
    public float burstSpread = 45f;
    public float lastShot;
    //public float shootRate = .4f;

    [SerializeField] AttributeDependant<float> maxAngle = new AttributeDependant<float>("Dexterity", new float[]{35, 30, 25, 20, 16, 12, 8, 5, 3, 1});
    //[SerializeField] private float[] maxAnglePerDexterity = {35, 30, 25, 20, 16, 12, 8, 5, 3, 1};
    [SerializeField] AttributeDependant<float> shootRate = new AttributeDependant<float>("Agility", new float[]{2f, 1.75f, 1.5f, 1.25f, 1f, .8f, .6f, .5f, .4f, .35f});
    //[SerializeField] private float[] shootRatePerAgility = {2f, 1.75f, 1.5f, 1.25f, 1f, .8f, .6f, .5f, .4f, .35f};

    [SerializeField] private float agilityExpGain = 1f;
    [SerializeField] private float dexterityExpGain = 2f;
    [SerializeField] private float dexterityExpMaxDistance = 20f;
    
    private new Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Time.time >= lastShot + shootRate.GetValue(_weaponManager.Stats))
            {
                GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);

                RaycastHit hit;
                Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 20f);
                
                float distance = 0;

                if(hit.collider)
                {
                    obj.transform.forward = hit.point - transform.position;
                    distance = Vector3.Distance(hit.point, camera.transform.position);
                }
                else
                {
                    obj.transform.forward = camera.transform.forward;
                    distance = dexterityExpMaxDistance;
                }

                float angle = maxAngle.GetValue(_weaponManager.Stats);
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(-angle, angle), Random.Range(-angle, angle));

                Debug.Log(randomRotation);
                Debug.Log(angle);

                obj.transform.forward = randomRotation * obj.transform.forward;

                lastShot = Time.time;

                _weaponManager.DepleteItem(1);

                UseAnimation();

                distance = Mathf.Max(0.1f, distance / dexterityExpMaxDistance);
                Debug.Log(distance);
                _weaponManager.Stats.IncreaseAttributeExp("Dexterity", dexterityExpGain * distance);
                _weaponManager.Stats.IncreaseAttributeExp("Agility", agilityExpGain);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            float rangeStep = burstRange / burstAmount;
            Vector3 xFirstStep = transform.position - transform.right * burstRange / 2;

            float angleStep = burstSpread / burstAmount;
            float firstAngleStep = 0 - angleStep * burstAmount / 2;

            int availableAmmo = Mathf.Min(burstAmount, _weaponManager.GetAmmo());

            for(int i = 0; i < availableAmmo; i++)
            {
                GameObject obj = Instantiate(prefab, xFirstStep + transform.right * rangeStep * i, Quaternion.identity);
                obj.transform.forward = Quaternion.AngleAxis(firstAngleStep + angleStep * i, Vector3.up) * Camera.main.transform.forward;

                _weaponManager.DepleteItem(1);
            }
        }

    }
}


