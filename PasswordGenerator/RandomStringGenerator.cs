using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordGenerator
{
    class RandomStringGenerator
    {
        private Random _r = new Random();
        private List<Container> _containers = new List<Container>();

        public int Count { get; private set; }

        public int ParametersCount => _containers.Count;

        public void AddParameter(int count, string letters = null)
        {
            char[] array;
            if (letters == null) array = _containers.SelectMany(t => t.Letters).ToArray();
            else array = letters.ToArray();

            if (count > array.Length) throw new Exception();

            _containers.Add(new Container(count, array));
            Count += count;
        }

        public void AddParameter(string letters, int min = 0, int ratio = 1)
        {
            AddParameter(letters.ToArray(), min, ratio);
        }

        public void AddParameter(char[] letters, int min = 0, int ratio = 1)
        {

        }

        public string Generate()
        {
            var pass = new char[Count];
            var index = 0;
            foreach (var container in _containers)
            {
                Shuffle(container.Letters, container.Count);
                for (int i = 0; i < container.Count; i++)
                {
                    pass[index++] = container.Letters[i];
                }
            }

            Shuffle(pass, pass.Length);
            return new string(pass);
        }

        private void Shuffle(char[] array, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var r = _r.Next(i, array.Length);
                var temp = array[r];
                array[r] = array[i];
                array[i] = temp;
            }
        }

        private struct Container
        {
            public int Count;
            public char[] Letters;

            public Container(int count, char[] letters)
            {
                Count = count;
                Letters = letters;
            }
        }
    }
}
