/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.IO;
using System.Collections;

namespace Funplus.Internal
{
	public class FileModifier : System.IDisposable
	{
		
		private string filePath;
		private string fileContent;
		
		public FileModifier (string filePath)
		{
			if (!File.Exists (filePath)) 
			{
				throw new FileNotFoundException(
					"Target file does not exist: "
					+ filePath);
			}

//			UnityEngine.Debug.Log ("Opening file: " + filePath);
			
			this.filePath = filePath;
			StreamReader streamReader = new StreamReader (filePath);
			fileContent = streamReader.ReadToEnd ();
			streamReader.Close ();
		}

		/// <summary>
		/// Check if file contains contents of specific pattern.
		/// </summary>
		/// <param name="pattern">Pattern.</param>
		public bool Contains (string pattern) 
		{
			return fileContent.Contains (pattern);
		}

		public void InsertAfter (string pattern, string textToInsert) 
		{
			Replace (pattern, string.Format("{0}\n{1}", pattern, textToInsert));
		}

		public void InsertAfterIfNotExist (string pattern, string textToInsert)
		{
			if (!Contains (textToInsert))
			{
				InsertAfter (pattern, textToInsert);
			}
		}

		public void InsertBefore (string pattern, string textToInsert)
		{
			Replace (pattern, string.Format("{0}\n{1}", textToInsert, pattern));
		}
		
		public void Replace (string pattern, string newText) 
		{
			fileContent = fileContent.Replace (pattern, newText);
		}
		
		public void Append (string newText) {
			fileContent += '\n' + newText;
		}
		
		public void Write () 
		{
//			UnityEngine.Debug.Log ("Writing to file: " + filePath);

			StreamWriter streamWriter = new StreamWriter (filePath);
			streamWriter.Write (fileContent);
			streamWriter.Close ();
		}
		
		public void Dispose() {
		}
	}
}
