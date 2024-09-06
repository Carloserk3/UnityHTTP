using System;
using System.IO;
using System.Net.Http;
using UnityEngine;

public static class SSEClient {

    public static event Action<string> OnReceive;
    public static event Action OnClose;

    static HttpClient client = new HttpClient();
    static Stream stream;
    static StreamReader reader;

    public static bool Connected;

    public static async void Connect(string address) {

        if (Connected) { return; }

        client.Timeout = TimeSpan.FromHours(3);

        try {

            Application.quitting += Dispose;          
            HttpResponseMessage response = await client.GetAsync(address, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            Connected = response.IsSuccessStatusCode;       

            using (stream = await response.Content.ReadAsStreamAsync())
            using (reader = new StreamReader(stream)) {
                stream.ReadTimeout = int.MaxValue;
                string line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    OnReceive?.Invoke(line);
                    stream.Flush();
                }
            }
        }

        catch {
            Connected = false;            
            OnClose.Invoke();
            Dispose();
        }

    }

    public static void Dispose() {
        client?.Dispose();
        stream?.Dispose();
        reader?.Dispose();
    }

}
