using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    //Cada cuanto dispara, y el contador de ese tiempo
    public float shootTime;
    float shootTimer;

    //Referencias a partes del drone
    public GameObject cannon;
    public Transform spawnPoint;

    //Referencia al manager, player, y a un objeto vacio que va a crear
    BattleManager bm;
    Transform player;
    Transform targetPosition;

    //Prefab del proyectil
    public GameObject projectilePrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        //Buscamos al manager, y creamos un objeto vacio que vamos a usar despues
        bm = FindObjectOfType<BattleManager>();
        targetPosition = new GameObject("Drone Target").transform;

        //Le pedimos al manager que nos de el Transform del jugador
        player = bm.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (bm.pause) return;

        //Contamos el tiempo que pasa y, cuando se cumple suficiente tiempo, dispara y resetea el contador
        shootTimer += Time.deltaTime;
        if(shootTimer >= shootTime)
        {
            shootTimer = 0;
            Shoot();
        }

        //Ubica al objeto vacio en una posicion tomando el XZ del jugador pero la Y del propio drone
        targetPosition.position = new Vector3(player.position.x, transform.position.y, player.position.z);

        //El drone mismo mira hacia el objeto vacio, pero el cañon le apunta a la altura del pecho del jugador
        transform.LookAt(targetPosition);
        cannon.transform.LookAt(bm.targetShoot);
    }

    //Cuando el drone muere, le avisa al manager para que cuente el puntaje. Tambien destruye el objeto vacio que creo al principio
    void OnDestroy()
    {
        bm.ScorePoints();
        if(targetPosition)
        {
            Destroy(targetPosition.gameObject);
        }        
    }

    //Dispara creando un proyectil, alineandolo con el cañon y avisandole que el drone es el objeto que lo creo (ver script Projectile)
    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = spawnPoint.position;
        projectile.transform.up = cannon.transform.forward;
        projectile.GetComponent<Projectile>().parent = gameObject;
        GetComponent<AudioSource>().Play();
    }
}
