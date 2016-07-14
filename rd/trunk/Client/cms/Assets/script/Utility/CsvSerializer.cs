//===================================================================================
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// This code is released under the terms of the CPOL license, 
//===================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace Csv.Serialization
{
    /// <summary>
    /// Serialize and Deserialize Lists of any object type to CSV.
    /// </summary>
    public class CsvSerializer<T> where T : new()
    {
        #region Fields
        private bool _ignoreEmptyLines = true;

        private bool _ignoreReferenceTypesExceptString = true;

        private string _newlineReplacement = ((char)0x254).ToString();

        private List<FieldInfo> _properties;

        private string _replacement = ((char)0x255).ToString();

        private string _rowNumberColumnTitle = "RowNumber";

        private char _separator = ',';

        private bool _useEofLiteral = false;

        private int _columnNameRow = 1;
        public int ColumnNameRow
        {
            get { return _columnNameRow; }
            set { _columnNameRow = value; }
        }
        private int _dataStartRow = 2;
        public int DataStartRow
        {
            get { return _dataStartRow; }
            set { _dataStartRow = value; }
        }
        #endregion Fields

        #region Properties
        public bool IgnoreEmptyLines
        {
            get { return _ignoreEmptyLines; }
            set { _ignoreEmptyLines = value; }
        }

        public bool IgnoreReferenceTypesExceptString
        {
            get { return _ignoreReferenceTypesExceptString; }
            set { _ignoreReferenceTypesExceptString = value; }
        }

        public string NewlineReplacement
        {
            get { return _newlineReplacement; }
            set { _newlineReplacement = value; }
        }

        public string Replacement
        {
            get { return _replacement; }
            set { _replacement = value; }
        }

        public string RowNumberColumnTitle
        {
            get { return _rowNumberColumnTitle; }
            set { _rowNumberColumnTitle = value; }
        }

        public char Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }

        public bool UseEofLiteral
        {
            get { return _useEofLiteral; }
            set { _useEofLiteral = value; }
        }

        #endregion Properties

        /// <summary>
        /// Csv Serializer
        /// Initialize by selected properties from the type to be de/serialized
        /// </summary>
        public CsvSerializer()
        {
            var type = typeof(T);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var q = new List<FieldInfo>();

            if (IgnoreReferenceTypesExceptString)
            {
                foreach (var a in fields)
                {
                    if (a.FieldType.IsValueType || a.FieldType.Name == "String")
                        q.Add(a);
                }
            }

            var r = from a in q
                    //where a.GetCustomAttribute<CsvIgnoreAttribute>() == null
                    orderby a.Name
                    select a;

            _properties = r.ToList();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        /*
        public List<T> Deserialize(Stream stream)
        {
            string[] columns;
            string[] rows;

            try
            {
                using (var sr = new StreamReader(stream))
                {
                    / *CsvFileReader reader = new CsvFileReader(stream);
                    List<string> d = new List<string>();
                    while (reader.ReadRow(d))
                    {
                        Debug.Log(d);
                    }* /

                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    columns = rows[ColumnNameRow].Split(Separator);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCsvFormatException("The CSV File is Invalid. See Inner Exception for more inoformation.", ex);
            }

            var data = new List<T>();

            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];

                if (IgnoreEmptyLines && string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else if (!IgnoreEmptyLines && string.IsNullOrEmpty(line))
                {
                    throw new InvalidCsvFormatException(string.Format(@"Error: Empty line at line number: {0}", row));
                }

                var parts = line.Split(Separator);

                var firstColumnIndex = 1;
                if (parts.Length == firstColumnIndex && parts[firstColumnIndex - 1] != null && parts[firstColumnIndex - 1] == "EOF")
                {
                    break;
                }

                var datum = new T();

                var start = DataStartRow;
                for (int i = start; i < parts.Length; i++)
                {
                    var value = parts[i];
                    var column = columns[i];

                    // continue of deviant RowNumber column condition
                    // this allows for the deserializer to implicitly ignore the RowNumber column
                    if (column.Equals(RowNumberColumnTitle) && !_properties.Any(a => a.Name.Equals(RowNumberColumnTitle)))
                    {
                        continue;
                    }

                    value = value
                        .Replace(Replacement, Separator.ToString())
                        .Replace(NewlineReplacement, Environment.NewLine);

                    FieldInfo p = null;
                    foreach (var item in _properties)
                    {
                        if (item.Name == column)
                        {
                            p = item;
                            break;
                        }
                    }

                    var converter = TypeDescriptor.GetConverter(p.FieldType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(datum, convertedvalue);
                }

                data.Add(datum);
            }

            return data;
        }
        */

        public List<T> Deserialize(List<string> header, List<List<string>> data)
        {
            var instance = new List<T>();

            for (int row = 0; row < data.Count; row++)
            {
                var parts = data[row];

                var datum = new T();

                var start = 0;
                for (int i = start; i < parts.Count; i++)
                {
                    string value = parts[i];
                    var field = header[i];

					if(string.IsNullOrEmpty(value))
					{
						continue;
					}

                    try
                    {
                        FieldInfo p = null;
                        foreach (var item in _properties)
                        {
                            if (item.Name == field)
                            {
                                p = item;
                                break;
                            }
                        }
                        var converter = TypeDescriptor.GetConverter(p.FieldType);
                        var convertedvalue = converter.ConvertFrom(value);

                        p.SetValue(datum, convertedvalue);
                    }
                    catch (System.Exception ex)
                    {
                        Logger.LogErrorFormat("Cannot find field:{0}  in class:{1}\n{2}", field, typeof(T), ex);
                    }
                }

                instance.Add(datum);
            }

            return instance;
        }
    }

    public class CsvIgnoreAttribute : Attribute { }

    public class InvalidCsvFormatException : Exception
    {
        /// <summary>
        /// Invalid Csv Format Exception
        /// </summary>
        /// <param name="message">message</param>
        public InvalidCsvFormatException(string message)
            : base(message)
        {
        }

        public InvalidCsvFormatException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
