using UnityEngine;

namespace UI.Button {
    public class Client : MonoBehaviour {
        public void ClientOnClick() {
            // TODO Client 객체 생성
            
            var network = GameObject.FindWithTag("Network");
            network.AddComponent<Network.Client>();
        }
    }
}