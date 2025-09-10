using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** Script para seleccionar personaje basado en el indice de personajes disponibles
 * Esteban.Hernandez
 */
public class PlayerSelector : MonoBehaviour {

	public Button NextButtonSelector;

    private void Start()
    {
        //Solo se habilita el boton para pasar a la siguiente escena si se ha seleccionado algo antes.
        NextButtonSelector.enabled = false;
    }

    public void SelectCharacter(int index)
	{
		CommonDataSingleton.instance.ChooseRacer(index);
        NextButtonSelector.enabled = true;
    }
}
