namespace GameMessageIdChecker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using GameMessageIdChecker.MessageScript;

    public delegate void LineCallback(int row, LineModel model);

    public class MessageDocument
    {
        public List<LineModel> LineList { get; private set; }

        private MessageDocument()
        {
            this.LineList = new List<LineModel>();
        }

        public void ScanIdRow(LineCallback callback)
        {
            // 人が読みやすい行番号☆（＾～＾）
            int row = 1;

            foreach (var line in this.LineList)
            {
                if (line.Type == MessageScript.LineType.Id)
                {
                    callback(row, line);
                }

                row++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">カレント・ディレクトリからのパス☆（＾～＾）</param>
        /// <returns></returns>
        public static MessageDocument Read(string file)
        {
            var instance = new MessageDocument();

            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal))
                {
                    // コメント行。
                    var model = new LineModel(LineType.Comment, line);
                    instance.LineList.Add(model);

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    // Trace.WriteLine($"Comment         | {line}");
                }
                else if (line.StartsWith("$", StringComparison.Ordinal))
                {
                    // ID。
                    var model = new LineModel(LineType.Id, line);
                    instance.LineList.Add(model);

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    // Trace.WriteLine($"Key             | {line}");
                }
                else if (line.StartsWith("&", StringComparison.Ordinal))
                {
                    // 命令行☆（＾～＾）
                    var model = new LineModel(LineType.Instruction, line);
                    instance.LineList.Add(model);

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    // Trace.WriteLine($"Instruction     | {line}");
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // 空行☆（＾～＾）
                    var model = new LineModel(LineType.Empty, line);
                    instance.LineList.Add(model);

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    // Trace.WriteLine($"Empty           | {line}");
                }
                else
                {
                    // 本文☆（＾～＾）
                    var model = new LineModel(LineType.Body, line);
                    instance.LineList.Add(model);

                    // Trace.WriteLine($"Body            | {line}");
                }
            }

            return instance;
        }
    }
}
