// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class toMainMenu : MonoBehaviour {

	// Es redundante ahora. Eliminen esto.
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if  (Input.GetKeyDown(KeyCode.Escape)){
			SceneManager.LoadScene("BPK_MainMenu");
			
		}
	}
}
