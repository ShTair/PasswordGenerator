using System;
using System.Collections.Generic;

namespace PasswordGenerator
{
    class Selector<T>
    {
        private Random _random = new Random();

        private List<_Container> _list = new List<_Container>();
        private int _total;

        public void Add(T item, int weight)
        {
            _total += weight;
            _list.Add(new _Container { Item = item, Weight = _total });
        }

        public T Draw()
        {
            var rnd = _random.Next(_total);

        }

        private class _Container
        {
            public T Item;

            public int Weight;
        }
    }
}
