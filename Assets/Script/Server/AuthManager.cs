using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

using System;
using System.Collections;

public class AuthManager : MonoSingleton<AuthManager>
{
	public bool IsFirebaseReady { get; private set; }

	public FirebaseAuth firebaseAuth;
	public FirebaseApp firebaseApp;
	public FirebaseUser firebaseUser;

	private string userKey;
	public void Init()
	{
		GooglePlayGameActive();

		firebaseAuth = FirebaseAuth.DefaultInstance;
		MyData.Instance.IsAuthInit = true;

		GoogleLogin();
	}
	public void GooglePlayGameActive()
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.RequestServerAuthCode(false)
			.RequestIdToken()
			.RequestEmail()
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
	}
	public void GoogleLogin()
	{
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate(success =>
			{
				if (success)
				{
					MyData.Instance.authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
					MyData.Instance.email = ((PlayGamesLocalUser)Social.localUser).Email;

					DebugOptimum.Log("구글과 파이어베이스 연동에 성공하였습니다.: " + MyData.Instance.email);
					DebugOptimum.Log("구글 토큰 : " + MyData.Instance.authCode);
					DebugOptimum.Log("구글 이메일 : " + MyData.Instance.email);

					StartCoroutine(FirebaseLogin());
				}
				else
				{
					DebugOptimum.Log("구글과 파이어베이스 연동에 실패하였습니다.: ");
					DataController.Instance.LoadGameData();
				}

				DebugOptimum.Log("google " + success);
			});
		}
	}
	public IEnumerator FirebaseLogin()
	{
		while (string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
		{
        	yield return null;
		}

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
				DebugOptimum.Log("파이어베이스 로그인 오류");
                return;
            }
			else
			{
				DebugOptimum.Log("파이어베이스 로그인 성공");
				IsFirebaseReady = true;
				Firebase.Auth.FirebaseUser newUser = task.Result;
				userKey = newUser.UserId;

				ServerData.Instance.Init(userKey);
				ServerData.Instance.LoadData();
			}
        });
	}
}
