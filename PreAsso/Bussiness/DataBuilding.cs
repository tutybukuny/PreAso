using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace PreAsso.Bussiness
{
    public class DataBuilding
    {
        public List<string> NumList = new List<string>
        {
            "Huyết áp ngưỡng thấp (mmHg)",
            "Huyết áp ngưỡng cao (mmHg)",
            "Mạch toàn thân (lần/phút)",
            "Mạch toàn thân (lần phút)",
            "Nhiệt độ (độ C)",
            "Nhịp thở (lần/phút)",
            "Nhịp thở (lần phút)",
            "Ure",
            "Creatinin",
            "Glucose máu",
            "Chỉ số Glucose máu (GM)",
            "Acid uric"
        };

        #region Association rules

        /// <summary>
        ///     building data for Association rules
        /// </summary>
        /// <param name="dataFolderPath">folder of data</param>
        /// <param name="excelFileName">excel data file</param>
        /// <param name="arffFileName">arff file name to save</param>
        /// <param name="thres">threshold of ratio</param>
        public void PrepareForAr(string dataFolderPath, string excelFileName, string arffFileName, double thres)
        {
            try
            {
                using (var pck = new ExcelPackage(new FileInfo(excelFileName)))
                {
                    var dataPrinter = new StreamWriter(arffFileName);
                    var headerContent = DataProcessing.Processing(dataFolderPath, excelFileName, thres);
                    var meanLines = File.ReadAllLines("same mean.txt");
                    var sameMeans = new List<List<string>>();
                    var means = new List<string>();

                    foreach (var line in meanLines)
                    {
                        var elements = Regex.Split(line, "\\s+");
                        means.Add(elements[0]);
                        sameMeans.Add(new List<string>(elements));
                    }

                    var headerList = new List<string>();
                    var headerRegex = "\t(.+)\t";
                    foreach (var line in headerContent)
                    {
                        dataPrinter.WriteLine(line);
                        var match = Regex.Match(line, headerRegex);
                        headerList.Add(match.Groups[1].Value);
                    }

                    dataPrinter.WriteLine("@Data");

                    var sheet = pck.Workbook.Worksheets[1];
                    var rows = sheet.Dimension.Rows;
                    var cols = sheet.Dimension.Columns;
                    var cells = sheet.Cells;

                    for (var i = 2; i <= rows; i++)
                    {
                        var prepareLine = "";
                        var lost = false;

                        #region age

                        var ageStr = cells[i, 4].Value?.ToString().Trim();
                        if (!string.IsNullOrEmpty(ageStr))
                        {
                            int age, now;
                            var nowStr = "";
                            try
                            {
                                age = int.Parse(ageStr);
                                if (cells[i, 12] == null)
                                {
                                    now = 2016;
                                }
                                else
                                {
                                    try
                                    {
                                        nowStr = cells[i, 12].Value.ToString().Trim();
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log.Error(e.Message, e);
                                        nowStr = null;
                                    }

                                    if (string.IsNullOrEmpty(nowStr))
                                    {
                                        now = 2016;
                                    }
                                    else
                                    {
                                        var m = Regex.Match(nowStr, @"\d+/\d+/(\d+)");
                                        now = int.Parse(m.Groups[1].Value);
                                    }
                                }
                                if (age > now)
                                {
                                    now += 2000;
                                    Console.WriteLine("ahihi");
                                }
                                else if (now - age > 100)
                                {
                                    now = 2016;
                                    Console.WriteLine("too old");
                                }

                                age = now - age;
                                if (age.ToString().Length == 0)
                                    continue;
                                prepareLine = ConvertToNominal(age, "Tuổi");
                            }
                            catch (Exception e)
                            {
                                Logger.Log.Error(e.Message, e);
                            }
                        }

                        #endregion

                        if (prepareLine.Length == 0)
                            continue;

                        var isMale = cells[i, 5].Value != null && cells[i, 5].Value.ToString() == "Nam";

                        for (var j = 1; j <= cols; j++)
                        {
                            var featureName = cells[1, j].Value.ToString();
                            featureName = Regex.Replace(featureName, "\\s+", "_").Replace(",", "");
                            if (!headerList.Contains(featureName)) continue;
                            var value = cells[i, j].Value == null ? "?" : cells[i, j].Value.ToString().Trim();
                            if (featureName == "Phân_loại")
                            {
                                if (value.Length > 0)
                                    prepareLine = prepareLine + "," + Regex.Replace(value, "\\s+", "_");
                                else
                                    lost = true;
                                continue;
                            }
                            if (headerList.Contains(featureName))
                            {
                                var match = Regex.Match(value, @"(\d+)/(\d+)");

                                if (NumList.Any(item => item.Replace(" ", "_") == featureName))
                                {
                                    if (match.Success)
                                    {
                                        var first = double.Parse(match.Groups[1].Value.Replace(",", "."));
                                        value = first + "";
                                    }
                                    else if (value.Contains("/"))
                                    {
                                        value = value.Replace(",", ".");
                                        value = Regex.Replace(value, @"((?!\d)(?!\.)).", "");
                                        value = value.Replace("/", "").Trim();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            value = value.Replace(",", ".");
                                            value = Regex.Replace(value, @"((?!\d)(?!\.)).", "");
                                            var t = double.Parse(value);
                                        }
                                        catch (Exception)
                                        {
                                            value = "";
                                        }
                                    }

                                    if (value != "" && value.Length > 0)
                                        value = ConvertToNominal(double.Parse(value), featureName.Replace("_", " "),
                                            isMale);
                                }

                                try
                                {
                                    if (value.Length == 0)
                                    {
                                        prepareLine += ",?";
                                    }
                                    else
                                    {
                                        value = Regex.Replace(value, "\\s+", "_");
                                        var index = DataProcessing.findSameMeans(value, sameMeans);
                                        value = index != -1 ? means[index] : value;
                                        prepareLine += "," + value;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Log.Error(e.Message, e);
                                }
                            }
                        }

                        if (!lost)
                            dataPrinter.WriteLine(prepareLine);
                        dataPrinter.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }
        }

        /// <summary>
        ///     convert value to norminal value
        /// </summary>
        /// <param name="value">value of feature</param>
        /// <param name="type">feature name</param>
        /// <param name="isMale">male or female</param>
        /// <returns>value of feature in norminal form</returns>
        private string ConvertToNominal(double value, string type, bool isMale = true)
        {
            var result = "";
            switch (type)
            {
                case "Acid uric":
                    if (isMale && value >= 420)
                        result = "cao";
                    else if (!isMale && value >= 360)
                        result = "cao";
                    else
                        result = "bình_thường";
                    break;
                case "Tuổi":
                    if (value <= 30)
                        result = "thanh_niên";
                    else if (value <= 65)
                        result = "trung_niên";
                    else
                        result = "lão_niên";
                    break;
                case "Creatinin":
                    if (isMale)
                    {
                        if (value < 53)
                            result = "thấp";
                        else if (value > 106)
                            result = "cao";
                        else
                            result = "bình_thường";
                    }
                    else
                    {
                        if (value < 44)
                            result = "thấp";
                        else if (value > 97)
                            result = "cao";
                        else
                            result = "bình_thường";
                    }
                    break;
                case "Glucose máu":
                    if (value < 3.9)
                        result = "thấp";
                    else if (value > 5.0)
                        result = "cao";
                    else
                        result = "bình_thường";
                    break;
                case "Huyết áp ngưỡng thấp (mmHg)":
                    result = value <= 80 ? "bình_thường" : "cao";
                    break;
                case "Huyết áp ngưỡng cao (mmHg)":
                    result = value <= 120 ? "bình_thường" : "cao";
                    break;
                case "Mạch toàn thân (lần/phút)":
                case "Mạch toàn thân (lần phút)":
                    if (value < 60)
                        result = "thấp";
                    else if (value > 100)
                        result = "cao";
                    else
                        result = "bình_thường";
                    break;
                case "Nhiệt độ (độ C)":
                    if (value < 36.1)
                        result = "thấp";
                    else if (value > 37.2)
                        result = "cao";
                    else
                        result = "bình_thường";
                    break;
                case "Chỉ số Glucose máu (GM)":
                    if (value < 5.6)
                        result = "bình_thường";
                    else if (value <= 6.9)
                        result = "tiền_đái_tháo_đường";
                    else
                        result = "đái_tháo_đường";
                    break;
            }

            return result;
        }

        #endregion

        #region The others

        public void Prepare(string dataFolderPath, string excelFileName, string dataFileName, double thres)
        {
            try
            {
                DataProcessing.Processing(dataFolderPath, excelFileName, thres, false);
                var meanLines = File.ReadAllLines("same mean.txt");
                var sameMeans = new List<List<string>>();
                var means = new List<string>();

                foreach (var line in meanLines)
                {
                    var elements = Regex.Split(line, "\\s+");
                    means.Add(elements[0]);
                    sameMeans.Add(new List<string>(elements));
                }
                using (var pck = new ExcelPackage(new FileInfo(excelFileName)))
                {
                    var preparePrinter = new StreamWriter(dataFolderPath + "/prepare.txt");
                    var dicContents = File.ReadAllLines(dataFolderPath + "/dictionary.txt").Where(l => l.Length > 0)
                        .ToList();
                    var dic = new Dictionary<string, int>();
                    foreach (var content in dicContents)
                    {
                        var arr = content.Split('\t');
                        dic.Add(arr[0], int.Parse(arr[1]));
                    }
                    var featureNames = File.ReadAllText(dataFolderPath + "/featureNamePrinter.txt");
                    var sheet = pck.Workbook.Worksheets[1];
                    var rows = sheet.Dimension.Rows;
                    var cols = sheet.Dimension.Columns;
                    var cells = sheet.Cells;

                    #region prepare

                    for (var i = 2; i <= rows; i++)
                    {
                        var prepareLine = "";
                        var lost = false;

                        #region age

                        var ageStr = cells[i, 4].Value?.ToString().Trim();
                        if (!string.IsNullOrEmpty(ageStr))
                        {
                            int age, now;
                            var nowStr = "";
                            try
                            {
                                age = int.Parse(ageStr);
                                if (cells[i, 12] == null)
                                {
                                    now = 2016;
                                }
                                else
                                {
                                    try
                                    {
                                        nowStr = cells[i, 12].Value.ToString().Trim();
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log.Error(e.Message, e);
                                        nowStr = null;
                                    }

                                    if (string.IsNullOrEmpty(nowStr))
                                    {
                                        now = 2016;
                                    }
                                    else
                                    {
                                        var m = Regex.Match(nowStr, @"\d+/\d+/(\d+)");
                                        now = int.Parse(m.Groups[1].Value);
                                    }
                                }

                                age = now - age;
                                prepareLine = "Tuổi:" + age + " ";
                            }
                            catch (Exception e)
                            {
                                Logger.Log.Error(e.Message, e);
                            }
                        }

                        #endregion

                        for (var j = 1; j <= cols; j++)
                        {
                            var featureName = cells[1, j].Value.ToString();
                            featureName = Regex.Replace(featureName, "\\s+", "_");
                            if (!featureNames.Contains(featureName)) continue;
                            var value = cells[i, j].Value == null ? "" : cells[i, j].Value.ToString().Trim();
                            if (featureName == "Phân_loại")
                            {
                                if (value.Length > 0)
                                    prepareLine = featureName + "_" + Regex.Replace(value, "\\s+", "_") + " " +
                                                  prepareLine;
                                else
                                    lost = true;
                                continue;
                            }
                            if (featureName == "Chỉ_số_Glucose_máu_(GM)")
                            {
                                if (value.Length > 0)
                                {
                                    var v = double.Parse(value);
                                    if (v < 5.6)
                                        value = "bình_thường";
                                    else if (v <= 6.9)
                                        value = "tiền_đái_tháo_đường";
                                    else
                                        value = "đái_tháo_đường";

                                    prepareLine = featureName + "_" + value + " " + prepareLine;
                                }
                                else
                                {
                                    lost = true;
                                }
                                continue;
                            }

                            if (value.Length == 0) continue;
                            if (dic.ContainsKey(featureName))
                            {
                                var match = Regex.Match(value, @"(\d+)/(\d+)");
                                if (match.Success)
                                {
                                    var first = double.Parse(match.Groups[1].Value.Replace(",", "."));
                                    //                                    var second = double.Parse(match.Groups[2].Value);
                                    value = first + "";
                                }
                                else
                                {
                                    value = value.Replace(",", ".");
                                    value = Regex.Replace(value, @"((?!\d)(?!\.)).", "");
                                    value = value.Replace("/", "").Trim();
                                }

                                try
                                {
                                    if (value.Length == 0) continue;
                                    prepareLine += featureName + ":" + double.Parse(value) + " ";
                                }
                                catch (Exception e)
                                {
                                    Logger.Log.Error(e.Message, e);
                                }
                            }
                        }

                        if (!lost)
                            preparePrinter.WriteLine(prepareLine);
                    }

                    #endregion

                    preparePrinter.Flush();
                    preparePrinter.Close();
                }

                BuildData(dataFolderPath, dataFileName);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }
        }

        public void BuildData(string dataFolderPath, string datFileName)
        {
            var dicContents = File.ReadAllLines(dataFolderPath + "/dictionary.txt").Where(l => l.Length > 0).ToList();
            var dic = new Dictionary<string, int>();
            foreach (var content in dicContents)
            {
                var arr = content.Split('\t');
                dic.Add(arr[0], int.Parse(arr[1]));
            }

            using (var reader = new StreamReader(dataFolderPath + "/prepare.txt"))
            {
                var printer = new StreamWriter(datFileName);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    foreach (var item in dic)
                        if (item.Key != "Tuổi")
                            line = line?.Replace(item.Key, (item.Value >= 87 ? item.Value - 86 : item.Value) + "");
                    //                        line = line?.Replace(item.Key, (item.Value >= 100 ? item.Value - 100 : item.Value) + "");
                    line = line.Replace("Tuổi", 1 + "");
                    printer.WriteLine(line);
                }
                printer.Flush();
                printer.Close();
            }
        }

        #endregion
    }

    internal enum FeatureType
    {
        Hant,
        Hanc,
        Mtt,
        Nd,
        Nt,
        Ure,
        Cre,
        Glu,
        Aci,
        Age
    }
}