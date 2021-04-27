using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;

using WebSocketSharp;

public class ServerData : MonoSingleton<ServerData>
{
    private DatabaseReference reference;
    private string userKey;

          Dictionary<string, object> items = new Dictionary<string, object>();
    public void Init(string userKey)
	{
		AppOptions options = new AppOptions { DatabaseUrl = new Uri("https://slader-21906011-default-rtdb.firebaseio.com/") };
        FirebaseApp app = FirebaseApp.Create(options);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        this.userKey = userKey;
	}
	public void SaveData()
	{
        if(!AuthManager.Instance.IsFirebaseReady)
        {
            return;
        }

        string json = JsonUtility.ToJson(MyData.Instance.charData);
        reference.Child("Users").Child(userKey).SetRawJsonValueAsync(json);
	}
	public void LoadData()
	{
        if(!AuthManager.Instance.IsFirebaseReady)
        {
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
            }
            else
            {
                MyData.Instance.charData = new CharData();
            }
        });   
	}
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            MyData.Instance.charData.attackLevel = (int)items["attackLevel"];
		    MyData.Instance.charData.coinLevel = (int)items["coinLevel"];
		    MyData.Instance.charData.manaLevel = (int)items["manaLevel"];
            MyData.Instance.charData.highScore = (int)items["highScore"];
		    MyData.Instance.charData.highWave = (int)items["highWave"];
		    MyData.Instance.charData.coin = (int)items["coin"];
		    MyData.Instance. charData.name = (string)items["name"];
		    MyData.Instance.charData.gameCount = (int)items["gameCount"]; 
        }
    }
}