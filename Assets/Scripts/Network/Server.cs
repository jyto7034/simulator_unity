using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;


// Front, Back 에서 공통적으로 사용할 Server 을 구현하는 Class
// Json 의 Header 에 Front, Back 의 정보를 담아서 처리함.
namespace Network {
    public class Server : MonoBehaviour {
        private TcpListener listener;
        private bool is_initialized;

        [HideInInspector] public int port = -1;

        [HideInInspector] public string json;

        void initialize() {
            // Do something . .
            // port = Utility.get_port();
            is_initialized = true;
        }

        void Start() {
            if (!is_initialized) {
                // Debug.LogError(GameError.ServerInitializeError.ToString());
                // TODO 예외처리
            }
            
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Debug.Log("Front Server started...");
            listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);
            
            var network = GameObject.FindWithTag("Network");            
            network.AddComponent<Network.Client>();
        }

        private void AcceptClient(IAsyncResult ar) {
            // TODO: 지정된 클라이언트인지 확인하는 보안 코드 작성해야함. 인원수는 2명으로 제한.
            var client = listener.EndAcceptTcpClient(ar);
            Debug.Log("Client connected...");
            var stream = client.GetStream();

            // Json 크기에 따라.
            var buffer = new byte[1024];
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback),
                new StateObject { Stream = stream, Buffer = buffer });
        }

        private void ReadCallback(IAsyncResult ar) {
            var state = (StateObject)ar.AsyncState;
            var stream = state.Stream;
            var buffer = state.Buffer;

            var bytesRead = stream.EndRead(ar);
            if (bytesRead > 0) {
                // 소켓 버퍼로부터 데이터를 저장함.
                var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + receivedData);

                // Json 을 string 타입으로 변환
                var receivedObject = JsonConvert.DeserializeObject<JsonData>(receivedData);

                // TODO: receivedObject 분석 후 responseObject 객체 생성
                // var responseObject = new JsonData { message = "Hello from server" };
                
                // 위 TODO 구현되면 삭제되야함.
                var responseObject = new JsonData { message = "Hello from server" };
                
                var responseData = JsonConvert.SerializeObject(responseObject);
                var responseBytes = Encoding.UTF8.GetBytes(responseData);

                // Send response
                stream.BeginWrite(responseBytes, 0, responseBytes.Length, new AsyncCallback(WriteCallback), stream);

                // Continue reading
                // GamState
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), state);
            }
            else {
                stream.Close();
                // TODO: 에러 전파
            }
        }

        private void WriteCallback(IAsyncResult ar) {
            var stream = (NetworkStream)ar.AsyncState;
            stream.EndWrite(ar);
            Debug.Log("Response sent to client.");
        }
    }
}