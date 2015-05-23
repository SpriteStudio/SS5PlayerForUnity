/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Script_SpriteStudio_AnimationReferenced : ScriptableObject
{
	/* Animation Data */
	public Library_SpriteStudio.AnimationInformationPlay[] ListInformationAnimation;

	/* Node Data */
	public Library_SpriteStudio.AnimationData[] ListNodeAnimationData;

	public Library_SpriteStudio.AnimationData DataGetNode(int NodeID)
	{
#if false
		return(((0 <= NodeID) && (ListNodeAnimationData.Length > NodeID)) ? ListNodeAnimationData[NodeID] : null);
#else
		for(int i=0; i<ListNodeAnimationData.Length; i++)
		{
			if(null != ListNodeAnimationData)
			{
				if(NodeID == ListNodeAnimationData[i].ID)
				{
					return(ListNodeAnimationData[i]);
				}
			}
		}
		return(null);
#endif
	}

	public void Decompress()
	{
		foreach(var it in ListNodeAnimationData)
		{
			it.Decompress();
		}
	}
	public void Compress()
	{
		foreach(var it in ListNodeAnimationData)
		{
			it.Compress();
		}
	}
}
