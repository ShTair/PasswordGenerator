using System;
using System.IO;
using System.Text;

namespace PasswordGenerator
{
    class Program
    {
        private static Random _r = new Random();

        static void Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("コマンドライン引数にパラメータファイルを指定するか、");
                Console.WriteLine("実行ファイルにパラメータファイルをドラッグしてください。");
                Console.WriteLine();

                var path = Path.GetFullPath("passparam.txt");
                if (File.Exists(path))
                {
                    Console.WriteLine("パラメータファイルの例が既に存在します。");
                    Console.WriteLine(path);
                    Console.WriteLine("パラメータファイルを再生成したい場合は、");
                    Console.WriteLine("既存のファイルを削除して再度実行してください。");
                }
                else
                {
                    using (var writer = new StreamWriter(path))
                    {
                        writer.WriteLine("3 0123456789");
                        writer.WriteLine("3 ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                        writer.WriteLine("3 abcdefghijklmnopqrstuvwxyz");
                        writer.WriteLine("3 !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");
                        writer.WriteLine("8");
                    }
                    Console.WriteLine("パラメータファイル例を作成しました。");
                    Console.WriteLine(path);
                }

                Console.ReadLine();
                return;
            }


            args[0] = Path.GetFullPath(args[0]);
            Console.WriteLine($"ロード: {args[0]}");

            var gen = new RandomStringGenerator();
            using (var reader = new StreamReader(args[0], Encoding.UTF8))
            {
                string line;
                int litcount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    var p = Param.Parse(line);
                    if (p == null) continue;

                    if (p.Literals == null)
                    {
                        if (p.Count > litcount) continue;
                        gen.AddParameter(p.Count);
                        Console.WriteLine($"使用文字{gen.ParametersCount}: {p.Count}文字 <- 候補すべての文字");
                    }
                    else
                    {
                        litcount += p.Literals.Length;
                        gen.AddParameter(p.Count, p.Literals);
                        Console.WriteLine($"使用文字{gen.ParametersCount}: {p.Count}文字 <- {string.Join("", p.Literals)}");
                    }
                }
            }

            if (gen.ParametersCount == 0)
            {
                Console.WriteLine($"パラメータファイルがうまく読み込めなかったようです。");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"パスワード長: {gen.Count}文字");
            Console.WriteLine();

            while (true)
            {
                Console.Write(gen.Generate());
                Console.ReadLine();
            }
        }

        class Param
        {
            public int Count { get; }
            public string Literals { get; }

            public Param(int count, string literals)
            {
                Count = count;
                Literals = literals;
            }

            public static Param Parse(string src)
            {
                int count;
                string data;

                var i = src.IndexOf(' ');
                if (i == -1)
                {
                    if (int.TryParse(src, out count))
                    {
                        if (count != 0) return new Param(count, null);
                    }
                }
                else if (i >= 1 && src.Length >= i + 2)
                {
                    if (int.TryParse(src.Remove(i), out count))
                    {
                        if (src.Length >= count + 2)
                        {
                            data = src.Substring(i + 1);
                            return new Param(count, data);
                        }
                    }
                }

                return null;
            }
        }
    }
}
