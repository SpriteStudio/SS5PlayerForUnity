/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_PartsInstance))]
public class Inspector_SpriteStudio_PartsInstance : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_PartsInstance Data = (Script_SpriteStudio_PartsInstance)target;

		EditorGUILayout.LabelField("[SpriteStudio Parts-Instance]");
		EditorGUILayout.Space();		

		Data.FlagHideForce = EditorGUILayout.Toggle("Force-Hide", Data.FlagHideForce);
		if(true == GUILayout.Button("Apply \"Force-Hide\" to Children"))
		{
			LibraryEditor_SpriteStudio.Utility.HideSetForce(Data.gameObject, Data.FlagHideForce, true, false);
		}
		EditorGUILayout.Space();

#if false
		/* MEMO: Experimentally & Sample */
		int	AnimationNoOld = Data.AnimationNo;
		Data.AnimationNo = EditorGUILayout.IntField("Animation No", Data.AnimationNo);
		if(AnimationNoOld != Data.AnimationNo)
		{
			Data.AnimationChangeInstance(Data.AnimationNo);
		}
#endif

#if false
		/* MEMO: Experimentally & Sample */
		GameObject PrefabNew = null;
		PrefabNew = (GameObject)(EditorGUILayout.ObjectField("Prefab ", PrefabNew, typeof(GameObject), false));
		if(null != PrefabNew)
		{
			Data.PrefabChangeInstance(PrefabNew);
		}
#endif

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
