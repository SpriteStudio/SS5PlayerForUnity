/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

public static partial class Library_SpriteStudio
{
	public static partial class Utility
	{
		public static partial class Parts
		{
			/* ******************************************************** */
			//! Get Camera
			/*!
			@param	InstanceGameObject
				GameObject to start the search
			@retval	Return-Value
				Instance of Camera<br>
				null == Not-Found / Failure

			Get Camera by examining "InstanceGameObject" and parents.
			*/
			public static Camera CameraGetParent(GameObject InstanceGameObject)
			{
				Transform InstanceTransform = InstanceGameObject.transform;
				Camera InstanceCamera = null;
				while(null != InstanceTransform)
				{
					InstanceCamera = InstanceTransform.gameObject.GetComponent<Camera>();
					if(null != InstanceCamera)
					{
						break;
					}
					InstanceTransform = InstanceTransform.parent;
				}
				return(InstanceCamera);
			}

			/* ******************************************************** */
			//! Get Manager-Draw
			/*!
			@param	InstanceGameObject
				GameObject to start the search
			@retval	Return-Value
				Instance of "Script_SpriteStudio_ManagerDraw"<br>
				null == Not-Found / Failure

			Get "Script_SpriteStudio_ManagerDraw" by examining "InstanceGameObject" and parents.
			*/
			public static Script_SpriteStudio_ManagerDraw ManagerDrawGetParent(GameObject InstanceGameObject)
			{
				Transform InstanceTransform = InstanceGameObject.transform;
				Script_SpriteStudio_ManagerDraw InstanceManagerDraw = null;
				while(null != InstanceTransform)
				{
					InstanceManagerDraw = InstanceTransform.gameObject.GetComponent<Script_SpriteStudio_ManagerDraw>();
					if(null != InstanceManagerDraw)
					{
						break;
					}
					InstanceTransform = InstanceTransform.parent;
				}
				return(InstanceManagerDraw);
			}

			/* ******************************************************** */
			//! Get Root-Parts
			/*!
			@param	InstanceGameObject
				GameObject to start the search
			@retval	Return-Value
				Instance of "Script_SpriteStudio_Root"<br>
				null == Not-Found / Failure

			Get "Script_SpriteStudio_Root" by examining "InstanceGameObject" and direct children.
			*/
			public static Script_SpriteStudio_Root RootGetChild(GameObject InstanceGameObject)
			{
				Script_SpriteStudio_Root ScriptRoot = null;

				/* Check Origin */
				ScriptRoot = InstanceGameObject.GetComponent<Script_SpriteStudio_Root>();
				if(null != ScriptRoot)
				{	/* Has Root-Parts */
					return(ScriptRoot);
				}

				/* Check Children */
				int CountChild = InstanceGameObject.transform.childCount;
				Transform InstanceTransformChild = null;
				for(int i=0; i<CountChild; i++)
				{
					InstanceTransformChild = InstanceGameObject.transform.GetChild(i);
					ScriptRoot = InstanceTransformChild.gameObject.GetComponent<Script_SpriteStudio_Root>();
					if(null != ScriptRoot)
					{	/* Has Root-Parts */
						return(ScriptRoot);
					}
				}

				return(null);
			}
		}
	}
}
