using System;

public class GameError : Exception {
    public GameError() : base() { }

    public GameError(string message) : base(message) { }

    public GameError(string message, Exception innerException) : base(message, innerException) { }
}