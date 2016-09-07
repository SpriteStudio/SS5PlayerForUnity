/**
	SpriteStudio5 Player for Unity
	Sample : Gauges Control (Scene Script)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class Sample_UpdateGauges : MonoBehaviour
{
	public SS5PUSample_Gauge_Bar InstanceBar;
	public SS5PUSample_Gauge_Meter InstanceMeter;

	/* Work-Area */
	private float ValueStart = 0.0f;
	private float ValueEnd = 0.0f;
	private float TimeEnd = -1.0f;
	private float TimeNow = 0.0f;

	void Start()
	{
		/* Initialize Work-Area */
		ValueStart = 0.0f;
		ValueEnd = 0.0f;
		TimeEnd = -1.0f;
		TimeNow = 0.0f;
	}
	
	void Update ()
	{
		/* Values Refresh */
		if(TimeEnd <= TimeNow)
		{
			ValueStart = ValueEnd;
			ValueEnd = Random.value;

			TimeNow = 0.0f;
			TimeEnd = Random.value + 0.5f;
		}
		else
		{
			TimeNow += Time.deltaTime;
		}

		float Value = Mathf.Lerp(ValueStart, ValueEnd, TimeNow);

		if(null != InstanceBar)
		{
			InstanceBar.Rate = 1.0f - Value;
		}
		if(null != InstanceMeter)
		{
			InstanceMeter.Rate = Value;
		}
	}
}
