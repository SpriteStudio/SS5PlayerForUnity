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
			public static partial class Flyweight
			{
				[System.Serializable]
				public class Factory
				{
					[SerializeField]
					List<Vector2> vector2s = new List<Vector2>();
					public List<Vector2> Vector2s
					{
						get
						{
							return vector2s;
						}
					}

					[SerializeField]
					List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix> coordinateMeshFixes = new List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix>();
					public List<Library_SpriteStudio.Data.AttributeCoordinateMeshFix> CoordinateMeshFixes
					{
						get
						{
							return coordinateMeshFixes;
						}
					}

					[SerializeField]
					List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix> colorMeshFixes = new List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix>();
					public List<Library_SpriteStudio.Data.AttributeColorBlendMeshFix> ColorMeshFixes
					{
						get
						{
							return colorMeshFixes;
						}
					}

					[SerializeField]
					List<Library_SpriteStudio.Data.AttributeUVMeshFix> uvMeshFixes = new List<Library_SpriteStudio.Data.AttributeUVMeshFix>();
					public List<Library_SpriteStudio.Data.AttributeUVMeshFix> UVMeshFixes
					{
						get
						{
							return uvMeshFixes;
						}
					}
				}

				public abstract class ListAttribute_Base<_TypeValue>
				{
					[System.Flags]
					public enum FlagBit : int
					{
						FRAMENO = 0x00007fff,
						INDEX = 0x3fff8000,

						CLEAR = 0x00000000,
					}

					[SerializeField]
					FlagBit[] ListStatus;

					abstract public _TypeValue GetValue(Factory flyweight, int index);
					abstract protected int Add(Factory flyweight, _TypeValue value);

					public Library_SpriteStudio.KindPack KindPack
					{
						get
						{
							return(Library_SpriteStudio.KindPack.FLYWEIGHT);
						}
					}

					public bool IsValid
					{
						get
						{
							return (ListStatus != null && ListStatus.Length > 0);
						}
					}

					static FlagBit GetFlagBit(int FrameNo, int IndexData)
					{
						FlagBit FlagBitNow = ((FlagBit)FrameNo) & FlagBit.FRAMENO;
						FlagBitNow |= ((FlagBit)(IndexData << 15)) & FlagBit.INDEX;
						return (FlagBitNow);
					}

					public bool TryGetValue(out _TypeValue value, int FrameNo, Factory flyweight)
					{
						int FrameNoOrigin;
						int index = IndexGetValue(out FrameNoOrigin, FrameNo);
						if (index < 0)
						{
							value = default(_TypeValue);
							return false;
						}
						value = GetValue(flyweight, index);
						return true;
					}

					public int IndexGetValue(out int FrameNoOrigin, int FrameNo)
					{
						int Index = -1;
						int FrameNoKey = -1;

						if (ListStatus == null || 0 >= ListStatus.Length)
						{   /* MEMO: Uncompressed or No-Data */
							goto IndexGetValue_End;
						}

						FlagBit StatusTemp;
						int Min = 0;
						int Max = ListStatus.Length - 1;
						int Middle;
						int Range;
						while (Min != Max)
						{
							Range = Min + Max;
							Middle = (Range / 2) + (Range % 2);
							StatusTemp = ListStatus[Middle] & FlagBit.FRAMENO;
							FrameNoKey = (FlagBit.FRAMENO == StatusTemp) ? -1 : (int)StatusTemp;
							if (FrameNo == FrameNoKey)
							{
								Min = Middle;
								Max = Middle;
							}
							else
							{
								if ((FrameNo < FrameNoKey) || (-1 == FrameNoKey))
								{
									Max = Middle - 1;
								}
								else
								{
									Min = Middle;
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
						return (Index);
					}

					public void Build(Factory flyweight, _TypeValue[] rawValues)
					{
						if (rawValues == null || rawValues.Length == 0)
						{   /* No-Data */
							ListStatus = new FlagBit[0];
							return;
						}

						/* WorkArea Boot-Up */
						List<int> ArrayFrameNo = new List<int>();
						List<int> ArrayIndex = new List<int>();

						/* Extructing Changing Point */
						for (int i = 0; i < rawValues.Length; i++)
						{
							if (i > 0
								&& rawValues[i].Equals(rawValues[i - 1]))
							{   /* Unchanging */
								continue;
							}

							var IndexExist = Add(flyweight, rawValues[i]);
							ArrayFrameNo.Add(i);
							ArrayIndex.Add(IndexExist);
						}

						ListStatus = new FlagBit[ArrayIndex.Count];
						for (int i = 0; i < ListStatus.Length; i++)
						{
							ListStatus[i] = GetFlagBit((int)ArrayFrameNo[i], (int)ArrayIndex[i]);
						}

						return;
					}
				}

				/* ------------------------------------------------------------ Attribute-List */
				[System.Serializable]
				public class ListAttributeVector2 : ListAttribute_Base<Vector2>
				{
					override public Vector2 GetValue(Factory flyweight, int index)
					{
						return flyweight.Vector2s[index];
					}
					protected override int Add(Factory flyweight, Vector2 value)
					{
						int i = flyweight.Vector2s.IndexOf(value);
						if (i >= 0)
						{
							return i;
						}
						else
						{
							flyweight.Vector2s.Add(value);
							return flyweight.Vector2s.Count - 1;
						}
					}
				}

				[System.Serializable]
				public class ListAttributeCoordinateMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeCoordinateMeshFix>
				{
					override public Library_SpriteStudio.Data.AttributeCoordinateMeshFix GetValue(Factory flyweight, int index)
					{
						return flyweight.CoordinateMeshFixes[index];
					}
					protected override int Add(Factory flyweight, Library_SpriteStudio.Data.AttributeCoordinateMeshFix value)
					{
						int i = flyweight.CoordinateMeshFixes.FindIndex(_ => _.Equals(value));
						if (i >= 0)
						{
							return i;
						}
						else
						{
							flyweight.CoordinateMeshFixes.Add(value);
							return flyweight.CoordinateMeshFixes.Count - 1;
						}
					}
				}

				[System.Serializable]
				public class ListAttributeColorBlendMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeColorBlendMeshFix>
				{
					override public Library_SpriteStudio.Data.AttributeColorBlendMeshFix GetValue(Factory flyweight, int index)
					{
						return flyweight.ColorMeshFixes[index];
					}
					protected override int Add(Factory flyweight, Library_SpriteStudio.Data.AttributeColorBlendMeshFix value)
					{
						int i = flyweight.ColorMeshFixes.FindIndex(_ => _.Equals(value));
						if (i >= 0)
						{
							return i;
						}
						else
						{
							flyweight.ColorMeshFixes.Add(value);
							return flyweight.ColorMeshFixes.Count - 1;
						}
					}
				}

				[System.Serializable]
				public class ListAttributeUVMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeUVMeshFix>
				{
					override public Library_SpriteStudio.Data.AttributeUVMeshFix GetValue(Factory flyweight, int index)
					{
						return flyweight.UVMeshFixes[index];
					}
					protected override int Add(Factory flyweight, Library_SpriteStudio.Data.AttributeUVMeshFix value)
					{
						int i = flyweight.UVMeshFixes.FindIndex(_ => _.Equals(value));
						if (i >= 0)
						{
							return i;
						}
						else
						{
							flyweight.UVMeshFixes.Add(value);
							return flyweight.UVMeshFixes.Count - 1;
						}
					}
				}
			}
		}
	}
}
