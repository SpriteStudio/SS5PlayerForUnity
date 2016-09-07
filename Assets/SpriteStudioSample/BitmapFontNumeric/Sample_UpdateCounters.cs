/**
	SpriteStudio5 Player for Unity
	Sample : Counters Control (Scene Script)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class Sample_UpdateCounters : MonoBehaviour
{
	public SS5PUSample_BitmapFontNumeric_Simple InstanceSimple;
	public SS5PUSample_BitmapFontNumeric_Complex_Cell InstanceComplexCell;
	public SS5PUSample_BitmapFontNumeric_Complex_CellTable InstanceComplexCellTable;

	void Start()
	{
	}
	
	void Update ()
	{
		if(null != InstanceSimple)
		{
			InstanceSimple.Value = (int)(Time.realtimeSinceStartup * 1000);
		}
		if(null != InstanceComplexCell)
		{
			InstanceComplexCell.Value = (int)(Time.realtimeSinceStartup * 1000);
		}
		if(null != InstanceComplexCellTable)
		{
			InstanceComplexCellTable.Value = (int)(Time.realtimeSinceStartup * 1000);
		}
	}
}

