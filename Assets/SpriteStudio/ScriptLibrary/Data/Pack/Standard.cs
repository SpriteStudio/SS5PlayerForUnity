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
	public static partial class Data
	{
		public static partial class Pack
		{
			public static partial class Standard
			{
				public class ListAttribute_Base<_TypeValue>
				{
					[System.Flags]
					public enum FlagBit : int
					{
						FRAMENO = 0x00007fff,
						INDEX = 0x3fff8000,

						CLEAR = 0x00000000,
					}

					public FlagBit[] ListStatus;
					public _TypeValue[] ListValue;

					public Library_SpriteStudio.KindPack KindPack
					{
						get
						{
							return((0 >= ListStatus.Length) ? Library_SpriteStudio.KindPack.STANDARD_UNCOMPRESSED : Library_SpriteStudio.KindPack.STANDARD_CPE);
						}
					}

					public bool IsValid
					{
						get
						{
							return(0 < ListValue.Length);
						}
					}

					public void CleanUp()
					{
						ListStatus = null;
						ListValue = null;
					}

					public void BootUp(int CountFrame)
					{	/* MEMO: FrameCount==0 ... No-Data *//* MEMO: Always Uncompressed */
						ListStatus = new FlagBit[0];
						ListValue = new _TypeValue[CountFrame];
					}

					public void BootUpCompressedForce(int CountData)
					{	/* MEMO: Always Uncompressed */
						ListStatus = new FlagBit[CountData];
						ListValue = new _TypeValue[CountData];
						for(int i=0; i<CountData; i++)
						{
							ListStatus[i] = FlagBit.INDEX;	/* .CLEAR; */	/* Invalid Data */
						}
					}
					public static FlagBit GetFlagBit(int FrameNo, int IndexData)
					{
						FlagBit FlagBitNow = ((FlagBit)FrameNo) & FlagBit.FRAMENO;
						FlagBitNow |= ((FlagBit)(IndexData << 15)) & FlagBit.INDEX;
						return(FlagBitNow);
					}

					public int IndexGetForce(out int FrameNo, int IndexStatus)
					{
						FlagBit StatusTemp = ListStatus[IndexStatus];
						FrameNo = (int)(StatusTemp & FlagBit.FRAMENO);
						StatusTemp &= FlagBit.INDEX;
						return((FlagBit.INDEX == StatusTemp) ? -1 : ((int)StatusTemp >> 15));
					}

					public int IndexGetValue(out int FrameNoOrigin, int FrameNo)
					{
						int Index = -1;
						int FrameNoKey = -1;

						if(0 >= ListStatus.Length)
						{	/* MEMO: Uncompressed or No-Data */
							if(0 >= ListValue.Length)
							{	/* MEMO: No-Data */
								goto IndexGetValue_End;
							}

							FrameNoKey = FrameNo;
							Index = FrameNo;
							goto IndexGetValue_End;
						}

						FlagBit StatusTemp;
						int Min = 0;
						int Max = ListStatus.Length - 1;
						int Medium;
						int Range;
						while(Min != Max)
						{
							Range = Min + Max;
							Medium = (Range / 2) + (Range % 2);
							StatusTemp = ListStatus[Medium] & FlagBit.FRAMENO;
							FrameNoKey = (FlagBit.FRAMENO == StatusTemp) ? -1 : (int)StatusTemp;
							if(FrameNo == FrameNoKey)
							{
								Min = Medium;
								Max = Medium;
							}
							else
							{
								if((FrameNo < FrameNoKey) || (-1 == FrameNoKey))
								{
									Max = Medium - 1;
								}
								else
								{
									Min = Medium;
								}
							}
						}
					
						StatusTemp = ListStatus[Min];
						FlagBit ValueTemp = StatusTemp & FlagBit.FRAMENO;
						FrameNoKey = (FlagBit.FRAMENO == ValueTemp) ? -1 : (int)ValueTemp;
						ValueTemp = StatusTemp & FlagBit.INDEX;
						Index = (FlagBit.INDEX == ValueTemp) ? -1 : ((int)ValueTemp >> 15);

					IndexGetValue_End:;
						FrameNoOrigin = FrameNoKey;
						return(Index);
					}

					public bool CompressCPE(int CountFrame)
					{	/* MEMO: CPE(Changing-Point Extracting) */
						if(0 < ListStatus.Length)
						{	/* Already Compressed */
							return(true);
						}

						if(0 >= ListValue.Length)
						{	/* No-Data */
							return(true);
						}

						/* WorkArea Boot-Up */
						List<int> ArrayFrameNo = new List<int>();
						List<int> ArrayIndex = new List<int>();
						List<_TypeValue> ArrayValue = new List<_TypeValue>();
						ArrayFrameNo.Clear();
						ArrayIndex.Clear();
						ArrayValue.Clear();

						/* Top-Data Store-Force */
						ArrayFrameNo.Add(0);
						ArrayIndex.Add(0);
						ArrayValue.Add(ListValue[0]);

						/* Extructing Changing Point */
						int Count = ListValue.Length;
						int CountArray = -1;
						int IndexExist = -1;
						for(int i=1; i<Count; i++)
						{
							if(true == ListValue[i].Equals(ListValue[i - 1]))
							{	/* Unchanging */
								continue;
							}

							CountArray = ArrayValue.Count;
							IndexExist = -1;
							for(int j=0; j<CountArray; j++)
							{
								if(true == ListValue[i].Equals(ArrayValue[j]))
								{
									IndexExist = j;
									break;
								}
							}
							if(-1 == IndexExist)
							{	/* Data-New */
								ArrayValue.Add(ListValue[i]);
								ArrayFrameNo.Add(i);
								ArrayIndex.Add(ArrayValue.Count - 1);
							}
							else
							{	/* Data-Exist */
								ArrayFrameNo.Add(i);
								ArrayIndex.Add(IndexExist);
							}
						}

						/* Rebuilding Array */
						CountArray = ArrayFrameNo.Count;
						if(CountArray >= Count)
						{	/* All value is Changing-Point ... Uncompress */
							ListStatus = new FlagBit[0];
							return(true);
						}
						ListStatus = new FlagBit[CountArray];
						for(int i=0; i<CountArray; i++)
						{
							ListStatus[i] = GetFlagBit((int)ArrayFrameNo[i], (int)ArrayIndex[i]);
						}
						ListValue = ArrayValue.ToArray();

						return(true);
					}

					public bool DecompressCPE(int CountFrame)
					{	/* MEMO: CPE(Changing-Point Extracting) */
						if(0 >= ListStatus.Length)
						{	/* Already Decompressed */
							return(true);
						}

						if(0 >= ListValue.Length)
						{	/* No Data */
							return(true);
						}

						_TypeValue[] ListValueNew = new _TypeValue[CountFrame];
						int Index;
						int FrameNoOriginDummy;
						for(int i=0; i<CountFrame; i++)
						{
							Index = IndexGetValue(out FrameNoOriginDummy, i);
							ListValueNew[i] = ListValue[Index];
						}
						ListStatus = new FlagBit[0];
						ListValue = ListValueNew;
						return(true);
					}
				}

				/* ------------------------------------------------------------ Attribute-List */
				[System.Serializable]
				public class ListAttributeInt : ListAttribute_Base<int>
				{
				}
				[System.Serializable]
				public class ListAttributeFloat : ListAttribute_Base<float>
				{
				}
				[System.Serializable]
				public class ListAttributeVector2 : ListAttribute_Base<Vector2>
				{
				}
				[System.Serializable]
				public class ListAttributeVector3 : ListAttribute_Base<Vector3>
				{
				}

				[System.Serializable]
				public class ListAttributeStatus : ListAttribute_Base<Library_SpriteStudio.Data.AttributeStatus>
				{
				}
				[System.Serializable]
				public class ListAttributeColorBlend : ListAttribute_Base<Library_SpriteStudio.Data.AttributeColorBlend>
				{
				}
				[System.Serializable]
				public class ListAttributeVertexCorrection : ListAttribute_Base<Library_SpriteStudio.Data.AttributeVertexCorrection>
				{
				}
				[System.Serializable]
				public class ListAttributeCell : ListAttribute_Base<Library_SpriteStudio.Data.AttributeCell>
				{
				}
				[System.Serializable]
				public class ListAttributeUserData : ListAttribute_Base<Library_SpriteStudio.Data.AttributeUserData>
				{
				}
				[System.Serializable]
				public class ListAttributeInstance : ListAttribute_Base<Library_SpriteStudio.Data.AttributeInstance>
				{
				}
				[System.Serializable]
				public class ListAttributeEffect : ListAttribute_Base<Library_SpriteStudio.Data.AttributeEffect>
				{
				}

				[System.Serializable]
				public class ListAttributeIndexCellMapFix : ListAttribute_Base<int>
				{
				}
			}
		}
	}
}
