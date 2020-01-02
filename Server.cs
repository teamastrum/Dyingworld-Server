using Godot;
using System;

public class Server : Node
{
    public int Port = 7777;
    public int MaxPlayers = 20;

    public Godot.Collections.Array Players;
    public NetworkedMultiplayerENet server;
    public override void _Ready()
    {
        Players = new Godot.Collections.Array();
        // Init server
        server = new NetworkedMultiplayerENet();
        server.CreateServer(Port, MaxPlayers);
        GetTree().NetworkPeer = server;

        // Events
        GetTree().Connect("network_peer_connected", this, "OnPeerConnected"); // On Peer connected
        GetTree().Connect("network_peer_disconnected", this, "OnPeerDisconnected"); // On Peer disconnected
    }

    public void OnPeerConnected(int id)
    {
        GD.Print("[INFO] Peer with ID " + id + " has been connected.");


        var newPeerScene = (PackedScene)ResourceLoader.Load("res://Peer.tscn");
        var newPeer = newPeerScene.Instance();

        newPeer.Name = Convert.ToString(id);
        newPeer.SetNetworkMaster(id);
        GetTree().Root.AddChild(newPeer);

        Players.Add(id);
    }

    public void OnPeerDisconnected(int id)
    {
        GD.Print("[INFO] Peer with ID " + id + " has left.");
        Players.Remove(id);
        GetTree().Root.GetNode<Spatial>(""+id);
    }
    [Remote]
    public void RegisterPlayer(string info)
    {
        GD.Print(info);
    }

    // [Puppet]
    // public void SendPlayerList()
    // {
    //     GD.Print("Hello!");
    // }
}
