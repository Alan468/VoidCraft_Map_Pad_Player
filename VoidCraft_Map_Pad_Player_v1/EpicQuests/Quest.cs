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
using Microsoft.Xna.Framework;
using Raw_Materials_C;

namespace EpicQuests
{
    class Quest
    {
        //  TODO:
        //wymy�li� jak zapisywa� warto�ci dla spe�nionych warunk�w questa



        private String name;//nazwa questa

        public String Name
        {
            get { return name; }
            set { String name = value; }
        }

        private Vector2 start_position;

        public Vector2 Start_position
        {
            get { return start_position; }
            set { start_position = value; }
        }
        //Wymagania dla questa, zawieraj� pozycj� i to, czy dana pozycja zosta�a zaliczona(bool) czy nie
        private Dictionary<Vector2, bool> quest_requirements;

        public Dictionary<Vector2, bool> Quest_requirements
        {
            get { return quest_requirements; }
            set { quest_requirements = value; }
        }

        private RawMaterials materials_needed;

        public RawMaterials Materials_needed
        {
            get { return   materials_needed; }
            set {   materials_needed = value; }
        }

        private bool materials_for_quest_owned;

        public bool Materials_for_quest_owned
        {
            get { return materials_for_quest_owned; }
            set { materials_for_quest_owned = value; }
        }

        private bool activated;

        public bool Activated
        {
            get { return activated; }
            set { activated = value; }
        }


        private bool finished;

        public bool Finished
        {
            
            get { return finished; }
            //set { finished = value; }
        }

        public bool IsFinished(RawMaterials player_materials)
        {
            if (finished) return finished;//je�li ju� jest uko�czony nie przeszukuj tylko zwr��, �e uko�czono
            foreach (KeyValuePair<Vector2, bool> Quests in Quest_requirements)
            {
                if (Quests.Value == false)
                {//je�li cho� jeden warunek nie jest spe�nionny to zwr�c fa�sz
                    return false;
                }
            }//jak wszystko jest spelnione to ustaw, �e uko�czono i zwr�c prawd�

            if (!player_materials.Contains(materials_needed)) return false;

            materials_for_quest_owned = true;
            finished = true;
            return true;
        }


        public Quest(String name, Vector2 startposition,RawMaterials requested_materials, params Vector2 [] quest_places)
        {
            this.name = name;
            this.start_position.X = startposition.X;//, startposition.Y);
            this.start_position.Y = startposition.Y;
            quest_requirements = new Dictionary<Vector2, bool>();
            for (int i = 0; i < quest_places.Length; i++)
            {
                Quest_requirements.Add(quest_places [i], false);//dodaje miejsca, ktore trzeba zaliczy� i domy�lnie ustawia je na false
            }
            materials_needed = requested_materials;
            this.activated = false;
        }


    }
}