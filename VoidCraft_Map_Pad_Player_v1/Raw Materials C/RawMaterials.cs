using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Raw_Materials_C
{
    class RawMaterials
    {
        internal class OutOfWoodException : Exception
        {
            public OutOfWoodException(string message) : base(message) { }
        }
        internal class OutOfStoneException : Exception
        {
            public OutOfStoneException(string message) : base(message) { }
        }
        internal class OutOfLianasException : Exception
        {
            public OutOfLianasException(string message) : base(message) { }
        }
        internal class OutOfMetalException : Exception
        {
            public OutOfMetalException(string message) : base(message) { }
        }
        internal class OutOfWaterException : Exception
        {
            public OutOfWaterException(string message) : base(message) { }
        }
        internal class OutOfFoodException : Exception
        {
            public OutOfFoodException(string message) : base(message) { }
        }



        private int _wood;

        public int Wood
        {
            get { return _wood; }
            set {
                if (_wood + value < 0)
                {
                    throw new OutOfWoodException("Zbyt ma�o drewna!");
                } else
                {
                    _wood = value;
                }
            }
        }

        private int _stone;

        public int Stone
        {
            get { return _stone; }
            set {
                if (_stone + value < 0)
                {
                    throw new OutOfStoneException("Zbyt ma�o kamienia!");
                } else
                {
                    _stone = value;
                }
            }
        }

        private int _lianas;

        public int Lianas
        {
            get { return _lianas; }
            set {
                if (Lianas + value < 0)
                { throw new OutOfLianasException("Zbyt ma�o lian!"); } else
                {
                    _lianas = value;
                }
            }
        }
        private int _metal;

        public int Metal
        {
            get { return _metal; }
            set {
                if (_metal + value < 0)
                { throw new OutOfMetalException("Zbyt ma�o metalu!"); } else
                {
                    _metal = value;
                }
            }
        }

        private int _water;

        public int Water
        {
            get { return _water; }
            set {
                if (_water + value < 0)
                { throw new OutOfWaterException("Zbyt ma�o wody!"); } else
                { _water = value; }
            }
        }

        private int _food;

        public int Food
        {
            get { return _food; }
            set {
                if (_food + value < 0)
                {
                    throw new OutOfFoodException("Zbyt ma�o jedzienia!");
                } else
                {
                    _food = value;
                }
            }
        }

        //Konstruktory
        public RawMaterials()
        {
            Wood = 0;
            Stone = 0;
            Lianas = 0;
            Metal = 0;
            Water = 0;
            Food = 0;
        }
        public RawMaterials(int Wood, int Stone, int Lianas, int Metal, int Water, int Food)
        {
            this.Wood = Wood;
            this.Stone = Stone;
            this.Lianas = Lianas;
            this.Metal = Metal;
            this.Water = Water;
            this.Food = Food;
        }

    }
}