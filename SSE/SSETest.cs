using UnityEngine;
using System;
using System.Text;

public class SSETest : MonoBehaviour {

    [SerializeField] string address;

    void Start() {
        SSEClient.Connect(address);
        SSEClient.OnReceive += (data) => { 
            byte[] buffer = Convert.FromBase64String(data);
            string decodedString = Encoding.UTF8.GetString(buffer);
            Debug.Log(decodedString);
        };
        SSEClient.OnClose += () => { Debug.LogWarning("SSE Closed"); };
    }

}
