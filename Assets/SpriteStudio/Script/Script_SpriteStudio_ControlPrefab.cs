/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio_ControlPrefab : MonoBehaviour
{
	public Script_SpriteStudio_ManagerDraw InstanceManagerDraw;
	public Object PrefabUnderControl;

//	void Awake()
//	{
//	}

	void Start()
	{
		if(null != PrefabUnderControl)
		{
			GameObject InstanceGameObjectChild = Library_SpriteStudio.Miscellaneousness.Asset.PrefabInstantiateChild(gameObject, (GameObject)PrefabUnderControl, false);
			if(null != InstanceGameObjectChild)
			{
				/* Instantiate Under-Control Prefab */
				Script_SpriteStudio_Root InstanceRootChild = InstanceGameObjectChild.GetComponent<Script_SpriteStudio_Root>();
				if(null != InstanceRootChild)
				{
					InstanceRootChild.InstanceGameObjectControl = gameObject;

					if(null == InstanceRootChild.InstanceManagerDraw)
					{
						InstanceRootChild.InstanceManagerDraw = InstanceManagerDraw;
					}
				}

			}
		}
	}
	
//	void Update()
//	{
//	}

//	void LateUpdate()
//	{
//	}

}
