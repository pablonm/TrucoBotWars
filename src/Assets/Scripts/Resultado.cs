using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Resultado : MonoBehaviour {

	public Text nombreGanador;
    public Text nombrePerdedor;


    public void setGanador(string nombre, int puntos) {
        nombreGanador.text = nombre + " - " + puntos;
    }

    public void setPerdedor(string nombre, int puntos) {
        nombrePerdedor.text = nombre + " - " + puntos;
    }

    public void mostrar() {
        transform.Find("Fondo").gameObject.SetActive(true);
    }

    public void salir() {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        //Application.Quit();
    }

}
