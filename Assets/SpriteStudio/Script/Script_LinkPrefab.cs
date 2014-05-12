/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
/* CAUTION!: This Script is only running on "Edit-Mode" */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class Script_LinkPrefab : MonoBehaviour
{
	public bool FlagAutoDevelop;
	public bool FlagDeleteScript;
	public Object LinkPrefab;

	void Awake()
	{
	}

	void Start()
	{
#if UNITY_EDITOR
		if(true == FlagAutoDevelop)
		{
			PrefabLinkInstantiate();
		}

		if(true == FlagDeleteScript)
		{
			PrefabLinkDeleteScript();
		}
#else
		PrefabLinkInstantiate();
		PrefabLinkDeleteScript();
#endif
	}

	public void PrefabLinkDeleteScript()
	{
		/* Get my component */
		Script_LinkPrefab ComponentScript = gameObject.GetComponent<Script_LinkPrefab>();
		if(null != ComponentScript)
		{
			/* Delete my self */
#if UNITY_EDITOR
			DestroyImmediate(ComponentScript);
#else
			Destroy(ComponentScript);
#endif

		}
	}

	public void PrefabLinkInstantiate()
	{
		if(null != LinkPrefab)
		{
			GameObject PrefabRoot = (GameObject)LinkPrefab;
			Transform TransformChild = gameObject.transform.FindChild(PrefabRoot.name);
			if(null == TransformChild)
			{
				/* Instantiate Prefab */
#if UNITY_EDITOR
				GameObject Instance = (GameObject)PrefabUtility.InstantiatePrefab(LinkPrefab);
#else
				GameObject Instance = (GameObject)Instantiate(LinkPrefab);
#endif
				if(null != Instance)
				{
					/* Make "Parent-Child"-Relation  */
					Instance.gameObject.transform.parent = gameObject.transform;
				}

#if UNITY_EDITOR
				/* Break Prefab Instance */
				PrefabUtility.DisconnectPrefabInstance(gameObject);
#endif
			}
		}
	}

	/* Don't Execute this function on Run-Time (for Editor-Only) */
	public void PrefabLinkDestroy()
	{
		if(null != LinkPrefab)
		{
			GameObject PrefabRoot = (GameObject)LinkPrefab;
			Transform TransformChild = gameObject.transform.FindChild(PrefabRoot.name);
			if(null != TransformChild)
			{
				GameObject GameObjectChild = TransformChild.gameObject;

#if UNITY_EDITOR
				DestroyImmediate(GameObjectChild);
#else
				Destroy(GameObjectChild);
#endif
			}
		}
	}
}
