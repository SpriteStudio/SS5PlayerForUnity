/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp.
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Library_SpriteStudio
{
	public static class UtilityMaterial
	{
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
			int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

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

			return(CountTableMaterial / ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1));
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
			int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

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
			int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

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