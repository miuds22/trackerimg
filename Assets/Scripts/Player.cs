using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public BattleManager bm;
    public GameObject arCam;

    float camOffset;

    public int maxHP;
    public int hp;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHP;

        //Calculamos la distancia vertical entre el jugador y la camara (el pivot esta en el piso)
        camOffset = arCam.transform.position.y - transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (bm.pause) return;

        //Forzamos la posicion del jugador, en vez de hacerlo hijo de la camara
        //Hacemos esto porque no queremos que el collider se incline cuando el jugador mira para abajo o arriba
        //Tomamos la posicion actual de la camara, menos la distancia calculada al principio
        transform.position = arCam.transform.position - Vector3.up * camOffset;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        if(hp <= 0)
        {
            bm.GameOver();
        }
    }
}
