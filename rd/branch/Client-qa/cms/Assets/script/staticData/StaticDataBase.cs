using UnityEngine;
using System.Collections;

public class StaticDataBase : MonoBehaviour {
	
	protected FireEngine.FireTABFile	m_StaticTable;
	
	[SerializeField]
	int m_RowNumber;
	public int RowNumAttr
	{
		get
		{
			return m_RowNumber;
		}
	}
	
	[SerializeField]
	int m_ColNumber;
	public int ColNumberAttr 
	{
		get 
		{
			return m_ColNumber;
		}
	}
	
	public	void	InitWithTableFile(string fileName)
	{
		m_StaticTable = new FireEngine.FireTABFile ();
		m_StaticTable.OpenFromFile (fileName);

		m_RowNumber = m_StaticTable.RowsNum;
		m_ColNumber = m_StaticTable.ColumnsNum;

		//m_StaticTable.LogSelf ();
	}
}
