/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp.
	All rights reserved.
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Library_SpriteStudio
{
	public static class UtilityParts
	{
		/* ******************************************************** */
		//! Get Root-Parts
		/*!
		@param	InstanceOrigin
			GameObject to start the search
		@retval	Return-Value
			Instance of "Script_SpriteStudio_PartsRoot"<br>
			null == Not-Found / Failure

		Get "Script_SpriteStudio_PartsRoot" by examining "InstanceOrigin" and direct children.
		*/
		public static Script_SpriteStudio_PartsRoot InstanceGetRoot(GameObject InstanceOrigin)
		{
			Script_SpriteStudio_PartsRoot ScriptRoot = null;

			/* Check Origin */
			ScriptRoot = InstanceOrigin.GetComponent<Script_SpriteStudio_PartsRoot>();
			if(null != ScriptRoot)
			{	/* Has Root-Parts */
				return(ScriptRoot);
			}

			/* Check Children */
			int CountChild = InstanceOrigin.transform.childCount;
			Transform InstanceTransformChild = null;
			for(int i=0; i<CountChild; i++)
			{
				InstanceTransformChild = InstanceOrigin.transform.GetChild(i);
				ScriptRoot = InstanceTransformChild.gameObject.GetComponent<Script_SpriteStudio_PartsRoot>();
				if(null != ScriptRoot)
				{	/* Has Root-Parts */
					return(ScriptRoot);
				}
			}

			/* Not-Found */
			return(null);
		}

		/* ******************************************************** */
		//! Get GameObject with a particular name
		/*!
		@param	PartsKind
			(Output) Kind of Parts
		@param	SpriteKind
			(Output) Kind of Sprite
		@param	InstanceOrigin
			GameObject to start the search
		@retval	Return-Value
			Instance of GameObject<br>
			null == Not-Found / Failure
		@retval	PartsKind
			Kind of Parts
		@retval	SpriteKind
			Kind of Sprite

		Get GameObject with a particular name by examining "InstanceOrigin" and children.<br>
		<br>
		Value is set to " PartsKind" means the type of Component that GameObject has.<br>
		It has the regularity of the following.<br>
		- "Library_SpriteStudio.KindParts.NORMAL" ... "Script_SpriteStudio_Triangle2" / Script_SpriteStudio_Triangle4"<br>
		- "Library_SpriteStudio.KindParts.ROOT" ... "Script_SpriteStudio_PartsRoot"<br>
		- "Library_SpriteStudio.KindParts.NULL" ... "Script_SpriteStudio_PartsNULL"<br>
		- "Library_SpriteStudio.KindParts.INSTANCE" ... "Script_SpriteStudio_PartsInstance"<br>
		<br>
		Value is set to "SpriteKind" is supplementary information when " PartsKind" is "Library_SpriteStudio.KindParts.NORMAL".<br>
		- "Library_SpriteStudio.KindSprite.TRIANGLE2" ... "Script_SpriteStudio_Triangle2"<br>
		- "Library_SpriteStudio.KindSprite.TRIANGLE4" ... "Script_SpriteStudio_Triangle4"<br>
		- "Library_SpriteStudio.KindSprite.NON" ... the other<br>
		<br>
		If applicable GameObject has no component for "SS5Player for Unity",
			"PartsKind" and "SpriteKind" are set to the contents of the "Library_SpriteStudio.KindParts.NORMAL" and "Library_SpriteStudio.KindSprite.NON" .<br>
		*/
		public static GameObject GameObjectGetName(out KindParts PartsKind, out KindSprite SpriteKind, GameObject InstanceOrigin, string Name)
		{
			/* Clear Output */
			PartsKind = KindParts.NORMAL;
			SpriteKind = KindSprite.NON;
			if(true == String.IsNullOrEmpty(Name))
			{	/* Error */
				return(null);
			}

			/* Check Name */
			if(true == Name.Equals(InstanceOrigin.name))
			{
				Script_SpriteStudio_Triangle2 ScriptTriangle2 = InstanceOrigin.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ScriptTriangle2)
				{	/* Parts: Sprite-Triangle2 */
					PartsKind = KindParts.NORMAL;
					SpriteKind = KindSprite.TRIANGLE2;
					return(InstanceOrigin);
				}
				else
				{
					Script_SpriteStudio_Triangle4 ScriptTriangle4 = InstanceOrigin.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ScriptTriangle4)
					{	/* Parts: Sprite-Triangle4 */
						PartsKind = KindParts.NORMAL;
						SpriteKind = KindSprite.TRIANGLE4;
						return(InstanceOrigin);
					}
					else
					{
						Script_SpriteStudio_PartsNULL ScriptNULL = InstanceOrigin.GetComponent<Script_SpriteStudio_PartsNULL>();
						if(null != ScriptNULL)
						{	/* Parts: NULL */
							PartsKind = KindParts.NULL;
							SpriteKind = KindSprite.NON;
							return(InstanceOrigin);
						}
						else
						{
							Script_SpriteStudio_PartsInstance ScriptInstance = InstanceOrigin.GetComponent<Script_SpriteStudio_PartsInstance>();
							if(null != ScriptInstance)
							{	/* Parts: Instance */
								PartsKind = KindParts.INSTANCE;
								SpriteKind = KindSprite.NON;
								return(InstanceOrigin);
							}
							else
							{
								Script_SpriteStudio_PartsRoot ScriptRoot = InstanceOrigin.GetComponent<Script_SpriteStudio_PartsRoot>();
								if(null != ScriptRoot)
								{	/* Parts: Root */
									PartsKind = KindParts.ROOT;
									SpriteKind = KindSprite.NON;
									return(InstanceOrigin);
								}
								else
								{	/* Parts: Not-for-SpriteStudio */
									PartsKind = KindParts.NORMAL;
									SpriteKind = KindSprite.NON;
									return(InstanceOrigin);
								}
							}
						}
					}
				}
			}

			/* Check Children */
			int CountChild = InstanceOrigin.transform.childCount;
			Transform InstanceTransformChild = null;
			GameObject InstanceChild = null;
			for(int i=0; i<CountChild; i++)
			{
				InstanceTransformChild = InstanceOrigin.transform.GetChild(i);
				InstanceChild = GameObjectGetName(out PartsKind, out SpriteKind, InstanceTransformChild.gameObject, Name);
				if(null != InstanceChild)
				{
					return(InstanceChild);
				}
			}

			/* Not-Found */
			return(null);
		}
	}

	public static class UtilityMaterial
	{
		public static readonly int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

		/* ******************************************************** */
		//! Create Material-Table
		/*!
		@param	TableDataTexture
			Texture-Table (Array of Texture)
		@retval	Return-Value
			Created Material-Table<br>
			null == Failure

		Generate a new Material-Table(Array of Material) from array of texture.<br>
		Generated Material-Table can be set to "Script_SpriteStudio_PartsRoot.TableMaterial".<br>
		<br>
		(Result's) Format: (MT = Material-Table)<br>
		MT[0]: TableDataTexture[0] / Shader for "Mix"-Operation<br>
		MT[1]: TableDataTexture[0] / Shader for "Add"-Operation<br>
		MT[2]: TableDataTexture[0] / Shader for "Sub"-Operation<br>
		MT[3]: TableDataTexture[0] / Shader for "Mul"-Operation<br>
		MT[4]: TableDataTexture[1] / Shader for "Mix"-Operation<br>
		MT[5]: TableDataTexture[1] / Shader for "Add"-Operation<br>
		...<br>
		*/
		public static Material[] TableCreateMaterial(Texture2D[] TableDataTexture)
		{
			if(null == TableDataTexture)
			{
				return(null);
			}
			int CountTexture = TableDataTexture.Length;
			if(0 >= CountTexture)
			{
				return(null);
			}

			Material[] TableDataMaterialNew = new Material[CountTexture * CountTextureBlock];
			int IndexTop;
			for(int i=0; i<CountTexture; i++)
			{
				IndexTop = i * CountTextureBlock;
				for(int j=0; j<CountTextureBlock; j++)
				{
					TableDataMaterialNew[IndexTop + j] = new Material(Shader_SpriteStudioTriangleX[j]);
					TableDataMaterialNew[IndexTop + j].mainTexture = TableDataTexture[j];
				}
			}
			return(TableDataMaterialNew);
		}

		/* ******************************************************** */
		//! Copy (Dupicate) Material-Table
		/*!
		@param	TableDataMaterial
			Source Material-Table (Array of Material)
		@retval	Return-Value
			Created Copy Material-Table<br>
			null == Failure

		Generate a duplicate of "TableDataMaterial".<br>
		Caution: The result is another instance the "TableDataMaterial".<br>
		<br>
		(Result's) Format: (MT = Material-Table)<br>
		MT[0]: Copy of TableDataMaterial[0]<br>
		MT[1]: Copy of TableDataMaterial[1]<br>
		MT[2]: Copy of TableDataMaterial[2]<br>
		...<br>
		*/
		public static Material[] TableCopyMaterial(Material[] TableDataMaterial)
		{
			if(null == TableDataMaterial)
			{
				return(null);
			}
			int CountTableMaterial = TableDataMaterial.Length;
			if(0 >= CountTableMaterial)
			{
				return(null);
			}

			/* Create Copy-Material */
			Material[] TableDataMaterialNew = new Material[CountTableMaterial];
			for(int i=0; i<CountTableMaterial; i++)
			{
				if(null != TableDataMaterial[i])
				{
					TableDataMaterialNew[i] = new Material(TableDataMaterial[i].shader);
					TableDataMaterialNew[i].mainTexture = TableDataMaterial[i].mainTexture;
				}
			}
			return(TableDataMaterialNew);
		}

		/* ******************************************************** */
		//! Get Material-Table Index from Texture-Index.
		/*!
		@param	IndexTexture
			Texture-Index (on Stored Material-Table)
		@retval	Return-Value
			Count of Texture<br>
			0 == Failure

		Calculate index of Material-Table from the Texture-Index.<br>
		1 Texture is defined in 4 Material on the Material-Table, the result is "IndexTexture * 4".<br>
		(4 Material : Shader is assigned to each Material ... "Mix", "Add", "Sub" and "Mul".)<br>
		<br>
		Ex.<br>
		int IndexMaterial = Library_SpriteStudio.IndexGetMaterial(2);<br>
		MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.MIX].mainTexture = Texture02;<br>
		MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.ADD].mainTexture = Texture02;<br>
		MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.SUB].mainTexture = Texture02;<br>
		MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.MUL].mainTexture = Texture02;<br>
		*/
		public static int IndexGetMaterial(int IndexTexture)
		{
			return((0 > IndexTexture) ? -1 : (IndexTexture * CountTextureBlock));
		}

		/* ******************************************************** */
		//! Get count of Textures
		/*!
		@param	TableDataMaterial
			Source Material-Table (Array of Material)
		@retval	Return-Value
			Count of Texture<br>
			0 == Failure

		Get count of textures that can be set to Material-Table.<br>
		"Material-Table" is assigned 4-Material per 1-Texture.<br>
		<br>
		(Materual-Table's) Format: (MT = Material-Table)<br>
		MT[0]: Texture-0 / Shader for "Mix"-Operation<br>
		MT[1]: Texture-0 / Shader for "Add"-Operation<br>
		MT[2]: Texture-0 / Shader for "Sub"-Operation<br>
		MT[3]: Texture-0 / Shader for "Mul"-Operation<br>
		MT[4]: Texture-1 / Shader for "Mix"-Operation<br>
		MT[5]: Texture-1 / Shader for "Add"-Operation<br>
		...<br>
		*/
		public static int TextureGetCount(Material[] TableDataMaterial)
		{
			if(null == TableDataMaterial)
			{
				return(0);
			}
			int CountTableMaterial = TableDataMaterial.Length;
			if(0 >= CountTableMaterial)
			{
				return(0);
			}

			return(CountTableMaterial / CountTextureBlock);
		}

		/* ******************************************************** */
		//! Change 1-Texture in Material-Table
		/*!
		@param	Index
			Texture No.
		@param	DataTexture
			Texture
		@param	TableDataMaterial
			Material-Table
		@retval	Return-Value
			true == Success<br>
			false == Failure

		Change 1-Texture is set in Material-Table.
		Texture is set to "TableDataMaterial[Index * 4] to TableDataMaterial[Index * 4 + 3]".
		*/
		public static bool TextureChange(int Index, Texture2D DataTexture, Material[] TableDataMaterial)
		{
			if(null == TableDataMaterial)
			{
				return(false);
			}
			int CountTableMaterial = TableDataMaterial.Length;
			if(0 >= CountTableMaterial)
			{
				return(false);
			}

			int IndexMaterial = Index * CountTextureBlock;
			if((0 > IndexMaterial) || (CountTableMaterial <= IndexMaterial))
			{
				return(false);
			}
			for(int i=0; i<CountTextureBlock; i++)
			{
				TableDataMaterial[IndexMaterial + i].mainTexture = DataTexture;
			}
			return(true);
		}

		/* ******************************************************** */
		//! Change textures in Material-Table
		/*!
		@param	TableDataTexture
			Texture-Table
		@param	TableDataMaterial
			Material-Table
		@retval	Return-Value
			true == Success<br>
			false == Failure

		Change Multiple-Textures is set in Material-Table.<br>
		Textures are set from Top of the Material-Table.<br>
		Texture exceeds the number that can be stored in "Material-Table" is ignored.<br>
		*/
		public static bool TextureChange(Texture2D[] TableDataTexture, Material[] TableDataMaterial)
		{
			if(null == TableDataMaterial)
			{
				return(false);
			}
			int CountTableMaterial = TableDataMaterial.Length;
			if(0 >= CountTableMaterial)
			{
				return(false);
			}

			if(null == TableDataTexture)
			{
				return(false);
			}
			int CountTexture = TableDataTexture.Length;
			if(0 >= CountTexture)
			{
				return(false);
			}

			int CountChangeLength = CountTexture * CountTextureBlock;
			if(CountChangeLength > CountTableMaterial)
			{
				CountChangeLength = CountTableMaterial;
			}

			for(int i=0; i<CountChangeLength; i++)
			{
				TableDataMaterial[i].mainTexture = TableDataTexture[i / CountTextureBlock];
			}
			return(true);
		}
	}
}