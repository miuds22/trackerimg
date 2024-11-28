using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //"Stats" del proyectil
    public float speed;
    public int attackPower;
    public float lifetime;

    //Referencia al drone que lo dispara, para apuntarle luego
    public GameObject parent;

    //Referencia al manager para avisarle cosas
    BattleManager bm;

    // Start is called before the first frame update
    void Start()
    {
        //Buscamos en la escena al BM por componente
        bm = FindObjectOfType<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bm.pause) return;

        //Up en vez de forward porque asi esta rotado el GameObject
        transform.position += transform.up * speed * Time.deltaTime;

        //Le restamos a la duracion del proyectil el tiempo que pasa
        //No usamos Destroy(lifetime) en Start para que se "congele" con la pausa
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        //Si le pega al jugador, le hace daño
        if(other.GetComponent<Player>())
        {
            other.GetComponent<Player>().TakeDamage(attackPower);
            Destroy(gameObject);
        }

        //Si le pega al drone, lo destruye (usamos Parent por como esta armado el prefab del drone)
        else if(other.GetComponentInParent<Drone>())
        {
            Destroy(other.transform.parent.gameObject);
            Destroy(gameObject);
        }

        //Si le pega a la hitbox del sable reflejando, cambia su direccion
        else if(other.CompareTag("Reflector"))
        {
            //Reproducimos tambien el sonido de ser reflejado
            GetComponent<AudioSource>().Play();

            //Si el drone existe, lo mira, y si no, sale en una direccion random
            //El drone podria no existir si llego a disparar mas de una vez y reflejamos los dos tiros
            if(parent)
            {
                transform.up = (parent.transform.position - transform.position).normalized;
            }            
            else
            {
                transform.up = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
            }
        }

    }
}
