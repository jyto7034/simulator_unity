using UnityEngine;

namespace UI.Button {
    public class Server : MonoBehaviour{
        public void ServerOnClick() {
            // TODO Server 객체 생성 후, Client 객체 생성
            var network = GameObject.FindWithTag("Network");
            network.AddComponent<Network.Server>();
        }
    }
}