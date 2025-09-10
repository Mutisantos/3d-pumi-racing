using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** Script para cargar escenas de Unity, ademas de realizar limpiezas iniciales
 * donde sea necesario.
 * Esteban.Hernandez
 */
public class SceneLoader : MonoBehaviour {

	void Start(){

	}
	public void myOwnLoadScene(int index)
	{
		SoundManager.instance.changeMusic(index);
		SceneManager.LoadScene(index);
	}

	public void LoadTrack()
    {
		int trackSelectionIndex = SceneManager.GetActiveScene().buildIndex + CommonDataSingleton.instance.ChosenTrackIndex + 1;
		SoundManager.instance.changeMusic(trackSelectionIndex);
		SceneManager.LoadScene(trackSelectionIndex);
	}

	public void FinishGame(){
		Application.Quit ();
	}

}
