using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    //Necesitamos esto en todas las escenas para que retenga la info de la espada y demas
    public PlayerData data;

    //Si el juego esta en pausa
    public bool pause;

    //Los distintos objetos que forman parte del sable
    public GameObject lightsaber;
    public GameObject beam;
    public GameObject hiltSlot;

    //Opciones de mangos del sable
    public GameObject[] hiltPrefabs;

    //El objeto del jugador, y el lugar hacia donde apuntan los drones (cerca del pecho)
    public GameObject player;
    public Transform targetShoot;

    int score;
    int maxScore;

    [Header("Game UI")]
    public TMP_Text hpText;
    public TMP_Text scoreText;
    public GameObject tapToCounterText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Drone Creation")]
    public int remainingDrones;
    public GameObject dronePrefab;
    public float droneDistance;
    public float droneSpawnTime;
    float droneSpawnTimer;
    public Animator alertAnim;

    [Header("Menu")]
    public GameObject mainCanvas;
    public GameObject menuObj;
    public GameObject menuButton;
    public GameObject resetButton;

    // Start is called before the first frame update
    void Start()
    {
        //Apagamos todos los elementos de UI que no queremos, y prendemos el mensaje tutorial
        mainCanvas.SetActive(true);
        menuButton.SetActive(true);
        resetButton.SetActive(true);
        menuObj.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        tapToCounterText.SetActive(true);

        //Nos aseguramos de que el juego no este pausado
        pause = false;

        //Hacemos que el puntaje para ganar sea el mismo que todos los drones que se van a crear
        //De esta manera cuando mueren todos, el jugador gana
        maxScore = remainingDrones;

        int hiltIndex = 0;

        //Si existe el ScriptableObject, cambiamos los valores por defecto
        if(data)
        {
            hiltIndex = data.hiltIndex;
            beam.GetComponent<MeshRenderer>().material.color = data.colorValue;
        }

        //Destruimos cualquier hijo que haya en el slot, para limpiarlo
        foreach (Transform child in hiltSlot.transform)
        {
            Destroy(child.gameObject);
        }

        //Instanciamos el mango de sable correcto, segun la informacion guardada, o el que esta por defecto
        Instantiate(hiltPrefabs[hiltIndex], hiltSlot.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (pause) return;

        //Actualizamos el puntaje y vida del jugador
        hpText.text = "HP: " + player.GetComponent<Player>().hp;
        scoreText.text = "Score: " + score;

        //Contamos el tiempo que va pasando, y si se cumple el tiempo de crear drones, crea uno
        //Ademas chequea que se puedan seguir creando, para que no siga infinitamente
        droneSpawnTimer += Time.deltaTime;
        if(droneSpawnTimer >= droneSpawnTime && remainingDrones > 0)
        {
            droneSpawnTimer = 0;
            remainingDrones--;
            SpawnDrone();
        }
    }

    void SpawnDrone()
    {
        //Instanciamos una copia del prefab de drone, y lo ubicamos en el lugar hacia donde queremos que dispare
        GameObject drone = Instantiate(dronePrefab);
        drone.transform.position = targetShoot.position;

        //Alinea el drone con el player para que miren en la misma direccion
        drone.transform.forward = player.transform.forward;

        //A partir de ahi, giramos al drone en Y en un angulo random de +-135°
        //Ademas, lo inclinamos un poco hacia arriba / abajo usando su forward como referencia en vez de un eje fijo
        drone.transform.Rotate(Vector3.up * Random.Range(-135f, 135f) + drone.transform.forward * Random.Range(-15f, 30f));
        
        //Luego, lo movemos hacia "su" adelante. Esto hace que le de la espalda al jugador, pero el mismo drone esta programado para mirarlo
        drone.transform.position += drone.transform.forward * droneDistance;

        //Obtenemos las posiciones del jugador y el drone sin contar la Y, para el calculo que vamos a hacer ahora
        Vector3 droneNoY = new Vector3(drone.transform.position.x, 0, drone.transform.position.z);
        Vector3 playerNoY = new Vector3(player.transform.position.x, 0, player.transform.position.z);

        //Calculamos el angulo entre la direccion hacia donde mira el player, y el vector entre las posiciones de arriba
        Vector3 from = player.transform.forward;
        Vector3 to = droneNoY - playerNoY;
        float angle = Vector3.SignedAngle(from, to, Vector3.up);

        //En base al angulo que calculamos, le avisamos al jugador si el drone esta a su izquierda o derecha, para que reaccione
        //Si esta dentro de un cono de +-25°, esta en frente del jugador y puede verlo por si mismo, no necesita feedback
        if (angle < -25)
        {
            alertAnim.Play("Alert Left");
        }
        else if (angle > 25)
        {
            alertAnim.Play("Alert Right");
        }

        GetComponent<AudioSource>().Play();
    }

    //Sumamos el puntaje, y si mato a todos (ver Start) se muestra la pantalla de victoria. Los drones llaman esto al morir
    public void ScorePoints()
    {
        score++;
        if (score >= maxScore) Victory();
    }

    //Pausamos el juego y mostramos solo el menu de derrota
    public void GameOver()
    {
        pause = true;
        gameOverPanel.SetActive(true);
        menuObj.SetActive(false);
        mainCanvas.SetActive(false);
        menuButton.SetActive(false);
        resetButton.SetActive(false);
    }

    //Idem pero con el de victoria
    void Victory()
    {
        pause = true;
        victoryPanel.SetActive(true);
        menuObj.SetActive(false);
        mainCanvas.SetActive(false);
        menuButton.SetActive(false);
        resetButton.SetActive(false);
    }

    //Completar: animacion de ataque
    public void Attack()
    {
    }

    public void ToggleMenu()
    {
        menuObj.SetActive(!menuObj.activeSelf);
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        pause = !pause;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
