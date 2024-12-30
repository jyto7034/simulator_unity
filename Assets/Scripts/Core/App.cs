using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.ComponentModel;
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit{}
}
namespace Core {
    public class App : MonoBehaviour {

        void Start() {
            // Client 가 만든 GameConfig 를 읽어들임.
            // var config = Utility.get_game_config();

            // var game = new Game();

            // Host 로 실행되는 경우.
            // switch (config.header) {
            //     case ClientType.Host:
            //         // Host 으로 실행.
            //         // TODO: GameObject 을 생성하여 Host 스크립트 attatch 하기
            //         break;
            //     case ClientType.Client:
            //         // Client 로 실행.
            //         // TODO: GameObject 을 생성하여 Client 스크립트 attatch 하기
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }
    }
}