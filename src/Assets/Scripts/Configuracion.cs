using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Configuracion : MonoBehaviour {

    public GameManager gameManager;

    public InputField nombre1;
    public InputField ip1;
    public InputField puerto1;
    public InputField nombre2;
    public InputField ip2;
    public InputField puerto2;
    public InputField partidas;

    public void empezar() {
        if (_configuracionesValidas()) {
            gameManager.empezar(nombre1.text, ip1.text, puerto1.text, nombre2.text, ip2.text, puerto2.text, int.Parse(partidas.text));
            transform.Find("Fondo").gameObject.SetActive(false);
        }
    }

    private bool _configuracionesValidas() {
        // Validación media trucha
        return (
            nombre1.text.Length > 0 &&
            ip1.text.Length > 0 &&
            puerto1.text.Length > 0 &&
            nombre2.text.Length > 0 &&
            ip2.text.Length > 0 &&
            puerto2.text.Length > 0 &&
            partidas.text.Length > 0
        );
    }
	
}
