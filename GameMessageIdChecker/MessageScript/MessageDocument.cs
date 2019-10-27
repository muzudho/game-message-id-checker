namespace GameMessageIdChecker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class MessageDocument
    {
        public List<string> Keys { get; private set; }

        private MessageDocument()
        {
            this.Keys = new List<string>();
        }

        public static MessageDocument Read(string pathFromExe)
        {
            var instance = new MessageDocument();

            var key = string.Empty;

            var lines = File.ReadAllLines(pathFromExe);
            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal))
                {
                    // コメント行。無視。

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    Trace.WriteLine($"Comment         | {line}");
                }
                else if (line.StartsWith("$", StringComparison.Ordinal))
                {
                    // キー。
                    instance.Keys.Add(line);

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    Trace.WriteLine($"Key             | {line}");
                }
                else if (line.StartsWith("&", StringComparison.Ordinal))
                {
                    // 命令行は無視☆（＾～＾）

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    Trace.WriteLine($"Instruction     | {line}");
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // 空行は無視するぜ☆（＾～＾）

                    // 出力ウィンドウに出すぜ☆（＾～＾）
                    Trace.WriteLine($"Empty           | {line}");
                }
                else
                {
                    // 本文は無視。
                    Trace.WriteLine($"Body            | {line}");
                }
            }

            return instance;
        }
    }
}
