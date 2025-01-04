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

public enum CardStatus {
    Open,
    Close,
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

public enum ZoneType {
    Hand,
    Deck,
    Graveyard,
    Field,
    None,
}

public enum PlayerType {
    Player1,
    Player2
}

public class AddCardOptions {
    public int? SlotId { get; set; }
}