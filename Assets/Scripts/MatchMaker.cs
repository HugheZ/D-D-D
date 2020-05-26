using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchMaker : MonoBehaviour
{
    [SerializeField]
    Canvas matchUI;
    [SerializeField]
    Button join;
    [SerializeField]
    Dropdown matches;
    [SerializeField]
    Button create;

    MatchInfoSnapshot selectedMatch;

    public List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
    NetManScript netMan;

    void Awake()
    {
        netMan = GetComponent<NetManScript>();
        if (netMan.matchMaker == null)
        {
            netMan.StartMatchMaker();
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CreateMatch()
    {
        // First four parameter: room, max size, advertised, password
        netMan.matchMaker.CreateMatch("room " + Random.Range(10000, 0), 4, true, "", "", "", 0, 0, OnMatchCreate);

        DisableMatchMakingUI();
    }

    public void JoinMatch()
    {
        netMan.matchMaker.JoinMatch(selectedMatch.networkId, "", "", "", 0, 0, OnMatchJoined);

        DisableMatchMakingUI();
    }

    void DisableMatchMakingUI()
    {
        Destroy(matchUI.gameObject);
    }

    public void RefreshMatches()
    {
        netMan.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        List<Dropdown.OptionData> elems = new List<Dropdown.OptionData>{ new Dropdown.OptionData("<Pick a Match>") };
        foreach (var match in matchList)
        {
            elems.Add(new Dropdown.OptionData(match.name));
        }
        matches.options = elems;
    }

    public void DropdownValueChange()
    {
        if(matches.value-1 >= 0 && matches.value-1 < matchList.Count)
        {
            selectedMatch = matchList[matches.value-1];
            join.interactable = true;
        }
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            netMan.StartHost(matchInfo);
        }
        else
        {
            Debug.LogError("Create match failed: " + extendedInfo);
        }
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success && matches != null && matches.Count > 0)
        {
            matchList = matches;
        }
        else if (!success)
        {
            Debug.LogError("List match failed: " + extendedInfo);
        }
    }

    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Join match succeeded");
            netMan.StartClient(matchInfo);
            Debug.Log("Started client");
        }
        else
        {
            Debug.LogError("Join match failed " + extendedInfo);
        }
    }

    public void OnConnected(NetworkMessage msg)
    {
        Debug.Log("Connected!");
    }

    public void LocalGameStart()
    {
        //set up match with just self
        netMan.StartHost();
        MultiplayerRunManager.Instance.isLocalMultiplayerGame = true;
        //notify manager
        MultiplayerRunManager.Instance.NotifyLocalGame();

        DisableMatchMakingUI();
    }

}