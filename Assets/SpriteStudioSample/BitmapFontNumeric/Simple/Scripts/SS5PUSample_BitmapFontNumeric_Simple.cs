/**
	SpriteStudio5 Player for Unity
	Sample : Numeric Bitmap-Font (Simple)

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

public class SS5PUSample_BitmapFontNumeric_Simple : MonoBehaviour
{
	/* Target Animation-Object */
	public Script_SpriteStudio_Root InstanceRoot;

	/* Value */
	public int Value;

	/* Form */
	public bool FlagPaddingZero;

	/* [Constant] Animation-Object's Datas */
	private enum Constant
	{
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

	private static readonly string[] NameCells = new string[(int)KindCharacter.TERMINATOR]
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
	};

	private static readonly string[] FormatToString = new string[(int)KindFormat.TERMINATOR]
	{
		"D",
		"D" + ((int)(Constant.DIGIT_MAX)).ToString(),
	};

	/* WorkArea */
	private int ValuePrevious;
	private bool FlagPaddingZeroPrevious;
	private int[] IndexCell = new int[(int)KindCharacter.TERMINATOR];
	private int[] IDPartsDigit = new int[(int)Constant.DIGIT_MAX];

	void Start()
	{
		/* Initialize WorkArea */
		ValuePrevious = int.MaxValue;
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
			for(int i=0; i<(int)Constant.DIGIT_MAX; i++)
			{
				IDPartsDigit[i] = InstanceRoot.IDGetParts(NameParts[i]);
			}

			/* Get Characters' Cell Index */
			Library_SpriteStudio.Data.CellMap InstanceDataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(0);
			for (int i=0; i<(int)KindCharacter.TERMINATOR; i++)
			{
				IndexCell[i] = InstanceDataCellMap.IndexGetCell(NameCells[i]);
			}
		}
	}
	
	void Update()
	{
		/* Clamp Value */
		int ValueDisplay = Mathf.Clamp(Value, ValueMin, ValueMax);

		/* Update Numerical-Display */
		if((ValuePrevious != ValueDisplay) || (FlagPaddingZeroPrevious != FlagPaddingZero))
		{
			/* Update WorkArea */
			ValuePrevious = ValueDisplay;
			FlagPaddingZeroPrevious = FlagPaddingZero;

			/* Generate Text */
			string Text = (true == FlagPaddingZero) ? ValueDisplay.ToString(FormatToString[(int)KindFormat.PADDING_ZERO]) : ValueDisplay.ToString(FormatToString[(int)KindFormat.NORMAL]);
			char[] Characters = Text.ToCharArray();
			int CountCharacters = Characters.Length;

			/* Update Digits */
			int IndexCharacter;
			int IDParts;
			for(int i=0; i<(int)Constant.DIGIT_MAX; i++)
			{
				IDParts = IDPartsDigit[i];
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
							/* Change Cell */
							/* MEMO: Ignore Attribute "Cell" */
							/* MEMO: (IndexCellMap == 0) Because this Animation has 1 Texture. */
							InstanceRoot.CellChange(IDParts, 0, IndexCell[IndexCharacter], true);
							
							/* Show Digit */
							/* MEMO: Don't Effect to children */
							InstanceRoot.HideSetForce(IDParts, false, false);
						}
					}
				}
			}
		}
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
