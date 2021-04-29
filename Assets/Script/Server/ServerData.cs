using System;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;

/// <summary>
/// Firebase에서 유저 정보의 데이터를 처리해준다.
/// </summary>
public class ServerData : SceneSingleTon<ServerData>
{
    private DatabaseReference reference;
    
    private Dictionary<string, object> items = new Dictionary<string, object>();
    private string userKey;

    public void Init(string userKey)
	{
		AppOptions options = new AppOptions { DatabaseUrl = new Uri("https://slader-21906011-default-rtdb.firebaseio.com/") };
        FirebaseApp app = FirebaseApp.Create(options);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        this.userKey = userKey;
	}
	public void SaveData()
	{
        if(!AuthManager.Instance.isFirebaseReady)
        {
            return;
        }

        string json = JsonUtility.ToJson(MyData.Instance.charData);
        reference.Child("Users").Child(userKey).SetRawJsonValueAsync(json);
	}
	public void LoadData()
	{
        if(!AuthManager.Instance.isFirebaseReady)
        {
		    DebugOptimum.Log("파이어 베이스 실패");
            return;
        }

		DebugOptimum.Log("구글 토큰 유저 키 : " + userKey);

        reference.Child("Users").Child(userKey).GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
		    { 
                DataSnapshot snapshot = task.Result;

                items.Clear();
                foreach(var data in snapshot.Children)
                {
                    items.Add(data.Key, data.Value);
                }

                //String으로 변환하고 형변환을 진행하기 때문에 오버헤드가 무척큼 다른걸로 대안 찾는 중
                var item1 = items["attackLevel"].ToString();
                MyData.Instance.charData.attackLevel = int.Parse(item1);
                var item2 = items["coinLevel"].ToString();
                MyData.Instance.charData.coinLevel = int.Parse(item2);
                var item3 = items["manaLevel"].ToString();
                MyData.Instance.charData.manaLevel = int.Parse(item3);
                var item4 = items["highScore"].ToString();
                MyData.Instance.charData.highScore = int.Parse(item4);
                var item5 = items["highWave"].ToString();
                MyData.Instance.charData.highWave = int.Parse(item5);
                var item6 = items["coin"].ToString();
                MyData.Instance.charData.coin = int.Parse(item6);
                var item7 = items["name"].ToString();
                MyData.Instance.charData.name = item7;
                var item8 = items["gameCount"].ToString();
                MyData.Instance.charData.gameCount = int.Parse(item8);

                //안되서 위 코드 수정
                //var item1 = (int)items["attackLevel"];
                //MyData.Instance.charData.attackLevel = item1;
                //var item2 = (int)items["attackLevel"];
                //MyData.Instance.charData.coinLevel = item2;
                //var item3 = (int)items["manaLevel"];
                //MyData.Instance.charData.manaLevel = item3;
                //var item4 = (int)items["highScore"];
                //MyData.Instance.charData.highScore = item4;
                //var item5 = (int)items["highWave"];
                //MyData.Instance.charData.highWave = item5;
                //var item6 = (int)items["coin"];
                //MyData.Instance.charData.coin = item6;
                //var item7 = (string)items["name"];
                //MyData.Instance.charData.name = item7;
                //var item8 = (int)items["gameCount"];
                //MyData.Instance.charData.gameCount = item8;
            }
        });   
	}
}