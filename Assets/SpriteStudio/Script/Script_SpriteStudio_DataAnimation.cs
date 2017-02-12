/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[System.Serializable]
public partial class Script_SpriteStudio_DataAnimation : ScriptableObject
{
	/* Parts-Relation Data */
	public Library_SpriteStudio.Data.Parts[] ListDataParts;

	/* Animation Data */
	public Library_SpriteStudio.Data.Animation[] ListDataAnimation;

	public int CountGetParts()
	{
		return(ListDataParts.Length);
	}
	public int CountGetPartsDraw()
	{
		int CountParts = ListDataParts.Length;
		int Count = 0;
		Library_SpriteStudio.Data.Parts DataParts = null;
		for(int i=0; i<CountParts; i++)
		{
			DataParts = DataGetParts(i);
			switch(DataParts.Kind)
			{
				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
				case Library_SpriteStudio.KindParts.INSTANCE:
				case Library_SpriteStudio.KindParts.EFFECT:
					Count++;
					break;

				case Library_SpriteStudio.KindParts.NON:
				case Library_SpriteStudio.KindParts.ROOT:
				case Library_SpriteStudio.KindParts.NULL:
					break;
			}
		}
		return(Count);
	}
	public Library_SpriteStudio.Data.Parts DataGetParts(int Index)
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
//			if(0 == string.Compare(Name, ListDataParts[i].Name))
			if(Name == ListDataParts[i].Name)
			{
				return(i);
			}
		}
		return(-1);
	}

	public int CountGetAnimation()
	{
		return(ListDataAnimation.Length);
	}
	public int IndexGetAnimation(string Name)
	{
		if(true == string.IsNullOrEmpty(Name))
		{
			return(-1);
		}

		int Count = ListDataAnimation.Length;
		for(int i=0; i<Count; i++)
		{
//			if(0 == string.Compare(Name, ListDataAnimation[i].Name))
			if(Name == ListDataAnimation[i].Name)
			{
				return(i);
			}
		}
		return(-1);
	}
	public Library_SpriteStudio.Data.Animation DataGetAnimation(int Index)
	{
		return(((0 <= Index) && (ListDataAnimation.Length > Index)) ? ListDataAnimation[Index] : null);
	}
}
