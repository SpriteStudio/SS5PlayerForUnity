/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
// #define CALCULATE_ULONG

using UnityEngine;

public static partial class Library_SpriteStudio
{
	public static partial class Utility
	{
		public static partial class Random
		{
			public class XorShift32 : Library_SpriteStudio.Utility.Random.Generator
			{
#if CALCULATE_ULONG
				protected ulong SeedNow;
#else
				protected uint SeedNow;
#endif

				public XorShift32()
				{
#if CALCULATE_ULONG
					SeedNow = 0L;
#else
					SeedNow = 0;
#endif
				}

				public void init_genrand(uint Seed)
				{
#if CALCULATE_ULONG
					SeedNow = (ulong)Seed;
#else
					SeedNow = Seed;
#endif
				}

				public uint genrand_uint32()
				{
#if CALCULATE_ULONG
					SeedNow = (SeedNow ^ (SeedNow << 13)) & 0x00000000ffffffffUL;
					SeedNow = (SeedNow ^ (SeedNow >> 17)) & 0x00000000ffffffffUL;
					SeedNow = (SeedNow ^ (SeedNow << 15)) & 0x00000000ffffffffUL;
					return((uint)(SeedNow & 0x00000000ffffffffUL));
#else
					SeedNow = SeedNow ^ (SeedNow << 13);
					SeedNow = SeedNow ^ (SeedNow >> 17);
					SeedNow = SeedNow ^ (SeedNow << 15);
					return(SeedNow);
#endif
				}

				public float genrand_float32()
				{
					return((float)((genrand_uint32() >> 9) & 0x007fffff) * (1.0f / 8388607.0f));
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real1()
				{
					return(genrand_uint32() * (1.0 / 4294967295.0));	/* divided by 2^32-1 */
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real2()
				{
					return(genrand_uint32() * (1.0 / 4294967296.0));	/* divided by 2^32 */
				}

				/* generates a random integer number from 0 to N-1 */
				public int genrand_N(int iN)
				{
					return((int)(genrand_uint32() * (iN / 4294967296.0)));
				}

				/* Interfaces */
				public uint[] ListSeed
				{
					get
					{
						return(null);
					}
				}
				public void InitSeed(uint Seed)
				{
					init_genrand(Seed);
				}
				public uint RandomUint32()
				{
					return(genrand_uint32());
				}
				public double RandomDouble(double Limit)
				{
					return((genrand_uint32() * (Limit / 4294967296.0)));
				}
				public float RandomFloat(float Limit)
				{
					return((float)(genrand_uint32() * (Limit / 4294967296.0f)));	/* 8388607.0f */
				}
				public int RandomN(int Limit)
				{
					return(genrand_N(Limit));
				}
			}
		}
	}
}
