using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Network {
    public class Client : MonoBehaviour {
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];

        void Start() {
            client = new TcpClient("127.0.0.1", 12345);
            stream = client.GetStream();

            // Send initial data
            SendData(new JsonData { message = "Hello from client" });

            // Begin reading
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), null);
        }

        private void ReadCallback(IAsyncResult ar) {
            var bytesRead = stream.EndRead(ar);
            if (bytesRead > 0) {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + receivedData);

                // Deserialize JSON
                var receivedObject = JsonConvert.DeserializeObject<JsonData>(receivedData);
            } else {
                stream.Close();
            }
        }

        public void SendData(JsonData data) {
            var jsonData = JsonConvert.SerializeObject(data);
            var dataBytes = Encoding.UTF8.GetBytes(jsonData);
            stream.BeginWrite(dataBytes, 0, dataBytes.Length, new AsyncCallback(WriteCallback), null);
        }

        private void WriteCallback(IAsyncResult ar) {
            stream.EndWrite(ar);
        }
    }
}