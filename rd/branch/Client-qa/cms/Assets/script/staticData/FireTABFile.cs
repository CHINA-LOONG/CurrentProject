using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FireEngine
{
	public enum FIELD_TYPE
	{
		EFT_NUM = 0,
		EFT_FLOAT = 1,
		EFT_TEXT = 2
	}

	public class FIELD
	{
		public FIELD_TYPE FieldType;
		public string FieldValueStr;
	}

	public class FireTABFile
	{
		private string mTableName = null;

		private int mRowsNum = 0;
		private int mColumnsNum = 0;

		private List<FIELD_TYPE> mFieldType = null;
		private Dictionary<string, int> mColumnKeyDic;
		private Dictionary<int ,string> mColumnIndexDic;
		private List<FIELD> mDataBufs = null;
		private List<int> mFilterColumn;
		
		public FireTABFile()
		{
			mTableName = "";
			if (mDataBufs == null)
				mDataBufs = new List<FIELD>();
			if (mFieldType == null)
				mFieldType = new List<FIELD_TYPE>();
			if (mColumnKeyDic == null)
				mColumnKeyDic = new Dictionary<string,int> ();

			if (mColumnIndexDic == null)
				mColumnIndexDic = new Dictionary<int, string> ();

			if (mFilterColumn == null) 
			{
				mFilterColumn = new List<int>();
			}
		}
		public FireTABFile (string tableName)
		{
			mTableName = tableName;
			if (mDataBufs == null)
				mDataBufs = new List<FIELD>();
			if (mFieldType == null)
				mFieldType = new List<FIELD_TYPE>();

			if (mColumnKeyDic == null)
				mColumnKeyDic = new Dictionary<string,int> ();

			if (mColumnIndexDic == null)
				mColumnIndexDic = new Dictionary<int, string> ();

			if (mFilterColumn == null) 
			{
				mFilterColumn = new List<int>();
			}
		}
		
		public void Destroy()
		{
			ClearRecords ();
		}

        public void ClearRecords()
        {
			if(mFieldType != null)
			{
				mFieldType.Clear();
				mFieldType = null;
			}
			if(mDataBufs != null)
			{
				mDataBufs.Clear();
				mDataBufs = null;
			}
			
			if (mColumnKeyDic != null) 
			{
				mColumnKeyDic.Clear();
				mColumnKeyDic = null;
			}
			
			if (mColumnIndexDic != null) 
			{
				mColumnIndexDic.Clear();
				mColumnIndexDic = null;
			}
			if (mFilterColumn != null) 
			{
				mFilterColumn.Clear();
			}
			
			mRowsNum = mColumnsNum = 0;
        }

        public string TableName { get { return mTableName; } }

		public int ColumnsNum { get { return mColumnsNum; } }

		public int RowsNum { get { return mRowsNum; } }

		public void OpenFromFile(UnityEngine.AssetBundle bundle, string fileName, bool bList = true)
		{
			if (bundle == null) return;
			
			UnityEngine.TextAsset txtAsset = null;
			if (bList)
			{
	           // txtAsset = bundle.Load(fileName, typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
			}
			else
			{
	            txtAsset = bundle.mainAsset as UnityEngine.TextAsset;
			}
			
			if(txtAsset != null)
			{
				byte[] sourceBytes = txtAsset.bytes;
				OpenFromBytes (sourceBytes);
				Logger.Log("FireTABFile.OpenFromFile:" + fileName + " loaded!");
			} 
            else
            {
                Logger.Log("FireTABFile.OpenFromFile:" + fileName + " doesn't exist!");
			}
		}

		public bool OpenFromFile(string filename)
		{
			try
			{
				byte[] bytes = null;
				
				FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
				BinaryReader reader = new BinaryReader (stream);
				
				bytes = reader.ReadBytes ((int)stream.Length);
				reader.Close ();
				stream.Close ();
			
				bool ret = OpenFromBytes (bytes);
				
				mTableName =  Path.GetFileNameWithoutExtension (filename);
				return ret;
			}
			catch(IOException exception)
			{
				string errMsg = "Error while open tab: " + filename;
				Debug.LogError(errMsg);
                Logger.LogException(exception);
				return false;
			}
			catch(UnauthorizedAccessException exception)
			{
				string errMsg = "Error while open tab: " + filename;
                Logger.LogError(errMsg);
                Logger.LogException(exception);
				return false;
			}
		}

		public bool OpenFromFileEx(byte[] bytes)
		{
			try
			{
				bool ret = OpenFromBytes (bytes);
				return ret;
			}
			catch(IOException exception)
			{
				string errMsg = "Error while open server res sync config.";
                Logger.LogError(errMsg);
                Logger.LogException(exception);
				return false;
			}
			catch(UnauthorizedAccessException exception)
			{
				string errMsg = "Error while open server res sync config.";
                Logger.LogError(errMsg);
                Logger.LogException(exception);
				return false;
			}
		}
		
		public static FireTABFile StaticOpenFromBytes(byte[] bytes)
		{
			FireTABFile ftf = new FireTABFile();
			if (ftf.OpenFromBytes(bytes))
				return ftf;
			else
				return null;
		}
		public bool OpenFromBytes (byte[] bytes)
		{
			string strAll = Encoding.Default.GetString (bytes);
			string[] lines = strAll.Split ('\n');
			
            mRowsNum = mColumnsNum = 0;
			string[] splitedArray;
			List<string> strList = new List<string> ();
			
			int lineIndex = -1;



			foreach(string strLine in lines)
			{
				lineIndex++;

				splitedArray = strLine.Split (',');
				strList.Clear ();
				strList.AddRange (splitedArray);

				if(lineIndex == 0)
				{
					//Data Struct
					int columnIndex = 0;
					foreach (string typeStr in strList)
					{
						string tempStr = typeStr.Trim ();
						if (tempStr == "NUM")
						{
                            mFieldType.Add(FIELD_TYPE.EFT_NUM);
						}
						else if (tempStr == "FLOAT")
						{
                            mFieldType.Add(FIELD_TYPE.EFT_FLOAT);
						}
						else if (tempStr == "TEXT")
						{
                            mFieldType.Add(FIELD_TYPE.EFT_TEXT);
						}
						else if (tempStr == "CNC")
						{
                            mFieldType.Add(FIELD_TYPE.EFT_TEXT);
						}
						else if (tempStr == "NULL")
						{
							//filter column
							mFilterColumn.Add(columnIndex);
						}

						columnIndex ++;
					}
                    mColumnsNum = mFieldType.Count;
				}
				else if(lineIndex == 1)
				{
					//Column Key
					if((strList.Count - mFilterColumn.Count) != mFieldType.Count)
					{
						Debug.LogError("Table  Header Error mFilterColumn = " + mFilterColumn.Count);
						return false;
					}
					int filterColumnCount = 0;
					for(int i =0 ; i < strList.Count; ++i)
					{
						if(mFilterColumn.Contains(i))
						{
							filterColumnCount ++;
							continue;
						}

						string subColumnKey = strList[i].Trim();
						mColumnKeyDic.Add(subColumnKey,i + 1 + filterColumnCount);
						mColumnIndexDic.Add(i + 1 + filterColumnCount,subColumnKey);
					}
				}
				else
				{
					//Data
					if (strLine.Length < 1 || strLine.StartsWith("#")) continue;

					if (splitedArray.Length == 0) continue;
					if ((splitedArray.Length - mFilterColumn.Count) < mFieldType.Count)
					{
						int nSubNum = mFieldType.Count - splitedArray.Length - mFilterColumn.Count;
						
						for (int _Idx = 0; _Idx < nSubNum; _Idx++)
						{
							strList.Add ("");
						}
					}

					int filterColumnCount = 0;
					for (int _Idx = 0; _Idx < strList.Count; _Idx++)
					{
						if(mFilterColumn.Contains(_Idx))
						{
							filterColumnCount ++;
							continue;
						}

						FIELD newField;
						switch (mFieldType[_Idx - filterColumnCount ])
						{
						case FIELD_TYPE.EFT_NUM:
							newField = new FIELD ();
							newField.FieldType = FIELD_TYPE.EFT_NUM;
							newField.FieldValueStr = strList[_Idx].Trim();
							mDataBufs.Add (newField);
							break;
						
						case FIELD_TYPE.EFT_FLOAT:
							newField = new FIELD ();
							newField.FieldType = FIELD_TYPE.EFT_FLOAT;
							newField.FieldValueStr = strList[_Idx].Trim();
							mDataBufs.Add (newField);
							break;
						
						case FIELD_TYPE.EFT_TEXT:
							newField = new FIELD ();
							newField.FieldType = FIELD_TYPE.EFT_TEXT;
							newField.FieldValueStr = strList[_Idx].Trim();
							mDataBufs.Add (newField);
							break;
						}
					}
                    mRowsNum++;
				}
			}
			return true;
		}

        public bool AddRecord(string record)
        {
            string[] splitedArray;
            List<string> strList = new List<string>();

            if (record.Length < 1 || record[0] == '#') return false;
            splitedArray = record.Split(',');
            strList.AddRange(splitedArray);

            if (splitedArray.Length == 0) return false;
            if (splitedArray.Length != mFieldType.Count)
            {
                if (splitedArray.Length < mFieldType.Count)
                {
                    int nSubNum = mFieldType.Count - splitedArray.Length;

                    for (int _Idx = 0; _Idx < nSubNum; _Idx++)
                    {
                        strList.Add("");
                    }
                }
            }

            if (strList[0].Length == 0)
                return false;

            for (int _Idx = 0; _Idx < mFieldType.Count; _Idx++)
            {
                FIELD newField;
                switch (mFieldType[_Idx])
                {
                    case FIELD_TYPE.EFT_NUM:
                        newField = new FIELD();
                        newField.FieldType = FIELD_TYPE.EFT_NUM;
                        newField.FieldValueStr = strList[_Idx];
                        mDataBufs.Add(newField);
                        break;

                    case FIELD_TYPE.EFT_FLOAT:
                        newField = new FIELD();
                        newField.FieldType = FIELD_TYPE.EFT_FLOAT;
                        newField.FieldValueStr = strList[_Idx];
                        mDataBufs.Add(newField);
                        break;

                    case FIELD_TYPE.EFT_TEXT:
                        newField = new FIELD();
                        newField.FieldType = FIELD_TYPE.EFT_TEXT;
                        newField.FieldValueStr = strList[_Idx];
                        mDataBufs.Add(newField);
                        break;
                }
            }
            mRowsNum++;

            return true;
        }

		public bool FieldEqual (FIELD a, FIELD b)
		{
			return a.FieldType == b.FieldType && a.FieldValueStr == b.FieldValueStr;
		}

		public FIELD GetCertainField (int nRowNum, string colKey)
		{
			int nColumnNum = mColumnKeyDic [colKey];
			return GetCertainField (nRowNum, nColumnNum);
		}

		public FIELD GetCertainField (int nRowNum, int nColumnNum)
		{
			if (nRowNum < 1 || nRowNum > mRowsNum || nColumnNum < 1 || nColumnNum > mColumnsNum)
			{
              //  FireLogger.LogMsg("FireTABFile.GetCertainField");
				return null;
			}
			int position = (nRowNum - 1) * mColumnsNum + (nColumnNum - 1);
			return mDataBufs[position];
		}

		public List<FIELD> GetSpecifiedRowFields (int nRowNum)
		{
			if (nRowNum > mRowsNum || nRowNum < 1)
			{
               // FireLogger.LogMsg("FireTABFile.GetSpecifiedRowFields");
				return null;
			}
			int length = nRowNum * mColumnsNum;
			List<FIELD> tempList = new List<FIELD>();
			for (int i = (nRowNum - 1) * mColumnsNum; (i < length) && (i < mDataBufs.Count); i++)
			{
				tempList.Add (mDataBufs[i]);
			}
			return tempList;
		}

		public List<FIELD> GetRowFieldsWithPrimaryColAndValue(string primaryKey, string value)
		{
			//FIELD_TYPE fType = GetColumnTyp (primaryKey);

			List<FIELD> allKey = GetSpecifiedColumnFields (primaryKey);

			for (int i = 0; i < allKey.Count; ++i) 
			{
				FIELD subField = allKey[i];
				if(subField.FieldValueStr == value)
				{
					return GetSpecifiedRowFields(i);
				}
			}
			return null;
		}

		public List<FIELD> GetSpecifiedColumnFields (string colKey)
		{
			int nColumnNum = mColumnKeyDic [colKey];
			return GetSpecifiedColumnFields (nColumnNum);
		}

		public List<FIELD> GetSpecifiedColumnFields (int nColumnNum)
		{
			if (nColumnNum > mColumnsNum || nColumnNum < 1)
			{
              ///  FireLogger.LogMsg("FireTABFile.GetSpecifiedColumnFields");
				return null;
			}
			List<FIELD> tempList = new List<FIELD>();
			for (int i = nColumnNum - 1; i < mDataBufs.Count; i += mColumnsNum)
			{
				tempList.Add (mDataBufs[i]);
			}
			return tempList;
		}

		public List<FIELD> GetFieldsByIndex (int nIndex)
		{
            List<FIELD> tempFieldsList = GetSpecifiedColumnFields(1);
            if (tempFieldsList == null) return null;
            return GetSpecifiedRowFields(nIndex + 1);
		}

        public List<FIELD> GetFieldsByKey(int key)
        {
            List<FIELD> tempFieldsList = GetSpecifiedColumnFields(1);
            if (tempFieldsList == null) return null;

            for (int i = 0; i < tempFieldsList.Count; i++)
                if (Convert.ToInt32(tempFieldsList[i].FieldValueStr) == key)
                    return GetSpecifiedRowFields(i + 1);

            return null;
        }

		public FIELD SearchFirstColumnEqual (int nColumnNum, FIELD value)
		{
			if (nColumnNum < 0 || nColumnNum >= mColumnsNum)
				return null;
			for (int i = 0; i < mRowsNum; i++)
			{
				FIELD theField = mDataBufs[mColumnsNum * i + nColumnNum];
				if (FieldEqual(theField, value))
				{
					return mDataBufs[mColumnsNum * i];
				}
			}
			return null;
		}

		public FIELD_TYPE GetColumnTyp(string colKey)
		{
			int nColumn = mColumnKeyDic [colKey];
			return GetColumnType (nColumn);
		}

		public FIELD_TYPE GetColumnType (int nColumn)
		{
			return mFieldType[nColumn];
		}

		public static int GetCharOff(string szStr, char ch, int nCount)
		{
			if (null == szStr)
				return -1;
			
			int uNum = 0, uIndex = 0, uSize = szStr.Length;
			
			for (int _Idx = 0; _Idx < uSize; ++_Idx)
			{
				if (ch == szStr[_Idx])
				{
					++uNum;
					uIndex = _Idx;
					
					if (uNum >= nCount)
						return _Idx;
				}
			}
			return uIndex;
		}

		public static string GetSplitData(string szData, int nCount)
		{
			if (null == szData)
				return "";
			
			int off1 = 0;
			
			if (nCount > 1)
			{
				off1 = GetCharOff (szData, '|', nCount - 1);
				if (0 != off1)
					++off1;
			}
			
			int off2 = GetCharOff (szData, '|', nCount);
			
			if (off2 <= off1)
			{
				off2 = szData.Length;
			}
			
			int copySize = off2 - off1;
			
			if (0 < copySize)
			{
				return szData.Substring (off1, copySize);
			}
			
			return "";
		}

		public void OutPutAllDatas (string pathName)
		{
            if (!Application.isEditor) return;

			if(mDataBufs != null)
			{
				FileStream fs = new FileStream (pathName, FileMode.Create);
				StreamWriter sw = new StreamWriter (fs);
				Debug.Log("write data..");
				//title
				for(int i =0; i< mFieldType.Count;++i)
				{
					FIELD_TYPE fType = mFieldType[i];
					if(fType == FIELD_TYPE.EFT_NUM)
					{
						sw.Write("NUM");
					}
					else if (fType == FIELD_TYPE.EFT_FLOAT)
					{
						sw.Write("FLOAT");
					}
					else if(fType == FIELD_TYPE.EFT_TEXT)
					{
						sw.Write("TEXT");
					}
					if(i == mFieldType.Count -1 )
					{
						sw.WriteLine();
					}
					else
					{
						sw.Write(",");
					}
				}
				//key 
				for(int i =1 ;i <= mColumnsNum; ++i)
				{
					string subKey = mColumnIndexDic[i];
					sw.Write(subKey);

					if(i == mColumnsNum)
					{
						sw.WriteLine();
					}
					else
					{
						sw.Write(",");
					}
				}
				//data
				for(int i = 0; i < mDataBufs.Count; i ++)
				{
					sw.Write (mDataBufs[i].FieldValueStr.Replace("\r",""));
					if ((i+1) % mColumnsNum == 0) 
					{
						sw.WriteLine();
					}
					else
					{
						sw.Write (",");
					}
				}
				sw.Flush();
                sw.Close(); fs.Close();
			}
		}
		
		public void OutPutAllDatas(string sourPathName, string curPathName)
		{
			if (!Application.isEditor) return;

			if(mDataBufs != null)
			{
				//title 
                FileStream readStream = new FileStream(sourPathName, FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader sr = new StreamReader(readStream);
				string strAll = sr.ReadToEnd();
				readStream.Close();
				sr.Close();
                string[] lines = strAll.Split('\n');
				
				//data
				FileStream fs = new FileStream (curPathName, FileMode.Create);
				StreamWriter sw = new StreamWriter (fs);

                sw.WriteLine(lines[0].Replace("\r",""));
                sw.WriteLine(lines[1].Replace("\r",""));
				
				for(int i = 0; i < mDataBufs.Count; i ++)
				{
					sw.Write (mDataBufs[i].FieldValueStr.Replace("\r","") + "	");
					if ((i+1) % mColumnsNum == 0) sw.WriteLine();
				}
				sw.Flush();
                sw.Close(); fs.Close();
			}
		}

		public int GetRowNumWithPrimaryKey(string key,string value)
		{
			int nColumnNum = mColumnKeyDic [key];
			return GetRowNumWithPrimaryKey (nColumnNum, value);
		}

		public int GetRowNumWithPrimaryKey(int nColumnNum,string value)
		{
			for (int i = 1; i < mRowsNum + 1; i++)
			{
				FIELD theField = mDataBufs[ (i-1) * mColumnsNum + nColumnNum - 1];
				if(theField.FieldValueStr == value)
				{
					return i;
				}
			}
			return -1;
		}

		public void SetCertainField(int nRow,int nCol,string value)
		{
			if (nRow < 1 || nRow > mRowsNum || nCol < 1 || nCol > mColumnsNum)
			{
				Debug.LogError("SetFiled Error!");
				return ;
			}
			int position = (nRow - 1) * mColumnsNum + (nCol - 1);
			FIELD tempData = mDataBufs[position];
			tempData.FieldValueStr = value;
			mDataBufs [position] = tempData;
		}

		public	void LogSelf()
		{
			int column = this.ColumnsNum;
			int rows = this.RowsNum;
			
			string tableName = this.TableName;
			Debug.Log ("TableName : " + tableName + " col = " + column + " row = " + rows);

			Debug.Log ("Data*************************Data");

			for (int i = 0; i< rows; ++i)
			{
				string tempMsg = "";
				for(int j =0;j<column; ++j)
				{
					FIELD subField = GetCertainField(i +1 , j+1);
					tempMsg = tempMsg + "\t" + subField.FieldValueStr;
				}
				Debug.Log("Row : " + (i+1) + "\t" + tempMsg);
			}
		}
	}
}