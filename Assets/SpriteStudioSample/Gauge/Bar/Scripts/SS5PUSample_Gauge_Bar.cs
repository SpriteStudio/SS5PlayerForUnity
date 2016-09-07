/**
	SpriteStudio5 Player for Unity
	Sample : Gauge Bar

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class SS5PUSample_Gauge_Bar : MonoBehaviour
{
	/* Target Animation-Object */
	public Script_SpriteStudio_Root InstanceRoot;

	/* Value */
	public float Rate;

	/* [Constant] Animation-Object's Datas */
	private const string NamePartsAffectRate = "Bar";

	/* Work-Area */
	private int IDPartsAffectRate = -1;
	private Transform InstanceTransformPartsAffectRate = null;
	private float RatePrevious = -1;

	void Start()
	{
		/* Initialize Animation-Datas */
		if(null != InstanceRoot)
		{
			/* Start Animation */
			int IndexAnimation = InstanceRoot.IndexGetAnimation("Normal");
			if(0 <= IndexAnimation)
			{
				InstanceRoot.AnimationPlay(IndexAnimation, 0);	/* Loop, Infinite */
			}

			/* Get Parts-ID */
			IDPartsAffectRate = InstanceRoot.IDGetParts(NamePartsAffectRate);
		}
	}
	
	void Update()
	{
		/* Get Part's Transform */
		if(0 <= IDPartsAffectRate)
		{
			if(null == InstanceTransformPartsAffectRate)
			{
				Library_SpriteStudio.Control.Parts InstanceParts = InstanceRoot.ControlGetParts(IDPartsAffectRate);
				if(null != InstanceParts)
				{
					GameObject InstanceGameObject = InstanceParts.InstanceGameObject;
					if(null != InstanceGameObject)
					{
						InstanceTransformPartsAffectRate = InstanceGameObject.transform;
					}
				}
			}
		}

		/* Clamp Rate */
		Rate = Mathf.Clamp01(Rate);

		/* Set Rate */
		if(Rate != RatePrevious)
		{
			if(null != InstanceTransformPartsAffectRate)
			{
				Vector3 Scale = Vector3.one;
				Scale.x = Rate;
				InstanceTransformPartsAffectRate.localScale = Scale;
			}

			RatePrevious = Rate;
		}
	}
}

