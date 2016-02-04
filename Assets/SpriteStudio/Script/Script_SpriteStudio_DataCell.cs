/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[System.Serializable]
public partial class Script_SpriteStudio_DataCell : ScriptableObject
{
	public Library_SpriteStudio.Data.CellMap[] ListDataCellMap;

	public int CountGetCellMap()
	{
		return((null != ListDataCellMap) ? ListDataCellMap.Length : -1);
	}
	public int IndexGetCellMap(string Name)
	{
		if(true == string.IsNullOrEmpty(Name))
		{
			return(-1);
		}

		int Count = ListDataCellMap.Length;
		for(int i=0; i<Count; i++)
		{
			if(0 == string.Compare(Name, ListDataCellMap[i].Name))
			{
				return(i);
			}
		}
		return(-1);
	}
	public Library_SpriteStudio.Data.CellMap DataGetCellMap(int Index)
	{
		return(((0 <= Index) && (ListDataCellMap.Length > Index)) ? ListDataCellMap[Index] : null);
	}
}
