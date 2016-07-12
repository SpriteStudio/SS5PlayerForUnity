/*
Copyright (c) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura.

This source-list (C# porting and customizing) is coded by
                                  Web Technology Corp. 2015 November 2, 2015.


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:



The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.



THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;

public static partial class Library_SpriteStudio
{
	public static partial class Utility
	{
		public static partial class Random
		{
			public class MersenneTwister : Library_SpriteStudio.Utility.Random.Generator
			{
				/* Seed for Emitter */
				private static readonly uint[] ListSeedEffect = new uint[]
				{
					485,	583,	814,	907,	1311,	1901,	2236,	3051,	3676,	4338,
					4671,	4775,	4928,	4960,	5228,	5591,	5755,	5825,	5885,	5967,	6014,	6056,
					6399,	6938,	7553,	8280,	8510,	8641,	8893,	9043,	31043,	31043,
				};

				private enum Constants : ulong
				{
					iN = 624UL,						/* int */
					iM = 397UL,						/* int */
					ulMATRIX_A = 0x9908b0dfUL,
					ulUPPER_MASK = 0x80000000UL,
					ulLOWER_MASK = 0x7fffffffUL,
				}
				private ulong[] m_aulMT;
				private int m_iMTI;

				public MersenneTwister()
				{
					m_aulMT = new ulong[(int)Constants.iN];
					m_iMTI = (int)Constants.iN + 1;	/* mti==N+1 means mt[N] is not initialized */
					ulong[] auInit =
					{
						0x123UL,
						0x234UL,
						0x345UL,
						0x456UL
					};
					init_by_array(auInit, auInit.Length);	/* use default seed */
				}

				/* initializes mt[N] with a seed */
				public void init_genrand(ulong ulSeed)
				{
					m_aulMT[0] = ulSeed & 0xffffffffUL;
					{
						for(m_iMTI=1; m_iMTI<(int)Constants.iN; m_iMTI++)
						{
							m_aulMT[m_iMTI] = (uint)((1812433253UL * (m_aulMT[m_iMTI - 1] ^ (m_aulMT[m_iMTI - 1] >> 30)) + (ulong)m_iMTI));
							/* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
							/* In the previous versions, MSBs of the seed affect   */
							/* only MSBs of the array mt[].                        */
							/* 2002/01/09 modified by Makoto Matsumoto             */
							m_aulMT[m_iMTI] &= 0xffffffffUL;	/* for >32 bit machines */
						}
					}
				}

				/* initialize by an array with array-length    */
				/* init_key is the array for initializing keys */
				/* key_length is its length                    */
				public void init_by_array(ulong[] aulInitKey, int iKeyLength)
				{
					init_genrand(19650218UL);
					int iI = 1;
					int iJ = 0;
					{
						for(int k=(((int)Constants.iN > iKeyLength) ? (int)Constants.iN : iKeyLength); k>0; k--)
						{
							m_aulMT[iI] = ((m_aulMT[iI] ^ ((m_aulMT[iI - 1] ^ (m_aulMT[iI - 1] >> 30)) * 1664525UL)) + aulInitKey[iJ] + (ulong)iJ);	/* non linear */
							m_aulMT[iI] &= 0xffffffffUL;	/* for WORDSIZE > 32 machines */
							iI++;
							iJ++;
							if(iI >= (int)Constants.iN)
							{
								m_aulMT[0] = m_aulMT[(int)Constants.iN - 1];
								iI = 1;
							}
							if(iJ >= iKeyLength)
							{
								iJ = 0;
							}
						}
					}
					{
						for(int k=(int)Constants.iN - 1; k>0; k--)
						{
							m_aulMT[iI] = (uint)((m_aulMT[iI] ^ ((m_aulMT[iI - 1] ^ (m_aulMT[iI - 1] >> 30)) * 1566083941UL)) - (ulong)iI);	/* non linear */
							m_aulMT[iI] &= 0xffffffffUL;	/* for WORDSIZE > 32 machines */
							iI++;
							if(iI >= (int)Constants.iN)
							{
								m_aulMT[0] = m_aulMT[(int)Constants.iN - 1];
								iI = 1;
							}
						}
					}
					m_aulMT[0] = 0x80000000; /* MSB is 1; assuring non-zero initial array */
				}

				/* generates a random number on [0,0xffffffff]-interval */
				public uint genrand_uint32()
				{
					ulong[] aulMag01 =
					{
						0,
						(ulong)Constants.ulMATRIX_A
					};
					ulong ulY = 0;

					/* mag01[x] = x * MATRIX_A for x=0,1 */
					if(m_iMTI >= (int)Constants.iN)
					{
						/* generate N words at one time */
						if (m_iMTI == ((int)Constants.iN + 1))
						{
							/* if init_genrand() has not been called, */
							init_genrand(5489UL);	/* a default initial seed is used */
						}
						int iKK = 0;
						{
							for(; iKK<((int)Constants.iN - (int)Constants.iM); iKK++)
							{
								ulY = (m_aulMT[iKK] & (ulong)Constants.ulUPPER_MASK) | (m_aulMT[iKK + 1] & (ulong)Constants.ulLOWER_MASK);
								m_aulMT[iKK] = m_aulMT[iKK + (int)Constants.iM] ^ (ulY >> 1) ^ aulMag01[ulY & 1];
							}
						}
						{
							for (; iKK <((int)Constants.iN - 1); iKK++)
							{
								ulY = (m_aulMT[iKK] & (ulong)Constants.ulUPPER_MASK) | (m_aulMT[iKK + 1] & (ulong)Constants.ulLOWER_MASK);
								m_aulMT[iKK] = m_aulMT[iKK + ((int)Constants.iM - (int)Constants.iN)] ^ (ulY >> 1) ^ aulMag01[ulY & 0x1UL];
							}
						}

						ulY = (m_aulMT[(int)Constants.iN - 1] & (ulong)Constants.ulUPPER_MASK) | (m_aulMT[0] & (ulong)Constants.ulLOWER_MASK);
						m_aulMT[(int)Constants.iN - 1] = m_aulMT[(int)Constants.iM - 1] ^ (ulY >> 1) ^ aulMag01[ulY & 1];
						m_iMTI = 0;
					}

					ulY = m_aulMT[m_iMTI++];
					/* Tempering */
					ulY ^= (ulY >> 11);
					ulY ^= (ulY << 7) & 0x9d2c5680UL;
					ulY ^= (ulY << 15) & 0xefc60000UL;
					ulY ^= (ulY >> 18);
					return((uint)(ulY & 0xffffffffUL));
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real1()
				{
					return genrand_uint32() * (1.0 / 4294967295.0);	/* divided by 2^32-1 */
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real2()
				{
					return genrand_uint32() * (1.0 / 4294967296.0);	/* divided by 2^32 */
				}

				/* generates a random integer number from 0 to N-1 */
				public int genrand_N(int iN)
				{
					return (int)(genrand_uint32() * (iN / 4294967296.0));
				}

				/* Interfaces */
				public uint[] ListSeed
				{
					get
					{
						return(ListSeedEffect);
					}
				}
				public void InitSeed(uint Seed)
				{
					if(0xffffffff != Seed)
					{	/* Seed Set */
						uint[] TableSeed = ListSeed;
						if(Seed < TableSeed.Length)
						{
							init_genrand(TableSeed[Seed]);
						}
						else
						{
							init_genrand(Seed);
						}
					}
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
					return((float)(genrand_uint32() * (Limit / 4294967296.0f)));
				}
				public int RandomN(int Limit)
				{
					return(genrand_N(Limit));
				}
			}
		}
	}
}
