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
		public static partial class TableMaterial
		{
			public static readonly int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;
			public static readonly int CountTextureBlockEffect = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR_EFFECT - 1;

			/* ******************************************************** */
			//! Create Material-Table (for "Root")
			/*!
			@param	TableDataTexture
				Texture-Table (Array of Texture)
			@retval	Return-Value
				Created Material-Table<br>
				null == Failure

			Generate a new Material-Table(Array of Material) from array of texture.<br>
			Generated Material-Table can be set to "Script_SpriteStudio_Root.TableMaterial".<br>
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
			public static Material[] Create(Texture2D[] TableDataTexture)
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
						TableDataMaterialNew[IndexTop + j] = new Material(Library_SpriteStudio.Shader_SpriteStudioTriangleX[j]);
						TableDataMaterialNew[IndexTop + j].mainTexture = TableDataTexture[j];
					}
				}
				return(TableDataMaterialNew);
			}

			/* ******************************************************** */
			//! Create Material-Table (for "RootEffect")
			/*!
			@param	TableDataTexture
				Texture-Table (Array of Texture)
			@retval	Return-Value
				Created Material-Table<br>
				null == Failure

			Generate a new Material-Table(Array of Material) from array of texture.<br>
			Generated Material-Table can be set to "Script_SpriteStudio_RootEffect.TableMaterial".<br>
			<br>
			(Result's) Format: (MT = Material-Table)<br>
			MT[0]: TableDataTexture[0] / Shader for "Mix"-Operation<br>
			MT[1]: TableDataTexture[0] / Shader for "Add"-Operation<br>
			MT[2]: TableDataTexture[1] / Shader for "Mix"-Operation<br>
			MT[3]: TableDataTexture[1] / Shader for "Add"-Operation<br>
			...<br>
			*/
			public static Material[] CreateEffect(Texture2D[] TableDataTexture)
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

				Material[] TableDataMaterialNew = new Material[CountTexture * CountTextureBlockEffect];
				int IndexTop;
				for(int i=0; i<CountTexture; i++)
				{
					IndexTop = i * CountTextureBlockEffect;
					for(int j=0; j<CountTextureBlockEffect; j++)
					{
						TableDataMaterialNew[IndexTop + j] = new Material(Library_SpriteStudio.Shader_SpriteStudioEffect[j]);
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
			public static Material[] Copy(Material[] TableDataMaterial)
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
			//! Get Material-Table Index from Texture-Index (for "Root")
			/*!
			@param	IndexTexture
				Texture-Index (on Stored Material-Table)
			@retval	Return-Value
				Count of Texture<br>
				0 == Failure

			Calculate index of Material-Table (for "Script_SpriteStudio_Root") from the Texture-Index.<br>
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
			//! Get Material-Table Index from Texture-Index (for "RootEffect")
			/*!
			@param	IndexTexture
				Texture-Index (on Stored Material-Table)
			@retval	Return-Value
				Count of Texture<br>
				0 == Failure

			Calculate index of Material-Table (for "Script_SpriteStudio_RootEffect") from the Texture-Index.<br>
			1 Texture is defined in 2 Material on the Material-Table, the result is "IndexTexture * 2".<br>
			(4 Material : Shader is assigned to each Material ... "Mix" and "Add".)<br>
			<br>
			Ex.<br>
			int IndexMaterial = Library_SpriteStudio.IndexGetMaterialEffect(3);<br>
			MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.MIX].mainTexture = Texture03;<br>
			MaterialTable[IndexMaterial + (int)Library_SpriteStudio.KindColorOperation.ADD].mainTexture = Texture03;<br>
			*/
			public static int IndexGetMaterialEffect(int IndexTexture)
			{
				return((0 > IndexTexture) ? -1 : (IndexTexture * CountTextureBlockEffect));
			}

			/* ******************************************************** */
			//! Get count of Textures (for "Root")
			/*!
			@param	TableDataMaterial
				Source Material-Table (Array of Material)
			@retval	Return-Value
				Count of Texture<br>
				0 == Failure

			Get count of textures that can be set to Material-Table (for "Script_SpriteStudio_Root").<br>
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
			public static int CountGetTexture(Material[] TableDataMaterial)
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
			//! Get count of Textures (for "RootEffect")
			/*!
			@param	TableDataMaterial
				Source Material-Table (Array of Material)
			@retval	Return-Value
				Count of Texture<br>
				0 == Failure

			Get count of textures that can be set to Material-Table (for "Script_SpriteStudio_RootEffect").<br>
			"Material-Table" is assigned 2-Material per 1-Texture.<br>
			<br>
			(Materual-Table's) Format: (MT = Material-Table)<br>
			MT[0]: Texture-0 / Shader for "Mix"-Operation<br>
			MT[1]: Texture-0 / Shader for "Add"-Operation<br>
			MT[2]: Texture-1 / Shader for "Mix"-Operation<br>
			MT[3]: Texture-1 / Shader for "Add"-Operation<br>
			...<br>
			*/
			public static int CountGetTextureEffect(Material[] TableDataMaterial)
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

				return(CountTableMaterial / CountTextureBlockEffect);
			}

			/* ******************************************************** */
			//! Change 1-Texture in Material-Table (for "Root")
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

			Change 1-Texture is set in Material-Table (for "Script_SpriteStudio_Root").<br>
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
			//! Change 1-Texture in Material-Table (for "RootEffect")
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

			Change 1-Texture is set in Material-Table (for "Script_SpriteStudio_RootEffect").<br>
			Texture is set to "TableDataMaterial[Index * 2] to TableDataMaterial[Index * 2 + 3]".
			*/
			public static bool TextureChangeEffect(int Index, Texture2D DataTexture, Material[] TableDataMaterial)
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

				int IndexMaterial = Index * CountTextureBlockEffect;
				if((0 > IndexMaterial) || (CountTableMaterial <= IndexMaterial))
				{
					return(false);
				}
				for(int i=0; i<CountTextureBlockEffect; i++)
				{
					TableDataMaterial[IndexMaterial + i].mainTexture = DataTexture;
				}
				return(true);
			}

			/* ******************************************************** */
			//! Change textures in Material-Table (for "Root")
			/*!
			@param	TableDataTexture
				Texture-Table
			@param	TableDataMaterial
				Material-Table
			@retval	Return-Value
				true == Success<br>
				false == Failure

			Change Multiple-Textures is set in Material-Table (for "Script_SpriteStudio_Root").<br>
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

			/* ******************************************************** */
			//! Change textures in Material-Table (for "RootEffect")
			/*!
			@param	TableDataTexture
				Texture-Table
			@param	TableDataMaterial
				Material-Table
			@retval	Return-Value
				true == Success<br>
				false == Failure

			Change Multiple-Textures is set in Material-Table (for "Script_SpriteStudio_RootEffect").<br>
			Textures are set from Top of the Material-Table.<br>
			Texture exceeds the number that can be stored in "Material-Table" is ignored.<br>
			*/
			public static bool TextureChangeEffect(Texture2D[] TableDataTexture, Material[] TableDataMaterial)
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

				int CountChangeLength = CountTexture * CountTextureBlockEffect;
				if(CountChangeLength > CountTableMaterial)
				{
					CountChangeLength = CountTableMaterial;
				}

				for(int i=0; i<CountChangeLength; i++)
				{
					TableDataMaterial[i].mainTexture = TableDataTexture[i / CountTextureBlockEffect];
				}
				return(true);
			}
		}
	}
}
