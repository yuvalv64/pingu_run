using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;
using Prototype.NetworkLobby;

public class NetworkPlayer : NetworkMessageHandler
{

    [Header("Player Properties")]
    public string playerID;

    [Header("Ship Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    private float networkSendRate = 8;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    //[Header("Camera Movement Properties")]
    //public float distance = 15.0f;
    //public float xSpeed = 60.0f;
    //public float ySpeed = 120.0f;
    //private float cameraX = 0;
    //private float cameraY = 0;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public Vector3 realPosition;
    public Vector3 lastRealPosition;
    public float timeStartedLerping;
    public float timeToLerp;

    private void Start()
    {
        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);

        if (isLocalPlayer)
        {
            //GameObject.FindGameObjectWithTag("GameManager").GetComponent<NetGameManager>().setPlayerName(playerID);
            Manager.Instance.SetLocalPlayerID(playerID);

            canSendNetworkMovement = false;
            RegisterNetworkMessages();
        }
        else
        {
            isLerpingPosition = false;

            realPosition = transform.position;
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
    }

    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        if (_msg.objectTransformName != transform.name)
        {
            Manager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<NetworkPlayer>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        lastRealPosition = realPosition;
        realPosition = _position;
        timeToLerp = _timeToLerp;

        if(realPosition != transform.position)
            isLerpingPosition = true;
        
        timeStartedLerping = Time.time;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        
        UpdatePlayerMovement(); 
        
    }

    private void UpdatePlayerMovement()
    {

        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        PlayerMovementMessage _msg = new PlayerMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer)
            NetworkLerp();
    }

    private void NetworkLerp()
    {
        if(isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }
    }
}