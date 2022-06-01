using System;

[Serializable]
public class CoreConfigLinkList
{
    public string Title;
    public string Address;
}

[Serializable]
public class CoreConfig
{
    // Command & Control

    public string UpdateAddress = "";
    public bool Updated;

    // Autoupdate system

    public string VersionLatest;
    public int VersionLatestEpoch;
    public string VersionLatestNotes;
    public string InstallerAuto;
    public string InstallerMac;
    public string InstallerWin;
    public string InstallerLinux;

    // Multiplayer networking

    public string AuthServerAddress;
    public string MasterServerAddress;
    public int MaxPlayersInServer = 16;

    // UI Message customization

    public CoreConfigLinkList[] LobbyLinks;
    public string LobbyMessageLatest;
    public string LobbyMessageOutdated;
    public string LobbyButtonUpdate;
    public string AlertMessageLatest;
    public string AlertMessageOutdated;
    public String[] LoadingMessages;
    public String[] TeaserMessages;

    // Live-updated news links
    public CoreConfigLinkList[] NewsLinks;
    
    // Geo Engine
    public string ApiKeyGoogleMaps = "AIzaSyDTNGZ5mA_UzRCthcLJN1KacMUqnq6jfys";
    public string ApiKeyBingMaps = "AuEVw_nAgx0OQ1FhY35dvJGu2jvgFgzUhRlW9TJDhNr7mGi_lAxbnZGRm5lRwjwF";
    public string ApiKeyWRLD = "54cbc90b28b0ffe741aa29818bf0f20b";
}