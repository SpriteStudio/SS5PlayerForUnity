using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Library_SpriteStudio_Flyweight
{
	[System.Flags]
	public enum FlagBit : int
	{
		FRAMENO = 0x00007fff,
		INDEX = 0x3fff8000,

		CLEAR = 0x00000000,
	}

	public abstract class FlyweightList<_TypeValue>
	{
		[SerializeField]
		FlagBit[] ListStatus;

		abstract public _TypeValue GetValue(Flyweight flyweight, int index);
		abstract protected int Add(Flyweight flyweight, _TypeValue value);

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

		public bool TryGetValue(out _TypeValue value, int FrameNo, Flyweight flyweight)
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

		public void Build(Flyweight flyweight, _TypeValue[] rawValues)
		{   /* MEMO: CPE(Changing-Point Extracting) */

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

	[System.Serializable]
	public class ListAttributeVector2 : FlyweightList<Vector2>
	{
		override public Vector2 GetValue(Flyweight flyweight, int index)
		{
			return flyweight.Vector2s[index];
		}
		protected override int Add(Flyweight flyweight, Vector2 value)
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
	public class ListAttributeCoordinateMeshFix : FlyweightList<Library_SpriteStudio.Data.AttributeCoordinateMeshFix>
	{
		override public Library_SpriteStudio.Data.AttributeCoordinateMeshFix GetValue(Flyweight flyweight, int index)
		{
			return flyweight.CoordinateMeshFixes[index];
		}
		protected override int Add(Flyweight flyweight, Library_SpriteStudio.Data.AttributeCoordinateMeshFix value)
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
	public class ListAttributeColorBlendMeshFix : FlyweightList<Library_SpriteStudio.Data.AttributeColorBlendMeshFix>
	{
		override public Library_SpriteStudio.Data.AttributeColorBlendMeshFix GetValue(Flyweight flyweight, int index)
		{
			return flyweight.ColorMeshFixes[index];
		}
		protected override int Add(Flyweight flyweight, Library_SpriteStudio.Data.AttributeColorBlendMeshFix value)
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
	public class ListAttributeUVMeshFix : FlyweightList<Library_SpriteStudio.Data.AttributeUVMeshFix>
	{
		override public Library_SpriteStudio.Data.AttributeUVMeshFix GetValue(Flyweight flyweight, int index)
		{
			return flyweight.UVMeshFixes[index];
		}
		protected override int Add(Flyweight flyweight, Library_SpriteStudio.Data.AttributeUVMeshFix value)
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
