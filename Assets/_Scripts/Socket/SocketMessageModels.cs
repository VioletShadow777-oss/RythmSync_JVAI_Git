using System;

[Serializable]
public class SocketEnvelope
{
    public string command;
}

[Serializable]
public class StartCommandMessage
{
    public string command;
    public int song;
}

[Serializable]
public class ScoreMessage
{
    public string command;
    public int song;
    public string rating;
    public int score;
    public int combo;
}

[Serializable]
public class ResultMessage
{
    public string command;
    public int song;
    public int finalScore;
    public int perfect;
    public int great;
    public int good;
    public int miss;
    public int maxCombo;
    public string grade;
}

[Serializable]
public class StateMessage
{
    public string command;
    public string state;
    public int song;
}