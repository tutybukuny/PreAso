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
            "Acid uric"
        };

        public void PrepareForAr(string dataFolderPath, string excelFileName, string arffFileName, double thres)
        {
            try
            {
                using (var pck = new ExcelPackage(new FileInfo(excelFileName)))
                {
                    var dataPrinter = new StreamWriter(arffFileName);
                    var headerContent = DataProcessing.Processing(excelFileName, thres);
                    var headerList = new List<string>();
                    var headerRegex = "\t(.+)\t";
                    foreach (var line in headerContent)
                    {
                        dataPrinter.WriteLine(line);
                        var match = Regex.Match(line, headerRegex);
                        headerList.Add(match.Groups[1].Value);
                    }

                    dataPrinter.WriteLine("@Data");

                    var sheet = pck.Workbook.Worksheets.Count > 1
                        ? pck.Workbook.Worksheets[2]
                        : pck.Workbook.Worksheets[1];
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
                                        prepareLine += ",?";
                                    else
                                        prepareLine += "," + Regex.Replace(value, "\\s+", "_");
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
            }

            return result;
        }

        #region Hide

        public void Prepare(string dataFolderPath, string excelFileName, string dicFileName, string featureNamesFile)
        {
            try
            {
                using (var pck = new ExcelPackage(new FileInfo(dataFolderPath + excelFileName)))
                {
                    var preparePrinter = new StreamWriter(dataFolderPath + excelFileName + " prepare.txt");
                    var dicContents = File.ReadAllLines(dataFolderPath + dicFileName).Where(l => l.Length > 0).ToList();
                    var dic = new Dictionary<string, int>();
                    foreach (var content in dicContents)
                    {
                        var arr = content.Split('\t');
                        dic.Add(arr[0], int.Parse(arr[1]));
                    }
                    var featureNames = File.ReadAllText(dataFolderPath + featureNamesFile);
                    var sheet = pck.Workbook.Worksheets.Count > 1
                        ? pck.Workbook.Worksheets[2]
                        : pck.Workbook.Worksheets[1];
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
                                    var cell = cells[i, 12];
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
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }
        }

        public void BuildData(string dataFolderPath, string dicFileName, string prepareFileName)
        {
            var dicContents = File.ReadAllLines(dataFolderPath + dicFileName).Where(l => l.Length > 0).ToList();
            var dic = new Dictionary<string, int>();
            foreach (var content in dicContents)
            {
                var arr = content.Split('\t');
                dic.Add(arr[0], int.Parse(arr[1]));
            }

            using (var reader = new StreamReader(dataFolderPath + prepareFileName))
            {
                var printer = new StreamWriter(dataFolderPath + prepareFileName.Replace("prepare", "data"));
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