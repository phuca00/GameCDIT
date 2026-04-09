using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    // Hàm này ghi đè logic mặc định, trả quyền quyết định vị trí về cho Owner (Client)
    protected override bool OnIsServerAuthoritative()
    {
        return false; 
    }
}