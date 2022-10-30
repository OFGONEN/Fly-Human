using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "currency", menuName = "FF/Game/Currency" ) ]
public class Currency : SharedIntNotifier
{
    public void Load()
    {
		SharedValue = PlayerPrefsUtility.Instance.GetInt( ExtensionMethods.Key_Currency, 0 );
	}

    public void Save()
    {
		PlayerPrefsUtility.Instance.SetInt( ExtensionMethods.Key_Currency, sharedValue );
	}
}