using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var ps = new List<Param>();
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
                        Console.WriteLine($"使用文字{ps.Count + 1}: {p.Count}文字 <- 候補すべての文字");
                    }
                    else
                    {
                        litcount += p.Literals.Length;
                        Console.WriteLine($"使用文字{ps.Count + 1}: {p.Count}文字 <- {string.Join("", p.Literals)}");
                    }
                    ps.Add(p);
                }
            }

            var passCount = ps.Sum(t => t.Count);
            if (passCount == 0)
            {
                Console.WriteLine($"パラメータファイルがうまく読み込めなかったようです。");
                Console.ReadLine();
                return;
            }

            var all = Concat(ps.Select(t => t.Literals).Where(t => t != null));

            Console.WriteLine($"パスワード長: {ps.Sum(t => t.Count)}文字");
            Console.WriteLine();

            while (true)
            {
                var s = Concat(ps.Select(t => Shuffle(t.Literals == null ? all : t.Literals, t.Count)));
                var res = Shuffle(s);

                Console.Write(res);
                Console.ReadLine();
            }
        }

        private static char[] Character(int start, int count)
        {
            var ret = new char[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = (char)(start + i);
            }

            return ret;
        }

        private static T[] Concat<T>(IEnumerable<T[]> srcs)
        {
            var res = new T[srcs.Sum(t => t.Length)];
            int index = 0;

            foreach (var src in srcs)
            {
                src.CopyTo(res, index);
                index += src.Length;
            }

            return res;
        }

        /// <summary>
        /// sourceは破壊される
        /// </summary>
        private static T[] Shuffle<T>(T[] source, int num = -1)
        {
            if (num == -1) num = source.Length;

            var v = new T[num];
            T temp;

            for (int i = 0; i < num; i++)
            {
                var ri = _r.Next(i, source.Length);
                v[i] = temp = source[ri];
                source[ri] = source[i];
                source[i] = temp;
            }

            return v;
        }

        class Param
        {
            public int Count { get; }
            public char[] Literals { get; }

            public Param(int count, char[] literals)
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
                            return new Param(count, data.ToArray());
                        }
                    }
                }

                return null;
            }
        }
    }
}
