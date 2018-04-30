using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Configuracion : MonoBehaviour {

    public GameManager gameManager;

    public InputField nombre1;
    public InputField ip1;
    public InputField puerto1;
    public InputField nombre2;
    public InputField ip2;
    public InputField puerto2;
    public InputField partidas;
    public Text errores;
    public string path = "./config.sav";

    private void Start()
    {
        try
        {
            //intenta abrir el archivo, si no lo encuentra va al catch
            StreamReader reader = new StreamReader(path);
            string[] data = reader.ReadLine().Split(';');
            reader.Close();
            //comprobacion trucha de que los datos estan bien
            if (data.Length == 7)
            {
                nombre1.text = data[0];
                ip1.text = data[1];
                puerto1.text = data[2];
                nombre2.text = data[3];
                ip2.text = data[4];
                puerto2.text = data[5];
                partidas.text = data[6];
            }
        }
        catch
        {
            Debug.Log("no se encontro el archivo");
        }
    }

    public void empezar() {
        string err = _configuracionesValidas();
        if (err == "") {
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine("{0};{1};{2};{3};{4};{5};{6}", nombre1.text, ip1.text, puerto1.text, nombre2.text, ip2.text, puerto2.text, partidas.text);
            writer.Close();
            gameManager.empezar(nombre1.text, ip1.text, puerto1.text, nombre2.text, ip2.text, puerto2.text, int.Parse(partidas.text));
            transform.Find("Fondo").gameObject.SetActive(false);
        }
        else
        {
            errores.text = err;
        }
    }

    private string _configuracionesValidas() {
        // Validación medio trucha
        // Pero ya no tanto
        string error = "";
        int intparsed;
        if (nombre1.text.Length < 1) error += "Ponele nombre a tu bot(1), insensible!\n";
        if (ip1.text != "localhost")
            if (ip1.text.Length > 0 && ip1.text.Split('.').Length == 4)
            {
                foreach (string s in ip1.text.Split('.'))
                {
                    if (!int.TryParse(s, out intparsed) || intparsed>255 || intparsed < 0)
                    {
                        error += "Che, tu ip(1) no tiene pinta de ip\n";
                    }
                }
            }
            else
            {
                error += "Che, tu ip(1) no tiene pinta de ip\n";
            }
        if (!int.TryParse(puerto1.text, out intparsed) || intparsed < 1 || intparsed > 9999)
        {
            error += "Pone bien bien el puerto(1)\n";
        }


        if (nombre2.text.Length < 1) error += "Ponele nombre a tu bot(2), insensible!\n";
        if (ip1.text != "localhost")
            if (ip2.text.Length > 0 && ip2.text.Split('.').Length == 4)
            {
                foreach (string s in ip2.text.Split('.'))
                {
                    if (!int.TryParse(s, out intparsed) || intparsed > 255 || intparsed < 0)
                    {
                        error += "Che, tu ip(2) no tiene pinta de ip\n";
                    }
                }
            }
            else
            {
                error += "Che, tu ip(2) no tiene pinta de ip\n";
            }
        if (!int.TryParse(puerto2.text, out intparsed) || intparsed < 1 || intparsed > 9999)
        {
            error += "Pone bien bien el puerto(2)\n";
        }

        if(partidas.text.Length < 1 || !int.TryParse(partidas.text, out intparsed)){
            error += "Partidas = {x/ x є N, x>0}\n";
        }
        return error;
    }
	
}
