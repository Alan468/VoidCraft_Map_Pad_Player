using System;

namespace Raw_Materials_C
{
    [Serializable]
   public class RawMaterials
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
                    throw new OutOfWoodException("Zbyt ma這 drewna!");
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
                    throw new OutOfStoneException("Zbyt ma這 kamienia!");
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
                { throw new OutOfLianasException("Zbyt ma這 lian!"); } else
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
                { throw new OutOfMetalException("Zbyt ma這 metalu!"); } else
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
                { throw new OutOfWaterException("Zbyt ma這 wody!"); } else
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
                    throw new OutOfFoodException("Zbyt ma這 jedzienia!");
                } else
                {
                    _food = value;
                }
            }
        }

        public bool Contains(RawMaterials checked_materials)//sprawdza czy dane materia造 zawieraj� podane w parametrach
            //na przyk豉d je�li chcemy sprawdzi�, czy gracz mo瞠 sobie pozwoli� na scrafcenie czego� to por闚nujemy jego materia造 z wymaganymi
        {
            if (this.Wood < checked_materials.Wood)
                return false;
            if (this.Stone < checked_materials.Stone)
                return false;
            if (this.Lianas < checked_materials.Lianas)
                return false;
            if (this.Metal < checked_materials.Metal)
                return false;
            if (this.Water < checked_materials.Water)
                return false;
            if (this.Food < checked_materials.Food)
                return false;
            return true;
            
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