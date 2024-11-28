using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color[] colors;

    public GameObject beam;
    public GameObject hiltSlot;

    public GameObject[] hiltPrefabs;

    public GameObject lightsaber;
    public float rotSpeed;

    public PlayerData data;

    public GameObject mainCanvas;
    public GameObject menuObj;

    // Start is called before the first frame update
    void Start()
    {
        menuObj.SetActive(false);
        mainCanvas.SetActive(true);

        Instantiate(hiltPrefabs[0], hiltSlot.transform);

        //Preguntamos si esta asignado el ScriptableObject para que no explote en caso contrario
        if (data)
        {
            ChangeHilt(data.hiltIndex);
            ChangeBeamColor(data.colorIndex);
        }

        //Completar: forzar animacion
    }

    // Update is called once per frame
    void Update()
    {
        //Hacemos que vaya girando el sable (decorativo)
        lightsaber.transform.Rotate(transform.up * 360 * rotSpeed * Time.deltaTime);
    }

    public void ChangeBeamColor(int index)
    {
        //Accedemos al componenteMeshRenderer de la luz, despues a su material, y despues a su color
        beam.GetComponent<MeshRenderer>().material.color = colors[index];

        //Si esta el ScriptableObject, guardamos la configuracion
        if(data)
        {
            data.colorIndex = index;
            data.colorValue = colors[index];
        }
    }

    public void ChangeHilt(int index)
    {
        //Destruimos cualquier hijo que haya en el slot, para limpiarlo
        foreach (Transform child in hiltSlot.transform)
        {
            Destroy(child.gameObject);
        }

        //Una vez que esta limpio, lo reemplazamos por el nuevo mango
        Instantiate(hiltPrefabs[index], hiltSlot.transform);

        //Si esta el ScriptableObject, guardamos la configuracion
        if (data)
        {
            data.hiltIndex = index;
        }
    }

    public void ToggleMenu()
    {
        menuObj.SetActive(!menuObj.activeSelf);
        mainCanvas.SetActive(!mainCanvas.activeSelf);
    }

    public void LoadScene(string sceneName)
    {
        //Completar: cambio de escena con string
    }
}
