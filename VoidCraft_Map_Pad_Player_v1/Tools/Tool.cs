using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VoidCraft_Map_Pad_Player_v1.Raw_Materials_C;


namespace VoidCraft_Map_Pad_Player_v1.Tools
{
    class Tool
    {
        internal class CantCraftException : Exception
        {
            public CantCraftException(string message) : base(message) { }
        }

        //Klasa opisuje narz�dzia. Zak�adamy, �e takowe si� nie niszcz�

        private Texture2D _toolTexture;//Wczytana zostanie tekstura narz�dzia

        public Texture2D ToolTexture
        {
            get { return _toolTexture; }
            set { _toolTexture = value; }
        }
        private string _toolName;

        public string ToolName
        {
            get { return _toolName; }
            set { _toolName = value; }
        }
        private bool _isOwned;

        //Zmienna m�wi o tym, czy gracz posiada to narz�dzie czy nie
        public bool IsOwned
        {
            get { return _isOwned; }
            set { _isOwned = value; }
        }

        private RawMaterials _requirements;

        public RawMaterials Requirements
        {
            get { return _requirements; }
            set { _requirements = value; }
        }

        public bool CanCraft(RawMaterials PlayerMaterials)//b�dzie sprawdza�, czy wymagania pokrywaj� si� z posiadanym eq playera
        {
            if (PlayerMaterials.Wood < Requirements.Wood) return false;
            if (PlayerMaterials.Stone < Requirements.Stone) return false;
            if (PlayerMaterials.Lianas < Requirements.Lianas) return false;
            if (PlayerMaterials.Metal < Requirements.Metal) return false;
            if (PlayerMaterials.Water < Requirements.Water) return false;
            if (PlayerMaterials.Food < Requirements.Food) return false;
            return true;
        }

        public void Craft(ref RawMaterials PlayerMaterials)
        {
            if (CanCraft(PlayerMaterials))
            {

                PlayerMaterials.Wood -= Requirements.Wood;
                PlayerMaterials.Stone -= Requirements.Stone;
                PlayerMaterials.Lianas -= Requirements.Lianas;
                PlayerMaterials.Metal -= Requirements.Metal;
                PlayerMaterials.Water -= Requirements.Water;
                PlayerMaterials.Food -= Requirements.Food;
                IsOwned = true;
            }
            else throw new CantCraftException("Zbyt ma�o materia��w do stworzenia przedmiotu: " + this.ToolName);
            

        }

        public Tool(string TexturePath, string ToolName, int WoodNeeded, int StoneNeeded,
            int LianasNeeded, int MetalNeeded, int WaterNeeded, int FoodNeeded)
        //kontruktor, kt�ry tworzy narz�dzie i ustawia wymagania do jego posiadania(scrafcenia) przez playera
        {
            //potrzeba pomocy w za�adowaniu tekstury! :(
            this.ToolName = ToolName;
            _requirements = new RawMaterials(WoodNeeded, StoneNeeded, LianasNeeded, MetalNeeded, WaterNeeded, FoodNeeded);
            this.IsOwned = false;
        }




    }
}