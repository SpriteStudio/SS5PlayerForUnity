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
		public static partial class TableCellChange
		{
			/* ******************************************************** */
			//! Create "Cell-Map Change"-Table
			/*!
			@param	InstanceRoot
				Animation(or Effect)-Data
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the initial "Cell-Map" information of the animation data.<br>
			Caution: The result is referencing data in initial "Cell-Map"s.
			*/
			public static Library_SpriteStudio.Control.CellChange[][] Create(Library_SpriteStudio.Script.Root InstanceRoot)
			{
				return((null == InstanceRoot) ? null : Create(InstanceRoot.DataCellMap.ListDataCellMap));
			}

			/* ******************************************************** */
			//! Create "Cell-Map Change"-Table
			/*!
			@param	ListCellMap
				Original "Cell-Map"s
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information.<br>
			Caution: The result is referencing data in original "Cell-Map"s.
			*/
			public static Library_SpriteStudio.Control.CellChange[][] Create(Library_SpriteStudio.Data.CellMap[] ListCellMap)
			{
				Library_SpriteStudio.Control.CellChange[][] Rv = null;
				if(null != ListCellMap)
				{
					int Count = ListCellMap.Length;
					Rv = new Library_SpriteStudio.Control.CellChange[Count][];
					if(null != Rv)
					{
						for(int i=0; i<Count; i++)
						{
							Rv[i] = CellMapCreate(ListCellMap[i], i);
						}
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Create "Cell-Map Change"-Table (1 Cell-Map)
			/*!
			@param	InstanceCellMap
				Original "Cell-Map"
			@param	IndexTexture
				Index of Texture (in the Material-Table)
			@retval	Return-Value
				Created "Cell-Map Change"-Table (1 Cell-Map)<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information.<br>
			Caution: The result is referencing data in original "Cell-Map".
			*/
			public static Library_SpriteStudio.Control.CellChange[] CellMapCreate(Library_SpriteStudio.Data.CellMap InstanceCellMap, int IndexTexture)
			{
				Library_SpriteStudio.Control.CellChange[] Rv = null;
				if(null != InstanceCellMap)
				{
					Library_SpriteStudio.Data.Cell DataCell = null;
					if(null != InstanceCellMap.ListCell)
					{
						int Count = InstanceCellMap.ListCell.Length;
						Rv = new Library_SpriteStudio.Control.CellChange[Count];
						if(null != Rv)
						{
							for(int i=0; i<Count; i++)
							{
								DataCell = InstanceCellMap.ListCell[i];
								if(null == DataCell)
								{	/* Invalid */
									Rv[i].CleanUp();
								}
								else
								{
									Rv[i].DataSet(IndexTexture, InstanceCellMap, DataCell);
								}
							}
						}
					}
					else
					{	/* Has no Cells */
						Rv = new Library_SpriteStudio.Control.CellChange[0];
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Copy "Cell-Map Change"-Table
			/*!
			@param	InstanceRoot
				Animation(or Effect)-Data
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the initial "Cell-Map" information of the animation data.<br>
			Caution: The result is referencing data in initial "Cell-Map". (but "Cell" data is another instance)
			*/
			public static Library_SpriteStudio.Control.CellChange[][] Copy(Library_SpriteStudio.Script.Root InstanceRoot)
			{
				return((null == InstanceRoot) ? null : Copy(InstanceRoot.DataCellMap.ListDataCellMap));
			}

			/* ******************************************************** */
			//! Copy "Cell-Map Change"-Table
			/*!
			@param	ListCellMap
				Original "Cell-Map"s
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information of the animation data.<br>
			Caution: The result is referencing data in original "Cell-Map"s. (but "Cell" data is another instance)
			*/
			public static Library_SpriteStudio.Control.CellChange[][] Copy(Library_SpriteStudio.Data.CellMap[] ListCellMap)
			{
				Library_SpriteStudio.Control.CellChange[][] Rv = null;
				if(null != ListCellMap)
				{
					int Count = ListCellMap.Length;
					Rv = new Library_SpriteStudio.Control.CellChange[Count][];
					if(null != Rv)
					{
						Library_SpriteStudio.Data.CellMap CellMapNew = null;
						if(null != CellMapNew)
						{
							for(int i=0; i<Count; i++)
							{
								CellMapNew = new Library_SpriteStudio.Data.CellMap();
								CellMapNew.Duplicate(ListCellMap[i]);
								Rv[i] = CellMapCopy(CellMapNew, i);

								CellMapNew.ListCell = null;	/* Dis-Use */
							}
						}
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Create "Cell-Map Change"-Table (1 Cell-Map)
			/*!
			@param	InstanceCellMap
				Original "Cell-Map"
			@param	IndexTexture
				Index of Texture (in the Material-Table)
			@retval	Return-Value
				Created "Cell-Map Change"-Table (1 Cell-Map)<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information.<br>
			Caution: The result is referencing data in original "Cell-Map". (but "Cell" data is another instance)
			*/
			public static Library_SpriteStudio.Control.CellChange[] CellMapCopy(Library_SpriteStudio.Data.CellMap InstanceCellMap, int IndexTexture)
			{
				Library_SpriteStudio.Control.CellChange[] Rv = null;
				if(null != InstanceCellMap)
				{
					Library_SpriteStudio.Data.Cell DataCell = null;
					if(null != InstanceCellMap.ListCell)
					{
						Library_SpriteStudio.Data.Cell DataCellNew = null;
						int Count = InstanceCellMap.ListCell.Length;
						Rv = new Library_SpriteStudio.Control.CellChange[Count];
						if(null != Rv)
						{
							for(int i=0; i<Count; i++)
							{
								DataCell = InstanceCellMap.ListCell[i];
								if(null == DataCell)
								{	/* Invalid */
									Rv[i].CleanUp();
								}
								else
								{
									DataCellNew = new Library_SpriteStudio.Data.Cell();
									DataCellNew.Duplicate(DataCell);
									Rv[i].DataSet(IndexTexture, InstanceCellMap, DataCellNew);
								}
							}
						}
					}
					else
					{	/* Has no Cells */
						Rv = new Library_SpriteStudio.Control.CellChange[0];
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Full-Copy "Cell-Map Change"-Table
			/*!
			@param	InstanceRoot
				Animation(or Effect)-Data
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the initial "Cell-Map" information of the animation data.<br>
			Caution: The result is another instance from initial "Cell-Map"s.
			*/
			public static Library_SpriteStudio.Control.CellChange[][] CopyFull(Library_SpriteStudio.Script.Root InstanceRoot)
			{
				return((null == InstanceRoot) ? null : CopyFull(InstanceRoot.DataCellMap.ListDataCellMap));
			}

			/* ******************************************************** */
			//! Full-Copy "Cell-Map Change"-Table
			/*!
			@param	ListCellMap
				Original "Cell-Map"s
			@retval	Return-Value
				Created "Cell-Map Change"-Table<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information of the animation data.<br>
			Caution: The result is another instance from original "Cell-Map"s.
			*/
			public static Library_SpriteStudio.Control.CellChange[][] CopyFull(Library_SpriteStudio.Data.CellMap[] ListCellMap)
			{
				Library_SpriteStudio.Control.CellChange[][] Rv = null;
				if(null != ListCellMap)
				{
					int Count = ListCellMap.Length;
					Rv = new Library_SpriteStudio.Control.CellChange[Count][];
					if(null != Rv)
					{
						for(int i=0; i<Count; i++)
						{
							Rv[i] = CellMapCopyFull(ListCellMap[i], i);
						}
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Full-Copy "Cell-Map Change"-Table (1 Cell-Map)
			/*!
			@param	InstanceCellMap
				Original "Cell-Map"
			@param	IndexTexture
				Index of Texture (in the Material-Table)
			@retval	Return-Value
				Created "Cell-Map Change"-Table (1 Cell-Map)<br>
				null == Failure

			Create the "Cell-Map Change"-Table on the basis of the original "Cell-Map" information.<br>
			Caution: The result is another instance from original "Cell-Map"s.
			*/
			public static Library_SpriteStudio.Control.CellChange[] CellMapCopyFull(Library_SpriteStudio.Data.CellMap InstanceCellMap, int IndexTexture)
			{
				Library_SpriteStudio.Control.CellChange[] Rv = null;
				if(null != InstanceCellMap)
				{
					Library_SpriteStudio.Data.Cell DataCell = null;
					if(null != InstanceCellMap.ListCell)
					{
						Library_SpriteStudio.Data.CellMap DataCellMapNew = null;
						Library_SpriteStudio.Data.Cell DataCellNew = null;
						int Count = InstanceCellMap.ListCell.Length;
						Rv = new Library_SpriteStudio.Control.CellChange[Count];
						if(null != Rv)
						{
							for(int i=0; i<Count; i++)
							{
								DataCell = InstanceCellMap.ListCell[i];
								if(null == DataCell)
								{	/* Invalid */
									Rv[i].CleanUp();
								}
								else
								{
									DataCellMapNew = new Library_SpriteStudio.Data.CellMap();
									DataCellMapNew.Duplicate(InstanceCellMap);
									DataCellMapNew.ListCell = null;	/* Dis-Use */

									DataCellNew = new Library_SpriteStudio.Data.Cell();
									DataCellNew.Duplicate(DataCell);

									Rv[i].DataSet(IndexTexture, DataCellMapNew, DataCellNew);
								}
							}
						}
					}
					else
					{	/* Has no Cells */
						Rv = new Library_SpriteStudio.Control.CellChange[0];
					}
				}
				return(Rv);
			}

			/* ******************************************************** */
			//! Check Indexes
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				Index of "Cell-Map"
			@param	IndexCell
				Index of Cell in "Cell-Map"
			@retval	Return-Value
				true == Valid Indexes<br>
				false == Invalid Indexes

			Check Indexes in "Cell-Map Change"-Table.
			*/
			public static bool TableCheckValidIndex(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
														int IndexCellMap,
														int IndexCell
													)
			{
				if(null == InstanceTableCellChange)
				{
					return(false);
				}
				if((0 > IndexCellMap) || (InstanceTableCellChange.Length <= IndexCellMap))
				{
					return(false);
				}

				if((0 > IndexCell) || (InstanceTableCellChange[IndexCellMap].Length <= IndexCell))
				{
					return(false);
				}

				return(true);
			}

			/* ******************************************************** */
			//! Get Cell's data
			/*!
			@param	Output
				Result storage destination
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				Index of "Cell-Map"
			@param	IndexCell
				Index of Cell in "Cell-Map"
			@retval	Return-Value
				true == "Output" is valid<br>
				false == "Output" is invalid

			Get cell's data in "Cell-Map Change"-Table.
			*/
			public static bool CellGet( ref Library_SpriteStudio.Control.CellChange Output,
										Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
										int IndexCellMap,
										int IndexCell
									)
			{
				if(false == TableCheckValidIndex(InstanceTableCellChange, IndexCellMap, IndexCell))
				{
					return(false);
				}

				Output = InstanceTableCellChange[IndexCellMap][IndexCell];
				return(true);
			}

			/* ******************************************************** */
			//! Set Cell's data
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				Index of "Cell-Map"
			@param	IndexCell
				Index of Cell in "Cell-Map"
			@param	IndexTexture
				Index of Texture (in Material-Table)
			@param	InstanceCellMap
				"Cell-Map" Data
			@param	InstanceCell
				Cell Data
			@retval	Return-Value
				true == "Output" is valid<br>
				false == "Output" is invalid

			Set cell data in "Cell-Map Change"-Table.<br>
			Caution: "InstanceCellMap" and "InstanceCell" are referenced.
			*/
			public static bool CellSet(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
										int IndexCellMap,
										int IndexCell,
										int IndexTexture,
										Library_SpriteStudio.Data.CellMap InstanceCellMap,
										Library_SpriteStudio.Data.Cell InstanceCell
									)
			{
				if(false == TableCheckValidIndex(InstanceTableCellChange, IndexCellMap, IndexCell))
				{
					return(false);
				}

				InstanceTableCellChange[IndexCellMap][IndexCell].DataSet(IndexTexture, InstanceCellMap, InstanceCell);
				return(true);
			}

			/* ******************************************************** */
			//! Set Cell's data
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				Index of "Cell-Map"
			@param	IndexCell
				Index of Cell in "Cell-Map"
			@param	IndexTexture
				Index of Texture (in Material-Table)
			@param	InstanceCellMap
				"Cell-Map" Data
			@param	InstanceCell
				Cell Data
			@retval	Return-Value
				true == "Output" is valid<br>
				false == "Output" is invalid

			Set cell data in "Cell-Map Change"-Table.<br>
			Caution: Cell data is not referencing "InstanceCellMap" and "InstanceCell".
			*/
			public static bool CellSetFull(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
											int IndexCellMap,
											int IndexCell,
											int IndexTexture,
											Library_SpriteStudio.Data.CellMap InstanceCellMap,
											Library_SpriteStudio.Data.Cell InstanceCell
										)
			{
				if(false == TableCheckValidIndex(InstanceTableCellChange, IndexCellMap, IndexCell))
				{
					return(false);
				}

				Library_SpriteStudio.Data.CellMap InstanceCellMapNew = new Library_SpriteStudio.Data.CellMap();
				InstanceCellMapNew.Duplicate(InstanceCellMap);
				InstanceCellMapNew.ListCell = null;	/* Dis-Use */

				Library_SpriteStudio.Data.Cell InstanceCellNew = new Library_SpriteStudio.Data.Cell();
				InstanceCellNew.Duplicate(InstanceCell);

				InstanceTableCellChange[IndexCellMap][IndexCell].DataSet(IndexTexture, InstanceCellMapNew, InstanceCellNew);
				return(true);
			}

			/* ******************************************************** */
			//! Get Cell-Map count
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@retval	Return-Value
				Count of "Cell-Map"s<br>
				-1 == Failure (Error)

			Get count of "Cell-Map"s.
			*/
			public static int CountGetCellMap(Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange)
			{
				if(null == InstanceTableCellChange)
				{
					return(-1);
				}
				return(InstanceTableCellChange.Length);
			}

			/* ******************************************************** */
			//! Get Cell count
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				"Cell-Map"'s Index
			@retval	Return-Value
				Count of "Cell-Map"s<br>
				-1 == Failure (Error)

			Get count of Cells in the "Cell-Map".
			*/
			public static int CountGetCell(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
											int IndexCellMap
										)
			{
				if(null == InstanceTableCellChange)
				{
					return(-1);
				}
				if((0 > IndexCellMap) || (InstanceTableCellChange.Length <= IndexCellMap))
				{
					return(-1);
				}
				return(InstanceTableCellChange[IndexCellMap].Length);
			}

			/* ******************************************************** */
			//! Get Cell's Index
			/*!
			@param	InstanceTableCellChange
				"Cell-Map Change"-Table
			@param	IndexCellMap
				"Cell-Map"'s Index
			@param	Name
				Cell's Name
			@retval	Return-Value
				Cell's Index<br>
				-1 == Not-Found / Failure (Error)

			Get Cell's Index.
			*/
			public static int IndexGetCell(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
											int IndexCellMap,
											string Name
										)
			{
				int CountCell = CountGetCell(InstanceTableCellChange, IndexCellMap);
				if(0 >= CountCell)
				{
					return(-1);
				}

				Library_SpriteStudio.Control.CellChange[] InstanceCellMapChanged = InstanceTableCellChange[IndexCellMap];
				string NameCellNow = null;
				for(int i=0; i<CountCell; i++)
				{
					NameCellNow = InstanceCellMapChanged[i].DataCell.Name;
					if(true == string.IsNullOrEmpty(NameCellNow))
					{
						continue;
					}
//					if(0 == string.Compare(Name, NameCellNow))
					if(Name == NameCellNow)
					{
						return(i);
					}
				}
				return(-1);
			}
		}
	}
}
