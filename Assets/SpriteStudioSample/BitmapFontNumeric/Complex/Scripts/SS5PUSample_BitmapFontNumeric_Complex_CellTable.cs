/**
	SpriteStudio5 Player for Unity
	Sample : Numeric Bitmap-Font (Font Managed Cell-Table Changing)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class SS5PUSample_BitmapFontNumeric_Complex_CellTable : MonoBehaviour
{
	/* Target Animation-Object */
	public Script_SpriteStudio_Root InstanceRoot;

	/* Value */
	public int Value;

	/* Color */
	public Color ColorFontOverlay;
	public Library_SpriteStudio.KindColorOperation KindOperationFontOverlay;

	/* Form */
	public int IndexFont;
	public int SizePixelFont;
	public bool FlagPaddingZero;
	public bool FlagProportional;

	/* Appendix (Play-Function)  */
	public Script_SpriteStudio_DataCell OutsideSSPJFontCellMap;
	public Texture2D OutsideSSPJFontTexture;

	/* [Constant] Animation-Object's Datas */
	private enum Constant
	{
		FONT_INSIDEPROJECT_MAX = 4,
		FONT_OUTSIDEPROJECT_MAX = 1,
		FONT_MAX = (FONT_INSIDEPROJECT_MAX + FONT_OUTSIDEPROJECT_MAX),
		DIGIT_MAX = 8,
	};
	private static readonly int ValueMax = (int)(Mathf.Pow(10.0f, (int)Constant.DIGIT_MAX)) - 1;
	private static readonly int ValueMin = -((int)(Mathf.Pow(10.0f, (int)Constant.DIGIT_MAX - 1)) - 1);

	private enum KindFormat
	{
		NORMAL = 0,
		PADDING_ZERO,

		TERMINATOR
	};
	private enum KindCharacter	/* MEMO: == Cell Index */
	{
		NUMBER_0 = 0,
		NUMBER_1,
		NUMBER_2,
		NUMBER_3,
		NUMBER_4,
		NUMBER_5,
		NUMBER_6,
		NUMBER_7,
		NUMBER_8,
		NUMBER_9,
		SYMBOL_PERIOD,
		SYMBOL_COMMA,
		SYMBOL_PLUS,
		SYMBOL_MINUS,
		SYMBOL_MUL,
		SYMBOL_DIV,

		TERMINATOR
	};
	private static readonly char[] ListCharacters = new char[(int)KindCharacter.TERMINATOR]
	{
		'0',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9',
		'.',
		',',
		'+',
		'-',
		'*',
		'/',
	};

	private enum KindTexture
	{
		INNER = 0,	/* Default(Font-0 to 3) */
		OUTER = 1,	/* Appendix(Font-4) */

		TERMINATOR
	};
	private static readonly int[] IndexTextures = new int[(int)Constant.FONT_MAX]
	{
		(int)KindTexture.INNER,	/* Font-0 (Inside: 0) */
		(int)KindTexture.INNER,	/* Font-1 (Inside: 1) */
		(int)KindTexture.INNER,	/* Font-2 (Inside: 2) */
		(int)KindTexture.INNER,	/* Font-3 (Inside: 3) */
		(int)KindTexture.OUTER,	/* Font-4 (Outside: 0) */
	};

	private static readonly string[][] NameCells = new string[(int)Constant.FONT_MAX][]
	{
		new string[(int)KindCharacter.TERMINATOR]	/* Font-0 */
		{
			"Font1_0",
			"Font1_1",
			"Font1_2",
			"Font1_3",
			"Font1_4",
			"Font1_5",
			"Font1_6",
			"Font1_7",
			"Font1_8",
			"Font1_9",
			"Font1_Period",
			"Font1_Comma",
			"Font1_Plus",
			"Font1_Minus",
			"Font1_Mul",
			"Font1_Div",
		},
		new string[(int)KindCharacter.TERMINATOR]	/* Font-1 */
		{
			"Font2_0",
			"Font2_1",
			"Font2_2",
			"Font2_3",
			"Font2_4",
			"Font2_5",
			"Font2_6",
			"Font2_7",
			"Font2_8",
			"Font2_9",
			"Font2_Period",
			"Font2_Comma",
			"Font2_Plus",
			"Font2_Minus",
			"Font2_Mul",
			"Font2_Div",
		},
		new string[(int)KindCharacter.TERMINATOR]	/* Font-2 */
		{
			"Font3_0",
			"Font3_1",
			"Font3_2",
			"Font3_3",
			"Font3_4",
			"Font3_5",
			"Font3_6",
			"Font3_7",
			"Font3_8",
			"Font3_9",
			"Font3_Period",
			"Font3_Comma",
			"Font3_Plus",
			"Font3_Minus",
			"Font3_Mul",
			"Font3_Div",
		},
		new string[(int)KindCharacter.TERMINATOR]	/* Font-3 */
		{
			"Font4_0",
			"Font4_1",
			"Font4_2",
			"Font4_3",
			"Font4_4",
			"Font4_5",
			"Font4_6",
			"Font4_7",
			"Font4_8",
			"Font4_9",
			"Font4_Period",
			"Font4_Comma",
			"Font4_Plus",
			"Font4_Minus",
			"Font4_Mul",
			"Font4_Div",
		},
		new string[(int)KindCharacter.TERMINATOR]	/* Font-4 (Appendix: Outside Project) */
		{
			"Font1_0",
			"Font1_1",
			"Font1_2",
			"Font1_3",
			"Font1_4",
			"Font1_5",
			"Font1_6",
			"Font1_7",
			"Font1_8",
			"Font1_9",
			"Font1_Period",
			"Font1_Comma",
			"Font1_Plus",
			"Font1_Minus",
			"Font1_Mul",
			"Font1_Div",
		},
	};

	private static readonly string[] NameParts = new string[(int)Constant.DIGIT_MAX]
	{
		"Digit00",
		"Digit01",
		"Digit02",
		"Digit03",
		"Digit04",
		"Digit05",
		"Digit06",
		"Digit07",
	};

	private static readonly string[] FormatToString = new string[(int)KindFormat.TERMINATOR]
	{
		"D",
		"D" + ((int)(Constant.DIGIT_MAX)).ToString(),
	};

	/* WorkArea */
	private int IndexFontPrevious;
	private int ValuePrevious;
	private bool FlagProportionalPrevious;
	private bool FlagPaddingZeroPrevious;

	private Library_SpriteStudio.Control.ColorBlendOverwrite InstanceColorBlendOverwrite = null;

	private Library_SpriteStudio.Control.CellChange[][][] ListCellTableFont;	/* Cell-Table each Font: [IndexFont] */

	private struct WorkArea_Digit
	{
		public int ID;
		public GameObject InstanceGameObject;

		public void CleanUp()
		{
			ID = -1;
			InstanceGameObject = null;
		}
	}
	private WorkArea_Digit[] WorkAreaPartsDigit = new WorkArea_Digit[(int)Constant.DIGIT_MAX];

	void Start()
	{
		int CountDigit = (int)Constant.DIGIT_MAX;

		/* Initialize WorkArea */
		for(int i=0; i<CountDigit; i++)
		{
			WorkAreaPartsDigit[i].CleanUp();
		}
		ValuePrevious = int.MaxValue;
		FlagProportionalPrevious = FlagProportional;
		FlagPaddingZeroPrevious = FlagPaddingZero;

		/* Initialize Animation-Datas */
		if(null != InstanceRoot)
		{
			/* Start Animation */
			int IndexAnimation = InstanceRoot.IndexGetAnimation("Digit08");
			if(0 <= IndexAnimation)
			{
				InstanceRoot.AnimationPlay(IndexAnimation, 1);
				InstanceRoot.AnimationStop();
			}

			/* Get Digit-Parts */
			Library_SpriteStudio.Control.Parts InstancePartsControl;
			int IDParts;
			for(int i=0; i<CountDigit; i++)
			{
				IDParts = InstanceRoot.IDGetParts(NameParts[i]);
				WorkAreaPartsDigit[i].ID = IDParts;
				if(0 <= IDParts)
				{
					InstancePartsControl = InstanceRoot.ControlGetParts(IDParts);
					if(null != InstancePartsControl)
					{
						WorkAreaPartsDigit[i].InstanceGameObject = InstancePartsControl.InstanceGameObject;
					}
				}
			}

			/* Initialize Material-Table & Cell-Tables for Exchange (to Font-Change) */
			FontInitialize();

			/* Set Font */
			IndexFontPrevious = IndexFont;
			FontSet(IndexFont);

			/* Get Color-Blend WorkArea */
			InstanceColorBlendOverwrite = InstanceRoot.DataGetColorBlendOverwrite();
		}
	}
	
	void Update()
	{
		bool FlagUpdateCell = false;

		/* Update Color */
		if(null != InstanceColorBlendOverwrite)
		{
			InstanceColorBlendOverwrite.SetOverall(KindOperationFontOverlay, ref ColorFontOverlay);
		}

		/* Font Update */
		if(IndexFontPrevious != IndexFont)
		{
			IndexFontPrevious = IndexFont;
			FontSet(IndexFont);
			FlagUpdateCell |= true;
		}
		Library_SpriteStudio.Control.CellChange[][] DataCellMapNow = InstanceRoot.TableCellChange;	/* "TableCellChange" is readonly, principle */
		if(null == DataCellMapNow)
		{	/* Error */
			return;
		}

		/* Clamp Value */
		int ValueDisplay = Mathf.Clamp(Value, ValueMin, ValueMax);

		/* Update Numerical-Displa  */
		FlagUpdateCell |= ((ValuePrevious != ValueDisplay) || (FlagPaddingZeroPrevious != FlagPaddingZero) || (FlagProportionalPrevious != FlagProportional)) ? true : false;
		if(true == FlagUpdateCell)
		{
			/* Update WorkArea */
			ValuePrevious = ValueDisplay;
			FlagProportionalPrevious = FlagProportional;
			FlagPaddingZeroPrevious = FlagPaddingZero;

			/* Generate Text */
			string Text = (true == FlagPaddingZero) ? ValueDisplay.ToString(FormatToString[(int)KindFormat.PADDING_ZERO]) : ValueDisplay.ToString(FormatToString[(int)KindFormat.NORMAL]);
			char[] Characters = Text.ToCharArray();
			int CountCharacters = Characters.Length;

			/* Update Digits */
			int CountDigit = (int)Constant.DIGIT_MAX;
			int IndexCharacter;
			int IDParts;
			Library_SpriteStudio.Data.Cell DataCell;
			int PositionPixelDigit = 0;
			for(int i=0; i<CountDigit; i++)
			{
				IDParts = WorkAreaPartsDigit[i].ID;
				if(0 <= IDParts)
				{	/* Valid */
					/* Hide Digit */
					InstanceRoot.HideSetForce(IDParts, true, false);

					/* Change Cell & Show Digit */
					if(i < CountCharacters)
					{
						/* MEMO: Character-Index is Cell-Index */
						IndexCharacter = IndexGetCharacter(Characters[(CountCharacters - 1) - i]);
						if(0 <= IndexCharacter)
						{
							/* Get Cell-Data */
							DataCell = DataCellMapNow[0][IndexCharacter].DataCell;  /* Same "ListCellTableFont[IndexFont][IndexCharacter].DataCell" */

							/* Change Cell */
							/* MEMO: Ignore Attribute "Cell" */
							/* MEMO: (IndexCellMap == 0) Because Cell-Table has only 1 Cell-Map. */
							InstanceRoot.CellChange(IDParts, 0, IndexCharacter, true);

							/* Show Digit */
							/* MEMO: Don't Effect to children */
							InstanceRoot.HideSetForce(IDParts, false, false);

							/* Get Pixel-Width */
							/* MEMO: Spacing-width = (Previous digit's width + Now digit's width) / 2 */
							int PixelSpaceNow = ((null != DataCell) && (true == FlagProportional)) ? ((int)DataCell.Rectangle.width) : SizePixelFont;
							PixelSpaceNow /= 2;

							/* Adjust Position */
							/* MEMO: The first digit is Fixed-Position) */
							if(0 < i)
							{
								PositionPixelDigit -= PixelSpaceNow;

								Transform InstanceTransformDigit = WorkAreaPartsDigit[i].InstanceGameObject.transform;
								Vector3 LocalPositionDigit = InstanceTransformDigit.localPosition;
								LocalPositionDigit.x = (float)PositionPixelDigit;
								InstanceTransformDigit.localPosition = LocalPositionDigit;
							}
							PositionPixelDigit -= PixelSpaceNow;
						}
					}
				}
			}
		}
	}

	private bool FontInitialize()
	{	/* MEMO: Solve between the cell (used by each font) and texture. */
		if(null == InstanceRoot)
		{
			return(false);
		}

		/* Create "Material-Table" */
		/* MEMO: Add Texture "Appendix" to Texture (used by "InstanceRoot"). */
		int CountTexture = (int)KindTexture.TERMINATOR;
		Texture2D[] TableTexture = new Texture2D[CountTexture];
		Material DataMaterialOriginal = InstanceRoot.MaterialGet((int)KindTexture.INNER, Library_SpriteStudio.KindColorOperation.MIX);

		TableTexture[(int)KindTexture.INNER] = (null != DataMaterialOriginal) ? ((Texture2D)(DataMaterialOriginal.mainTexture)) : null;
		TableTexture[(int)KindTexture.OUTER] = OutsideSSPJFontTexture;

		InstanceRoot.TableMaterialChange(Library_SpriteStudio.Utility.TableMaterial.Create(TableTexture));

		/* Prepare a table for each font */
		/* MEMO: I'm coding without using "Library_SpriteStudio.Utility.TableCell"-function. */
		/*       (To provide a sample for full-scratch "Cell-Table".)                        */
		int CountFont = (int)Constant.FONT_MAX;
		int CountCharacter = (int)KindCharacter.TERMINATOR;

		ListCellTableFont = new Library_SpriteStudio.Control.CellChange[CountFont][][];
		Library_SpriteStudio.Control.CellChange[][] InstanceCellTableCellMap;
		Library_SpriteStudio.Data.CellMap DataCellMap;
		for(int i=0; i<CountFont; i++)
		{
			/* Create "Cell-Table" for a font. */
			/* MEMO: Set length of array to "1" because original SSAE-data is using only 1 Cell-Map. */
			InstanceCellTableCellMap = new Library_SpriteStudio.Control.CellChange[1][];
			ListCellTableFont[i] = InstanceCellTableCellMap;

			Library_SpriteStudio.Control.CellChange[] InstanceCellTable = new Library_SpriteStudio.Control.CellChange[CountCharacter];
			InstanceCellTableCellMap[0] = InstanceCellTable;

			/* Get Cell-Map (in Animation-Data) */
			int IndexTexture = IndexTextures[i];

			DataCellMap = null;	/* Value "Error" */
			if((int)Constant.FONT_INSIDEPROJECT_MAX > i)
			{	/* Default(inside Animation-Data) Cell-Map */
				if(null != InstanceRoot.DataCellMap)
				{
					DataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(0);
				}
			}
			else
			{	/* Appendix(Outside Animation-Data) Cell-Map */
				if(null != OutsideSSPJFontCellMap)
				{
					DataCellMap = OutsideSSPJFontCellMap.DataGetCellMap(0);
				}
			}

			/* Set Cell-Data (for the number of characters which is being used) */
			if(null != DataCellMap)
			{
				int IndexCell;
				Library_SpriteStudio.Data.Cell DataCell;
				for(int j=0; j<CountCharacter; j++)
				{
					/* Get original Cell-Data */
					IndexCell = DataCellMap.IndexGetCell(NameCells[i][j]);
					if(0 <= IndexCell)
					{	/* Valid */
						DataCell = DataCellMap.DataGetCell(IndexCell);
					}
					else
					{	/* Not Found */
						DataCell = null;
					}

					/* Set Cell-Data */
					if(null != DataCell)
					{	/* Valid */
						InstanceCellTable[j].DataSet(IndexTexture, DataCellMap, DataCell);
					}
					else
					{	/* Invalid */
						InstanceCellTable[j].CleanUp();
					}
				}
			}
			else
			{	/* Error (Cell-Map is not Found) */
				for(int j=0; j<CountCharacter; j++)
				{
					/* Set "Invalid" to Cell */
					InstanceCellTable[j].CleanUp();
				}
			}
		}

		return(true);
	}

	private bool FontSet(int IndexKindFont)
	{
		if(null == InstanceRoot)
		{
			return(false);
		}

		int Index = ((0 > IndexKindFont) || ((int)Constant.FONT_MAX <= IndexKindFont)) ? 0 : IndexKindFont;
		InstanceRoot.CellMapChange(ListCellTableFont[Index]);	/* Range: no-Instance, no- Effect / Material-Table: no-Change(Instance/Effect) */

		return(true);
	}

	private int IndexGetCharacter(char Character)
	{
		int Count = (int)KindCharacter.TERMINATOR;
		for(int i=0; i<Count; i++)
		{
			if(ListCharacters[i] == Character)
			{
				return(i);
			}
		}

		return(-1);
	}
}
