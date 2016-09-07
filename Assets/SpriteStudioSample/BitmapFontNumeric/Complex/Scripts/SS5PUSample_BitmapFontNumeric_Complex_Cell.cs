/**
	SpriteStudio5 Player for Unity
	Sample : Numeric Bitmap-Font (Font Managed Cell Changing)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class SS5PUSample_BitmapFontNumeric_Complex_Cell : MonoBehaviour
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

	/* [Constant] Animation-Object's Datas */
	private enum Constant
	{
		FONT_MAX = 4,
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
	private enum KindCharacter
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

	private struct WorkArea_FontCell
	{
		public int IndexCell;
		public Library_SpriteStudio.Data.Cell InstanceDataCell;
	}
	private WorkArea_FontCell[] WorkAreaFontCell = new WorkArea_FontCell[(int)KindCharacter.TERMINATOR];

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
			int IndexCell;
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
						/* Get Cell-Data */
						IndexCharacter = IndexGetCharacter(Characters[(CountCharacters - 1) - i]);
						if(0 <= IndexCharacter)
						{
							IndexCell = WorkAreaFontCell[IndexCharacter].IndexCell;
							if(0 <= IndexCell)
							{
								/* Change Cell */
								/* MEMO: Ignore Attribute "Cell" */
								/* MEMO: (IndexCellMap == 0) Because this Animation has 1 Texture. */
								InstanceRoot.CellChange(IDParts, 0, IndexCell, true);

								/* Show Digit */
								/* MEMO: Don't Effect to children */
								InstanceRoot.HideSetForce(IDParts, false, false);

								/* Get Pixel-Width */
								/* MEMO: Spacing-width = (Previous digit's width + Now digit's width) / 2 */
								int PixelSpaceNow = (true == FlagProportional) ? ((int)(WorkAreaFontCell[IndexCharacter].InstanceDataCell.Rectangle.width)) : SizePixelFont;
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
	}

	private bool FontSet(int IndexKindFont)
	{
		if(null == InstanceRoot)
		{
			return(false);
		}

		int Index = ((0 > IndexKindFont) || ((int)Constant.FONT_MAX <= IndexKindFont)) ? 0 : IndexKindFont;
		int Count = (int)KindCharacter.TERMINATOR;
		Library_SpriteStudio.Data.CellMap InstanceDataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(0);
		if(null == InstanceDataCellMap)
		{
			return(false);
		}

		string[] ListNameCell = NameCells[Index];
		int IndexCell;
		for (int i=0; i<Count; i++)
		{
			IndexCell = InstanceDataCellMap.IndexGetCell(ListNameCell[i]);
			if(0 > IndexCell)
			{	/* Error */
				WorkAreaFontCell[i].IndexCell = -1;
				WorkAreaFontCell[i].InstanceDataCell = null;
				continue;
			}

			WorkAreaFontCell[i].IndexCell = IndexCell;
			WorkAreaFontCell[i].InstanceDataCell = InstanceDataCellMap.DataGetCell(IndexCell);
		}

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
