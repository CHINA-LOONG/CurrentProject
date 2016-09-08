#if UNITY_ANDROID || UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System.IO;

public partial class FileModifier : System.IDisposable {
	
	private string filePath;
	private string fileContent;
	
	public FileModifier (string filePath) {
		if (!File.Exists(filePath)) {
			throw new FileNotFoundException(
				"Target file does not exist: "
				+ filePath);
		}

		UnityEngine.Debug.Log ("Opening file: " + filePath);
		
		this.filePath = filePath;
		StreamReader streamReader = new StreamReader (filePath);
		fileContent = streamReader.ReadToEnd ();
		streamReader.Close ();
	}

	public bool Contains (string pattern) {
		return fileContent.Contains (pattern);
	}
	
	public void InsertAfter (string pattern, string textToInsert) {
		string newText = pattern + "\n" + textToInsert + "\n";
		Replace (pattern, newText);
	}
	
	public void Replace (string pattern, string newText) {
		fileContent = fileContent.Replace (pattern, newText);
	}
	
	public void Append (string newText) {
		fileContent += '\n' + newText;
	}
	
	public void Write () {
		UnityEngine.Debug.Log ("Writing to file: " + filePath);

		StreamWriter streamWriter = new StreamWriter (filePath);
		streamWriter.Write (fileContent);
		streamWriter.Close ();
	}
	
	public void Dispose() {
	}
}

#endif