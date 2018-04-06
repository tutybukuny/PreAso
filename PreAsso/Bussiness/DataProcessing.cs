using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using PreAsso.Entity;

namespace PreAsso.Bussiness
{
    public class DataProcessing
    {
        /// <summary>
        ///     Preprocessing data
        /// </summary>
        /// <param name="excelFileName">excel data file</param>
        /// <param name="thres">threshold of ratio</param>
        /// <returns>return header of arff file</returns>
        public static List<string> Processing(string excelFileName, double thres)
        {
            var features = new List<FeatureEntity>();
            var lines = new List<string>();

            try
            {
                using (var pck = new ExcelPackage(new FileInfo(excelFileName)))
                {
//                    var sheet = pck.Workbook.Worksheets.Count > 1
//                        ? pck.Workbook.Worksheets[2]
//                        : pck.Workbook.Worksheets[1];
                    var sheet = pck.Workbook.Worksheets[1];
                    var rows = sheet.Dimension.Rows;
                    var cols = sheet.Dimension.Columns;

                    for (var i = 1; i <= rows; i++)
                    for (var j = 1; j <= cols; j++)
                        if (i == 1)
                        {
                            var name = sheet.Cells[i, j].Value.ToString();
                            var feature = new FeatureEntity {Count = 0, Name = name};
                            features.Add(feature);
                        }
                        else
                        {
                            var value = sheet.Cells[i, j].Value == null
                                ? null
                                : sheet.Cells[i, j].Value.ToString().Trim();
                            if (!string.IsNullOrEmpty(value))
                            {
                                features[j - 1].Count++;
                                if (!features[j - 1].ValueList.Contains(value))
                                    features[j - 1].ValueList.Add(value);
                            }
                        }

                    var numberList = new List<string>
                    {
                        "Mạch toàn thân (lần/phút)",
                        "Mạch toàn thân (lần phút)",
                        "Nhiệt độ (độ C)",
                        "Nhịp thở (lần/phút)",
                        "Nhịp thở (lần phút)",
                        "Ure",
                        "Creatinin",
                        "Glucose máu"
                    };
                    var ignoreList = new List<string>
                    {
                        "1. Triệu chứng toàn thân",
                        "Lý do vào viện",
                        "A. Bệnh sử",
                        "Sinh ngày",
                        "Ngày vào viện",
                        "Dấu thời gian",
                        "Nơi làm việc",
                        "2. Khám các bộ phận",
                        "2. Khám bộ phận",
                        "13. Glucose máu (GM)",
                        "Họ và tên",
                        "Mã bệnh nhân (nếu có)",
                        "* Diễn giải mục 1",
                        "* Diễn giải mục 7",
                        "* Diễn giải mục 8",
                        "Cân nặng (Kg)",
                        "* Diễn giải mục 14",
                        "* Diễn giải mục 15",
                        "Địa chỉ"
                    };

                    lines.Add("@RELATION\tmedical\r\n");
                    lines.Add("@ATTRIBUTE\tTuổi\t{thanh_niên,trung_niên,lão_niên}");
                    foreach (var feature in features)
                    {
                        if (ignoreList.Contains(feature.Name)) continue;
                        feature.PercentCalculating(rows - 1);
                        if (feature.Percent < thres) continue;
                        var line = "@ATTRIBUTE\t" + Regex.Replace(feature.Name.Replace(",", ""), "\\s+", "_") + "\t{";
                        if (feature.Name == "Acid uric" || feature.Name == "Huyết áp ngưỡng thấp (mmHg)"
                            || feature.Name == "Huyết áp ngưỡng cao (mmHg)")
                        {
                            line += "bình_thường,cao";
                        }
                        else if (feature.Name == "Chỉ số Glucose máu (GM)")
                        {
                            line += "bình_thường,tiền_đái_tháo_đường,đái_tháo_đường";
                        }
                        else if (numberList.Contains(feature.Name))
                        {
                            line += "thấp,bình_thường,cao";
                        }
                        else
                        {
                            var count = 0;
                            foreach (var value in feature.ValueList)
                                line += (count++ == 0 ? "" : ",") + Regex.Replace(value, "\\s+", "_");
                        }
                        line += "}";
                        lines.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message, e);
            }

            return lines;
        }
    }
}