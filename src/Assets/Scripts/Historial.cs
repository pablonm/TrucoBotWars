using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Historial : MonoBehaviour {

    private string[] instrucciones;
    private int dimLogic;
    private Text t;

    private void Start()
    {
        instrucciones = new string[14];
        dimLogic = -1;
        t = this.gameObject.GetComponent<Text>();
    }

    public void agregarAlHistorial(string mensaje, string nombre)
    {
        if (dimLogic < 13)
        {
            dimLogic++;
        }
        else
        {
            for (int i = 0; i < dimLogic; i++)
            {
                instrucciones[i] = instrucciones[i + 1];
            }
        }
        instrucciones[dimLogic] = string.Format("> {0}: {1}", nombre, mensaje);
        t.text = String.Join("\n", instrucciones);
    }
}
