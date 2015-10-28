/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
/* [InitializeOnLoad] */
[System.Serializable]
public partial class Script_SpriteStudio_LinkPrefab : MonoBehaviour
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
		if((true == FlagAutoDevelop) || (true == EditorApplication.isPlaying))
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
		Script_SpriteStudio_LinkPrefab ComponentScript = gameObject.GetComponent<Script_SpriteStudio_LinkPrefab>();
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
			/* MEMO: "Instance"-Parts has C#-Script ("ScriptPartsInstance" is not null) */
			Script_SpriteStudio_PartsInstance ScriptPartsInstance = gameObject.GetComponent<Script_SpriteStudio_PartsInstance>();
			GameObject PrefabRoot = (GameObject)LinkPrefab;
			GameObject Instance = null;
			Transform TransformChild = gameObject.transform.Find(PrefabRoot.name);
			if(null == TransformChild)
			{
				/* Instantiate Prefab */
#if UNITY_EDITOR
				Instance = (GameObject)PrefabUtility.InstantiatePrefab(LinkPrefab);
#else
				Instance = (GameObject)Instantiate(LinkPrefab);
				Instance.name = LinkPrefab.name;
#endif
				if(null != Instance)
				{
					/* Make "Parent-Child"-Relation  */
					Instance.gameObject.transform.parent = gameObject.transform;
					Instance.gameObject.transform.localPosition = Vector3.zero;
					Instance.gameObject.transform.localEulerAngles = Vector3.zero;
					Instance.gameObject.transform.localScale = Vector3.one;
				}
#if UNITY_EDITOR
				/* Break Prefab Instance */
				PrefabUtility.DisconnectPrefabInstance(gameObject);
#endif
			}
			else
			{
				Instance = TransformChild.gameObject;
			}

			if(null != ScriptPartsInstance)
			{	/* "Instance"-Parts */
				ScriptPartsInstance.LinkSetPartsInstance(Instance);
			}
			else
			{	/* Control-Node */
				Script_SpriteStudio_PartsRoot ScriptRoot = Instance.GetComponent<Script_SpriteStudio_PartsRoot>();
				if(null != ScriptRoot)
				{
					ScriptRoot.NodeSetControl(gameObject);
				}
			}
		}
	}

	/* Don't Execute this function on Run-Time (for Editor-Only) */
	public void PrefabLinkDestroy()
	{
		if(null != LinkPrefab)
		{
			GameObject PrefabRoot = (GameObject)LinkPrefab;
			Transform TransformChild = gameObject.transform.Find(PrefabRoot.name);
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