using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class LeaderboardLoader : NetworkBehaviour
{
    [SerializeField] private float waitTime = 2f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(waitTime);

        string lastLevel = PlayerPrefs.GetString("LastLevel", "level1");

        int levelNumber = int.Parse(lastLevel.Replace("level", ""));
        int nextLevel = levelNumber + 1;

        if (nextLevel > 8)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("selectlevel", LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.SceneManager.LoadScene("level" + nextLevel, LoadSceneMode.Single);
        }
    }
}