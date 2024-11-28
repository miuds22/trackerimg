using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System;

public class ScanManager : MonoBehaviour
{
    [Header("Player Data")]
    public PlayerData data;

    [Header("Menu")]
    public GameObject mainCanvas;
    public GameObject menuObj;
    public GameObject animateButton;
    public string animationName;

    [Header("modelProyected")]

    public GameObject[] models;
    public int activeModel;

    [Header("buttons")]
    public GameObject lBtn;
    public GameObject rBtn;

    [Header("Tracking")]
    public ARTrackedImageManager trackedImageMgr;

    //Ahora vamos a necesitar un array de nombres, porque tenemos varias imagenes
    public string[] targetNames;

    //Y aca se van a guardar una vez instanciados
    List<GameObject> objects;

    [Header("Testing")]
    public TMP_Text console; //Consola "casera" hecha con un Text
    public bool showConsole; //Podemos elegir activarla para testear o desactivarla para la build

    void Awake()
    {
        //Si se olvidaron de asignar el manager manualmente, lo busca
        if (!trackedImageMgr) trackedImageMgr = FindObjectOfType<ARTrackedImageManager>();

        //Prende el Canvas con el boton de menu y apaga el menu en si
        mainCanvas.SetActive(true);
        menuObj.SetActive(false);

        lBtn.SetActive(false);
        rBtn.SetActive(false);
        //Le damos un valor inicial al texto para saber si todavia no esta trackeando nada
        console.text = "Nothing";

        //Si no lo queremos ver, cambiamos el valor de showConsole y esto se lo aplica
        console.gameObject.SetActive(showConsole);

            //Inicializamos la lista de objetos
        objects = new List<GameObject>();

        //Instanciamos una copia de cada prefab, la guardamos en la lista y la apagamos
        for (int i = 0; i < models.Length; i++)
        {

            objects.Add(Instantiate(models[i]));
            objects[i].SetActive(false);
            console.text += "insrtanciado: " + targetNames[i] + "\n";
        }
    }

    void Update()
    {
        //Si no hay objetos creados, desactivamos el boton de animacion y cortamos la ejecucion
        if (objects.Count == 0)
        {
            animateButton.SetActive(false);
            return;
        }

        //De lo contrario, primero creamos un booleano en false que diga si al menos algun objeto esta activo para animarlo
        bool anyActive = false;

        //Luego recorremos toda la lista de objetos y, si alguno esta activo, lo pasamos a true
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].activeSelf)
            {
                anyActive = true;
            }
        }

        //Finalmente seteamos el estado del boton a lo que sea que diga el booleano de recien
        animateButton.SetActive(anyActive);
    }

    //Esta es una "magia de programacion" que pasa funciones como variables
    //En este caso la funcion grande de abajo se pone y se saca de una "fila"
    //Cuando pasa el "evento" trackedImagesChanged llama a las funciones en esa fila
    void OnEnable()
    {
        trackedImageMgr.trackedImagesChanged += OnTrackedImageChanged;
    }

    void OnDisable()
    {
        trackedImageMgr.trackedImagesChanged -= OnTrackedImageChanged;
    }

    //Esta funcion toma la informacion del tracker, que incluye todas las imagenes que estan siendo trackeadas
    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        #region Disclaimer - Bug de Unity
        //Este script asume que el sistema de AR de Android/ Unity tiene el bug que impide que las imagenes se remuevan
        //Normalmente, cuando se reconoce una imagen va a eventArgs.added, luego a eventArgs.updated
        //Y si no se ve, se pasa a eventArgs.removed
        //Lamentablemente esa ultima parte no funciona, y nuestro workaround es usar el Tracking State
        //Tracking es cuando se ve, y Limited es cuando no se ve
        //Tratamos un TrackingState.Limited como si estuviera removida
        //Todo esto entonces siempre va a pasar en updated
        #endregion

        //Creamos un string vacio que va a ir a la consola casera
        //Descomentar todas las lineas asociadas a "consoleMsg" que sean necesarias para testear algo
        string consoleMsg = "";

        //Mostramos cuantas imagenes estan siendo trackeadas

        lBtn.SetActive(true);
        rBtn.SetActive(true);


        //Primero recorremos todos los nombres de imagenes   
        for (int i = 0; i < models.Length; i++)
        {
            //Partimos de la base de que no se tiene que mostrar el objeto correspondiente a la imagen que estamos evaluando
            bool match = false; 


            //Ahora recorremos todas las imagenes que el sistema de Image Tracking esta reconociendo
            foreach (ARTrackedImage updatedImage in eventArgs.updated)
            {
               
                match = false;
                if ( updatedImage.trackingState == TrackingState.Tracking && i == activeModel)
                {
                    objects[i].transform.parent = updatedImage.transform;
                    objects[i].transform.position = updatedImage.transform.position;
                    objects[i].transform.rotation = updatedImage.transform.rotation;
                    match = true;
                }
            }
            //Finalmente aplicamos el resultado de match al GameObject correspondiente al nombre que estamos evaluando
            objects[i].SetActive(match);
        }

        //Pasamos todos los mensajes que fuimos acumulando a la consola casera
        console.text = consoleMsg;
    }



    public void NextObject()
    {
        activeModel++;
        if (activeModel >= models.Length) activeModel = 0;
        console.text += "contador:" + activeModel;
    }

    public void PreviousObject()
    {
        activeModel--;
        if (activeModel <= -1) activeModel = models.Length - 1;
        console.text += "contador:" + activeModel;
    }

    //Esta funcion la llama el boton de Animate y afecta a todos los objetos que van en las imagenes
    public void AnimateObject()
    {
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<Animator>().Play(animationName);
        }
    }

    public void ToggleMenu()
    {
        menuObj.SetActive(!menuObj.activeSelf);
        mainCanvas.SetActive(!mainCanvas.activeSelf);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}