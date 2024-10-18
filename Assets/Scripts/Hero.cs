using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; } //singleton
    [Header("Inscribed")]
    //the fields that control movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;


    [Header("Dynamic")][Range(0,4)][SerializeField]
    private float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        } else
        {
            Debug.LogError("Hero.Awake() - Attempt to assign second Hero.S");
        }
    }
 public void RechargeShield()
    {
        // Increase the shield level by 1, up to a maximum of 4 (or your desired maximum)
        shieldLevel = Mathf.Min(shieldLevel + 1, 4);
        Debug.Log("Shield recharged to level: " + shieldLevel);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //pull in info from the input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        //change our trans.pos based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        //rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult,0);

        //allow the ship to fire
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }
    }
    void TempFire()
    {
        GameObject projGo = Instantiate<GameObject>(projectilePrefab);
        projGo.transform.position = transform.position;
        Rigidbody rigidB = projGo.GetComponent<Rigidbody>();
        rigidB.velocity = Vector3.up * projectileSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //Debug.Log("Shield trigger hit by: " + go.gameObject.name);
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            shieldLevel--;
            Destroy(go);
        } else
        {
            Debug.LogWarning("Shield trigger hit by non-Enemy" + go.name);
        }
    }
    public float shieldLevel
    {
        get { return (_shieldLevel); }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject); //Destroy the Hero, because our shield were gone.
                Main.HERO_DIED();
            }
        }
    }
}