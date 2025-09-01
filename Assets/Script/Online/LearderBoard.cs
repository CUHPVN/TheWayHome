using System;
using LootLocker;
using LootLocker.Requests;
using Unity.VisualScripting;
using UnityEngine;

public class LearderBoard : MonoBehaviour
{
    public static LearderBoard Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Send(int score)
    {
        string leaderboardKey = "your_leaderboard_key";

        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, (response) =>
        {
            if (response.success)
            {
                //Debug.Log("Gửi điểm số thành công!");
            }
            else
            {
                //Debug.LogError("Gửi điểm số thất bại, hãy bật wifi và chơi ở loading scene!");
            }
        });
    }
    public void SetName(string name)
    {
        LootLockerSDKManager.SetPlayerName(name, (response) =>
        {
            if (response.success)
            {
                //Debug.Log("Tên người chơi đã được gán!");
                LoginSystem.Instance.Login();
            }
            
                //Debug.LogError("Không thể gán tên.");
        });

    }
    public void SetAvar(int index)
    {
        LootLockerSDKManager.UpdateOrCreateKeyValue("avar", index.ToString(), true, (response) =>
        {
        if (response.success)
        {
            //Debug.Log("Avatar người chơi đã được gán!");
            LoginSystem.Instance.Login();
        }
            else
        {
            //Debug.LogError("Không thể gán avatar.");
        }
        }
        );
    }
    public int GetHightScore(string playerID)
    {
        string leaderboardKey = "your_leaderboard_key";
        int score=0;
        LootLockerSDKManager.GetMemberRank(leaderboardKey, playerID, (response) =>
        {
            if (!response.success)
            {
                //Debug.Log("Could not get the entry!");
                //Debug.Log(response.errorData.ToString());
                score = 0;
                return;
            }
            //Debug.Log("Successfully got entry!");
            score = response.score;
        });
        return score;
    }
    public void LoadLeaderBoard()
    {
        Get();
    }
    public void Get()
    {
        int count = 10;
        int after = 0;

        LootLockerSDKManager.GetScoreList("your_leaderboard_key", count, after, (response) =>
        {
            if (response.success)
            {
                if (LeaderBoardUIManager.Instance != null)
                {
                    LeaderBoardUIManager.Instance.Clear();
                    foreach (var entry in response.items)
                    {
                        int playerId = entry.player.id;
                        string playerName = entry.player.name;
                        int rank = entry.rank;
                        int score = entry.score;
                        //Debug.Log(rank+ playerName+ score+playerId);
                        if (playerId == LoginSystem.Instance.GetPlayerID())
                            LeaderBoardUIManager.Instance.AddPlayer(rank, playerName + " (YOU)", score, playerId);
                        else
                            LeaderBoardUIManager.Instance.AddPlayer(rank, playerName, score, playerId);
                    }
                }
            }
            else
            {
                //Debug.LogError("Lấy danh sách bảng xếp hạng thất bại.");
            }
        });
    }
    public void GetAvarOfPlayer(int playerId, Action<string> onAvarReceived)
    {
        LootLockerSDKManager.GetOtherPlayersPublicKeyValuePairs(playerId.ToString(), (response) =>
        {
            if (response.success && response.payload != null)
            {
                string avar = "0";
                foreach (var item in response.payload)
                {
                    if (item.key == "avar")
                    {
                        avar = item.value;
                        break;
                    }
                }
                onAvarReceived?.Invoke(avar);
            }
            else
            {
                //Debug.LogWarning($"Không thể lấy avar cho playerId: {playerId}");
                onAvarReceived?.Invoke(null);
            }
            
        });
       
    }
}
