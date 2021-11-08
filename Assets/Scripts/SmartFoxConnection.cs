using Sfs2X;
using Sfs2X.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance;
	private static SmartFox sfs;

	public static SmartFox Connection
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			return sfs;
		}
		set
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
			}
			sfs = value;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return (sfs != null);
		}
	}

	//public static void LoginToZone(string zone)
 //   {
	//	sfs.Send(new LoginRequest(sfs.MySelf.Name, "", "zone"));
	//}

	// Handle disconnection automagically
	// ** Important for Windows users - can cause crashes otherwise
	void OnApplicationQuit()
	{
		if (sfs != null && sfs.IsConnected)
		{
			sfs.Disconnect();
		}
	}

	// Disconnect from the socket when ordered by the main Panel scene
	// ** Important for Windows users - can cause crashes otherwise
	public void Disconnect()
	{
		OnApplicationQuit();
	}
}
