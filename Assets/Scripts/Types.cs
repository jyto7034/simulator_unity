using System;
using System.Net.Sockets;
using Transform;

public class JsonData {
    public string message;
}

public enum ClientType {
    Host,
    Client,
}

public enum ServerType {
    BackEnd,
    FrontEnd,
}

public enum ServerState {
    // 상대 행동을 기다리는 중
    Pending,
    
    // 상대 행동 정보를 받음
    Received,
    
    // 자신의 행동 정보를 보냄.
    Sending,    
}


public class StateObject {
    public NetworkStream Stream { get; set; }
    public byte[] Buffer { get; set; }
}

public class GameConfig {
    public ClientType header;
    public string p1_deckcode;
    public string p2_deckcode;
        
    public static GameConfig New() {
        GameConfig config = new GameConfig();
        return config;
    }
}

public struct FromTo {
    public TransformData from;
    public TransformData to;
}

public enum ZoneType {
    Hand,
    Deck,
    Graveyard,
    Field,
    None,
}