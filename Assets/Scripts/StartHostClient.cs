using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour
{
    public void StartHost()
    {
        StartCoroutine(I_StartHost());
    }

    public void StartClient()
    {
        StartCoroutine(I_StartClient());
    }

    private IEnumerator I_StartHost()
    {
        NetworkManager.Singleton.StartHost();
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.SceneManager.LoadScene("level1", LoadSceneMode.Single);
    }
    
    private IEnumerator I_StartClient()
    {
        NetworkManager.Singleton.StartClient();
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.SceneManager.LoadScene("level1", LoadSceneMode.Single);
        
    }
}