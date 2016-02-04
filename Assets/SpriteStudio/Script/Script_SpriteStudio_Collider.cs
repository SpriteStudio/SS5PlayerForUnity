/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio_Collider : MonoBehaviour
{
	/* Relation Datas */
	public Script_SpriteStudio_Root InstanceRoot;
	public int IDParts;

	/* WorkArea for Runtime (Root's WorkArea) */
	internal float ColliderRadiusPrevious = -1.0f;	/* for Radius-Collision */
	internal Vector2 ColliderRectSizePrevious = Vector2.zero;	/* for Rectangle-Collision */
	internal Vector2 ColliderRectPivotPrevious = Vector2.zero;	/* for Rectangle-Collision */

//	void Awake()
//	{
//	}

//	void Start()
//	{
//	}

//	void Update()
//	{
//	}

// 	void LateUpdate()
//	{
//	}

	void OnTriggerEnter(Collider Pair)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecColliderOnTriggerEnter(IDParts, Pair);
		}
	}

	void OnTriggerExit(Collider Pair)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecColliderOnTriggerExit(IDParts, Pair);
		}
	}

	void OnTriggerStay(Collider Pair)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecColliderOnTriggerStay(IDParts, Pair);
		}
	}

	void OnCollisionEnter(Collision Contacts)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecCollisionTriggerEnter(IDParts, Contacts);
		}
	}

	void OnCollisionExit(Collision Contacts)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecCollisionTriggerExit(IDParts, Contacts);
		}
	}

	void OnCollisionStay(Collision Contacts)
	{
		if(null != InstanceRoot)
		{
			InstanceRoot.CallBackExecCollisionTriggerStay(IDParts, Contacts);
		}
	}
}