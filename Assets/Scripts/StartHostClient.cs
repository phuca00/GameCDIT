using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour
{
    public void StartHost() 
    { 
        // CHỐT AN TOÀN: Nếu mạng đang chạy rồi thì KHÔNG làm gì cả (Chống sập game)
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) 
        {
            Debug.LogWarning("Mạng đã chạy, không thể Start Host lần nữa!");
            return;
        }
        StartCoroutine(I_StartHost()); 
    }

    public void StartClient() 
    { 
        // CHỐT AN TOÀN
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient) 
        {
            Debug.LogWarning("Mạng đã chạy, không thể Start Client lần nữa!");
            return;
        }
        NetworkManager.Singleton.StartClient(); 
    }

    private IEnumerator I_StartHost()
    {
        NetworkManager.Singleton.StartHost();
        
        // Đợi đến khi Server mở cửa đón khách thành công
        while (!NetworkManager.Singleton.IsListening) yield return null;
        
        // Host kéo toàn bộ phòng sang level1
        NetworkManager.Singleton.SceneManager.LoadScene("level1", LoadSceneMode.Single);
    }

    private void OnApplicationQuit() 
    {
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
    }
}