/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[System.Serializable]
public partial class Script_SpriteStudio_DataEffect : ScriptableObject
{
	/* Parts-Relation Data */
	public Library_SpriteStudio.Data.PartsEffect[] ListDataParts;

	/* Emitters' Data */
	public Library_SpriteStudio.Data.EmitterEffect[] ListDataEmitter;

	public int CountGetParts()
	{
		return(ListDataParts.Length);
	}
	public Library_SpriteStudio.Data.PartsEffect DataGetParts(int Index)
	{
		return(((0 <= Index) && (ListDataParts.Length > Index)) ? ListDataParts[Index] : null);
	}
	public int IndexGetParts(string Name)
	{
		if(true == string.IsNullOrEmpty(Name))
		{
			return(-1);
		}

		int Count = ListDataParts.Length;
		for(int i=0; i<Count; i++)
		{
			if(0 == string.Compare(Name, ListDataParts[i].Name))
			{
				return(i);
			}
		}
		return(-1);
	}

	public int CountGetEmitter()
	{
		return(ListDataEmitter.Length);
	}
	public Library_SpriteStudio.Data.EmitterEffect DataGetEmitter(int Index)
	{
		return(((0 <= Index) && (ListDataEmitter.Length > Index)) ? ListDataEmitter[Index] : null);
	}
}