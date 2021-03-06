using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MapControler;
using PadControler;
using System;
using System.Collections.Generic;
using PlayerControler;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Tools;
using Menu;
using System.Text;
using EpicQuests;
using Sounds;
using Message;
using InGameMenuControler;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Raw_Materials_C;
using System.Diagnostics;


/// ZALICZONE NA 58/70 ! Trza by sie napi� Alan D:

/////////////////////////////////////////////////////////////    J: Kurde nie da sie na timerze zrobic zbierania bo Button A sprawdza tylko warunek przy pierwszym nacisnieciu...
//////////////////      John                /////////////////
//////////////////      13.06.2017r         /////////////////   
//////////////////      16:11               /////////////////    A: Johnny dodaj tekstury drzewa ,kamienia ,wody itd do 4 warstwy
//////////////////      VERSION 0.1         /////////////////    P: Juan, trzeba zrobi� projekt mapy albo tekstury do toolsow
/////////////////////////////////////////////////////////////    A: ... co� tam wa�nego :/ 

// Zrobiona Klasa InGameMenu, nie wrzuca�em jej do maina jeszcze
// Testowy knefel okomentowany i nie b�dzie reagowa� na kliki

namespace VoidCraft_Map_Pad_Player_v1
{
    public class Game1 : Game
    {



        GraphicsDeviceManager Graphics;
        SpriteBatch spriteBatch;
        SpriteFont DefaultFont;

        Map map;
        GameControler Pad;
        MainMenu main;
       public static Messages messages;

        BackgroundSongs GameBgAmbient;
        SoundEffects GrassWalk;

        Direction WalkingDirection;

        public static int ScreenX { get; private set; }
        public static int ScreenY { get; private set; }

        double WalkSpeed = 0.05;
        int GameHour; // U�ywa� metody ChangeGameTime(int H,int M);
        int GameMinute;

        double Starting_posX = 26;
        double Starting_posY = 34;

        bool DebugMode = true;
        public static bool GameRunning = false;
        public static bool LoadedGame =  false;
        public static bool IsSoundPlaying = true;
        public static int SongPlayed = 0;

        Texture2D back;

        // Texture2D Knefel_EQ; // Guziczek do ingame menu **
        // Texture2D Inventory; // Ekwipunek Ingamemenu **
        bool IsMenuButtonPressed = false; // pomocniczy bool dow wy�wietlania menu **
        public InGameMenu InGameMenuManager;
        public InGameMenuState InGameMenuStateManager;
        public TouchCollection TouchCollectionManager;
     
        string SoundText = "Dzwiek";
        string SaveText = "Zapisz";
        ItemToCraftChosen ItemToCraftChosenManager = ItemToCraftChosen._None;
        public List<Rectangle> QuestsPos;

        float timer = 1;


        //John'owicz

        enum SwingDirection { Swing_Up, Swing_Down, Swing_Right, Swing_Left }
        SwingDirection swingdirection;
        public List<Texture2D> PlayerMoveTexture; // Tworzenie Listy na teksturyPlayera
        private Player Gracz; // Tworzenie istancji
        private int IloscKlatek = 4; // ilosc klatek w danej animacji
        bool PAC = false;
        bool Zbierz = false;
        DirectionPAC PACDirection;
        double PacTimer = 0;

        double DayCycleTimer = 0; // Timer dla systemu dnia i nocy
        public List<Texture2D> DayCycleTexture;  // Lista na Textury Nocy
        int DayCycle = 0;

        GamePadStatus buff = GamePadStatus.None;

        Rectangle Buff;

        // ----------------------------------------------------------------------------------------------------- Konstruktor
        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            Graphics.IsFullScreen = true;
            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 480;
            Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            WalkingDirection = Direction.Idle_Down;
            PACDirection = DirectionPAC.Pac_Left;
            swingdirection = SwingDirection.Swing_Up;
        }

        // ----------------------------------------------------------------------------------------------------- Init
        protected override void Initialize()
        {
            Buff = new Rectangle();

            ScreenX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            main = new MainMenu();

            messages = new Messages(Content, new Rectangle(50, 20, 200, 50), 3);

            DefaultFont = Content.Load<SpriteFont>("SpriteFontPL");

            PlayerMoveTexture = new List<Texture2D>();
            DayCycleTexture = new List<Texture2D>();

            ChangeGameTime(6, 30);

            GameBgAmbient = new BackgroundSongs("Takeover", Content, true, 0.80f, "Ambient");
            GrassWalk = new SoundEffects("GrassStep", Content, "GrassWalk");

            //MAPY->  ProjektTestowy  JohnnoweTekstury  NoweTeksturyV4  MalaMapa  POLIGON  VoidMap

            map = new Map(GraphicsDevice, "Map_Final_V2", ScreenX, ScreenY);
            map.SetPosition(Starting_posX, Starting_posY);


            Pad = new GameControler(GraphicsDevice, ScreenX, ScreenY);

            // Wczytywanie tekstur Cyklu Dnia
            String DayFoldName = "NightFolder\\Night_";
            for (int i = 0; i < 25; i++) { DayCycleTexture.Add(Content.Load<Texture2D>(DayFoldName + (i + 1))); }

            //// Wczytywanie tekstur Animacji i tworzenie instancji Player
            String CharFoldName = "Characters\\NewChar_";
            for (int i = 0; i < 12; i++) { PlayerMoveTexture.Add(Content.Load<Texture2D>(CharFoldName + (i))); }

            // Przekazuje teksture do postaci i ilosc klatek w danej animacji

            //---------------------------------------------------------------

            Gracz = new Player(GrassWalk.sound, PlayerMoveTexture[4], 1, IloscKlatek, 10, 600); // Gdyby� przekaza� ContenMenager Content jako parametr to wczytywanie si� nie zmieni ,a b�dzie w klasie ... :P


            //----------------------------------------------------------

            // --------------------------------------------------------------------------------------------------------------------------------------------------------

            //   Knefel_EQ = Content.Load<Texture2D>("Knefel_EQ"); // �adowanie tekstury knefla **
            //   Inventory = Content.Load<Texture2D>("UI\\Equipment"); // �adowanie tekstury menu **
            InGameMenuManager = new InGameMenu(Content);
            InGameMenuStateManager = InGameMenuState._Game;
            QuestsPos = new List<Rectangle>();


            base.Initialize();
        }

        // ----------------------------------------------------------------------------------------------------- Update
        protected override void Update(GameTime gameTime)
        {
            if (GameRunning == true)
            {



                Gracz.PosY = map.GetPosition().Y;
                Gracz.PosX = map.GetPosition().X;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    Exit();

                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer < 0)
                {
                    GameTimeControl();
                    timer = 1;   //Reset Timer
                }

                Gracz.gin(gameTime);
                Gracz.Update(gameTime);



                /// Zmiana predkosci animacji przy Pacnieciu
                if (PAC == false) { Gracz.milliseconsuPerFrame = 140; } else { Gracz.milliseconsuPerFrame = 60; }


                ///---------- STEROWANIE POSTACIA ----------------
                if (!IsMenuButtonPressed)
                {

                    if (Pad.IsButtonPresed(GamePadStatus.DirNone))
                    {
                        PAC = false;
                        if (buff == GamePadStatus.Up)
                        {
                            Gracz.Move(Direction.Idle_Back, PlayerMoveTexture);
                        }
                        else if (buff == GamePadStatus.Down)
                        {
                            Gracz.Move(Direction.Idle_Down, PlayerMoveTexture);
                        }
                        else if (buff == GamePadStatus.Right)
                        {
                            Gracz.Move(Direction.Idle_Right, PlayerMoveTexture);
                        }
                        else if (buff == GamePadStatus.Left)
                        {
                            Gracz.Move(Direction.Idle_Left, PlayerMoveTexture);
                        }
                    }



                    if (Pad.IsButtonPresed(GamePadStatus.Up))                   ///---------- UP ----------------
                    {
                        if (map.GetNextID(3, Direction.On) == 0)
                        {
                            WalkingDirection = Direction.Up;
                            buff = GamePadStatus.Up;
                            Gracz.Move(Direction.Up, PlayerMoveTexture);
                            map.MoveMap(0, -WalkSpeed);

                            if (map.GetNextID(3, Direction.On) != 0)
                                map.MoveMap(0, WalkSpeed);
                            swingdirection = SwingDirection.Swing_Up;
                        }
                    }
                    else
                    if (Pad.IsButtonPresed(GamePadStatus.Down))                 ///---------- DOWN ----------------
                    {
                        if (map.GetNextID(3, Direction.On) == 0)
                        {
                            WalkingDirection = Direction.Down;
                            buff = GamePadStatus.Down;
                            Gracz.Move(Direction.Down, PlayerMoveTexture);
                            map.MoveMap(0, WalkSpeed);

                            if (map.GetNextID(3, Direction.On) != 0)
                                map.MoveMap(0, -WalkSpeed);
                            swingdirection = SwingDirection.Swing_Down;
                        }
                    }
                    else
                    if (Pad.IsButtonPresed(GamePadStatus.Right))                ///---------- RIGHT ----------------
                    {

                        if (map.GetNextID(3, Direction.On) == 0)
                        {
                            WalkingDirection = Direction.Right;
                            buff = GamePadStatus.Right;
                            Gracz.Move(Direction.Right, PlayerMoveTexture);
                            map.MoveMap(WalkSpeed, 0);
                            if (map.GetNextID(3, Direction.On) != 0)
                                map.MoveMap(-WalkSpeed, 0);
                            swingdirection = SwingDirection.Swing_Right;

                        }
                    }
                    else
                    if (Pad.IsButtonPresed(GamePadStatus.Left))                 ///---------- LEFT ----------------
                    {

                        if (map.GetNextID(3, Direction.On) == 0)
                        {
                            WalkingDirection = Direction.Left;
                            buff = GamePadStatus.Left;
                            Gracz.Move(Direction.Left, PlayerMoveTexture);
                            map.MoveMap(-WalkSpeed, 0);
                            if (map.GetNextID(3, Direction.On) != 0)
                                map.MoveMap(WalkSpeed, 0);
                            swingdirection = SwingDirection.Swing_Left;
                        }
                    }
                }


                if (Pad.IsButtonClicked(GamePadStatus.A))                   ///---------- BUTTON A ----------------
                {


                    switch (swingdirection)                                 ///---------- Animacja Zbierania ----------------
                    {
                        case SwingDirection.Swing_Up:
                            PAC = true;
                            PACDirection = DirectionPAC.Pac_Up;
                            Gracz.PAC_PAC(DirectionPAC.Pac_Up, PlayerMoveTexture);

                            break;
                        case SwingDirection.Swing_Down:
                            PAC = true;
                            PACDirection = DirectionPAC.Pac_Down;
                            Gracz.PAC_PAC(DirectionPAC.Pac_Down, PlayerMoveTexture);
                            break;

                        case SwingDirection.Swing_Right:
                            PAC = true;
                            if (Gracz.Tools.Find(x => x.ToolName == "Axe").IsOwned == true)
                            {
                                PACDirection = DirectionPAC.Pac_R_Axe;
                                Gracz.PAC_PAC(DirectionPAC.Pac_R_Axe, PlayerMoveTexture);
                            }

                            else
                            {
                                PACDirection = DirectionPAC.Pac_Right;
                                Gracz.PAC_PAC(DirectionPAC.Pac_Right, PlayerMoveTexture);
                            }
                            break;

                        case SwingDirection.Swing_Left:
                            PAC = true;
                            if (Gracz.Tools.Find(x => x.ToolName == "Axe").IsOwned == true)
                            {
                                PACDirection = DirectionPAC.Pac_L_Axe;
                                Gracz.PAC_PAC(DirectionPAC.Pac_L_Axe, PlayerMoveTexture);
                            }

                            else
                            {
                                PACDirection = DirectionPAC.Pac_Left;
                                Gracz.PAC_PAC(DirectionPAC.Pac_Left, PlayerMoveTexture);
                            }
                            break;

                        default:
                            break;
                    }

                    /*if (kierun_Right == true)                              
                    {
                        PAC = true;
                        if (Gracz.Tools.Find(x => x.ToolName == "Axe").IsOwned == true)
                        {
                            PACDirection = DirectionPAC.Pac_R_Axe;
                            Gracz.PAC_PAC(DirectionPAC.Pac_R_Axe, PlayerMoveTexture);
                        }
                        else
                        {
                            PACDirection = DirectionPAC.Pac_Right;
                            Gracz.PAC_PAC(DirectionPAC.Pac_Right, PlayerMoveTexture);
                        }
                    }

                    if (kierun_Left == true)
                    {
                        PAC = true;
                        if (Gracz.Tools.Find(x => x.ToolName == "Axe").IsOwned == true)
                        {
                            PACDirection = DirectionPAC.Pac_L_Axe;
                            Gracz.PAC_PAC(DirectionPAC.Pac_L_Axe, PlayerMoveTexture);
                        }
                        else
                        {
                            PACDirection = DirectionPAC.Pac_Left;
                            Gracz.PAC_PAC(DirectionPAC.Pac_Left, PlayerMoveTexture);
                        }
                    }
                    */

                    ///-------------------------------------------------------------   WYKRYWANIE KOLIZJI   -------------------------//
                    if (map.GetNextID(3, WalkingDirection) == 6)
                    { // skrzyneczka
                        //String message = "Oooo skrzyneczka na pozycji " + map.GetNextCords(WalkingDirection).X + "x" + map.GetNextCords(WalkingDirection).Y;
                        //Gracz.Chests.Find(x, y => x.X == map.GetNextCords(WalkingDirection).X &&
                        //x.X == map.GetNextCords(WalkingDirection).Y);
                        //map.Message(message, DefaultFont, new Rectangle(50, 20, 600, 200));
                        //messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                    }


                    if (map.GetNextID(3, WalkingDirection) == 2)
                    { // Drewno

                        if (Gracz.Tools.Find(x => x.ToolName == "Axe").IsOwned == true)
                        {
                            //linq
                            Gracz.Materials.Wood += 2;
                            Gracz.Materials.Lianas += 2;
                            String message = "Zebrano drewno siekiera\r\n ilosc drewna: " + Gracz.Materials.Wood + "\r\n Ilosc lian:" + Gracz.Materials.Lianas;
                            //map.Message(message, DefaultFont, new Rectangle(50, 20, 400, 200));
                            messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                        }
                        else
                        {
                            Gracz.Materials.Wood++;
                            Gracz.Materials.Lianas++;
                            String message = "Zebrano drewno i liany\r\n ilosc drewna: " + Gracz.Materials.Wood + "\r\n Ilosc lian: " + Gracz.Materials.Lianas;
                            //map.Message(message, DefaultFont, new Rectangle(50, 20, 400, 200));
                            messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                        }

                        // Zmiana ID kafelek mapy
                        Vector2 TreeBase = map.ChangeID(WalkingDirection, 1, 31); // Zmiana podstawy drzewa na pieniek
                        map.ChangeID(WalkingDirection, 3, 1); // Zmiana ID podstawy na BLOKADA
                        map.ChangeID((int)TreeBase.X, (int)TreeBase.Y - 1, 2, 0); // Usuni�cie korony drzewa (1 wy�ej ni� podstawa)

                    }
                    else if (map.GetNextID(3, WalkingDirection) == 3)
                    { // Jerzynki
                        Gracz.Materials.Food++;
                        String message = "Zebrano jedzenie, ilosc jedzenia: " + Gracz.Materials.Food;
                        //map.Message(message, DefaultFont, new Rectangle(50, 20, 400, 100));
                        messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                        Vector2 TreeBase = map.ChangeID(WalkingDirection, 1, 2); // Zmiana krzaka je�ynkowego na zwyk�y
                        map.ChangeID(WalkingDirection, 3, 1); // Zmiana ID ...

                    }
                    else if (map.GetNextID(3, WalkingDirection) == 4)
                    { // Kamien
                        Gracz.Materials.Stone++;
                        String message = "Zebrano kamien, ilosc kamienia: " + Gracz.Materials.Stone;
                        //map.Message(message, DefaultFont, new Rectangle(50, 20, 400, 100));
                        messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                    }
                    else if (map.GetNextID(3, WalkingDirection) == 5)
                    { // Woda
                        Gracz.Materials.Water++;
                        String message = "Zebrano wode ilosc wody: " + Gracz.Materials.Water;
                        //map.Message(message, DefaultFont, new Rectangle(50, 20, 400, 100));
                        messages.AddMessage(message, new Rectangle(50, 20, 600, 200));
                    }
                    else
                    {

                    }
                }



                //-------------------------------------------- BUTTON B ---------------------------------------//

                if (LoadedGame == true)
                {
                    try
                    {
                        Gracz = Player.LoadPlayer();
                        Gracz.Texture = PlayerMoveTexture[4];
                        Gracz.Grass = GrassWalk.sound;

                        map = new Map(GraphicsDevice, "Map_Final_V2", ScreenX, ScreenY);
                        // map.SetPosition(Starting_posX, Starting_posY);

                        map.LoadMapFromXML();

                        //map = Map.LoadMapFromXML();
                        map.SetPosition(Gracz.PosX, Gracz.PosY);
                        LoadedGame = false;
                    }
                    catch (Exception ex)
                    {
                        messages.CreateIndependentMessage(ex.Message, new Rectangle(50, 20, 1000, 1000));
                        LoadedGame = false;
                    }
                }

                if (Pad.IsButtonClicked(GamePadStatus.B))
                {
                    //ChangeGameTime(GameHour + 1, 0);

                    string dairy = "";
                    foreach (string m in Gracz.Player_Dairy.dairy_notes)
                    {
                        dairy += m;
                    }
                    //map.Message(dairy, DefaultFont, new Rectangle(50, 20, 1000, 1000));
                    messages.CreateIndependentMessage(dairy, new Rectangle(50, 20, 1000, 1000));
                }

                //if (map.GetCurrentID(4) != 0)
                //{
                //    map.Message("Oooo misja :/  ID:" + map.GetMissionID(4), DefaultFont, new Rectangle(50, 20, 400, 100));
                //}
                //------questy-----------

                if (Gracz.ActiveGuest >= Gracz.Quests.Count)
                {

                    string message = "\r\n Brawo! Zebrales potrzebne materialy\r\n aby stworzyc schronienie i przetrwac\r\n nadchodzaca NOC \r\n \r\n Ukonczono fabule prologu!";
                    messages.CreateIndependentMessage(message, new Rectangle(50, 20, 1000, 1000));
                }
                else if (!Gracz.Quests[Gracz.ActiveGuest].Activated)
                {
                    //map.Message(Gracz.Quests [Gracz.ActiveGuest].Name, DefaultFont, new Rectangle(50, 20, 400, 100));
                    messages.CreateIndependentMessage(Gracz.Quests[Gracz.ActiveGuest].Name, new Rectangle(50, 20, 1000, 1000));

                    Gracz.Quests[Gracz.ActiveGuest].Activated = true;
                    Gracz.Player_Dairy.dairy_notes.Add(Gracz.Quests[Gracz.ActiveGuest].Quest_message);

                }
                else if (Gracz.Quests[Gracz.ActiveGuest].IsFinished(Gracz.Materials, Gracz.Tools))
                {

                    messages.CreateIndependentMessage("Ukonczono misje:\r\n" + Gracz.Quests[Gracz.ActiveGuest].Name, new Rectangle(50, 20, 1000, 1000));

                    Gracz.Player_Dairy.dairy_notes.Add("\r\nUkonczono misje : \r\n" + Gracz.Quests[Gracz.ActiveGuest].Name);
                    Gracz.ActiveGuest++;
                }
                messages.Update();
            }
            else
            {
                main.Update();
            }

            if (SongPlayed == 0)
            {
                GameBgAmbient.ChangeSong("TakeOver");
                SongPlayed = 100;
            }
            else if (SongPlayed == 1)
            { //  true false
                GameBgAmbient.ChangeSong("BgMusic");
                SongPlayed = 100;
            }
            else if(IsSoundPlaying== false)
            {
                GameBgAmbient.Stop();
                SongPlayed = 0;
            }
            base.Update(gameTime);

        }

        // ----------------------------------------------------------------------------------------------------- Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            spriteBatch.Begin();
            if (GameRunning == true)
            {
                // Rysowanie Pierwszych 2 warstw.
                map.Draw(spriteBatch, 0);
                map.Draw(spriteBatch, 1);


                // Rysowanie Gracza
                Gracz.Draw(spriteBatch, new Rectangle(
                    ((ScreenX / 2) - (map.GetZoomValue() / 2)),
                    ((ScreenY / 2)) - map.GetZoomValue() + 40,
                    map.GetZoomValue() - 20, map.GetZoomValue() - 20)
                    );

                // Rysowanie 3 Warstwy.
                map.Draw(spriteBatch, 2);

                messages.DrawMessages(spriteBatch);

                //  0 - 5 NOC ||  5 - 7 ZMIANA ||  7 - 17 DZIE� ||  17 - 19 ZMIANA ||  19 - 23 NOC
                // Dzien i noc
                if (GameHour >= 5 && GameHour <= 7) // ZMIANA W !!!!! DZIEN
                {
                    while (DayCycleTimer % 300 == 0)
                    {
                        if (DayCycle == 0)
                            break;
                        DayCycleTimer++;
                        DayCycle--;
                    }
                }
                else if (GameHour >= 17 && GameHour <= 19) // ZMIANA W !!!!! NOC
                {
                    while (DayCycleTimer % 300 == 0)
                    {
                        if (DayCycle == 24)
                            break;
                        DayCycleTimer++;
                        DayCycle++;
                    }
                }


                spriteBatch.Draw(DayCycleTexture[DayCycle], new Rectangle(0, 0, ScreenX, ScreenY), Color.White);      // Rysowanie Tekstury Nocy






                // Rysowanie Przyciskow i informacji pomocniczych.
                Pad.Draw(spriteBatch);



                // \/\/\/\/\/\/\/\/\/\/\/\/\/\/  Menu EKWIPUNEK 

                // rysuj po padzie :)  TO DO: Trzeba zablokowac Pada(poruszanie sie) na czas kiedy jest EQ otwarte,  /John

                InGameMenuManager.DrawInGameMenuButton(spriteBatch);

                //   Rectangle KnefelRect = new Rectangle((int)(ScreenX / 1.125), 50, 50, 50);   // Pozycja knefla do ingame menu **
                //    spriteBatch.Draw(Knefel_EQ, new Rectangle((int)(ScreenX / 1.125), 50, 50, 50), Color.White); // Rysowanie knefla do ingame menu **

                TouchCollection tl = TouchPanel.GetState();

                foreach (TouchLocation T in tl)
                {
                    if (InGameMenuManager.InGameMenuButtonPos.Contains(T.Position) && !Buff.Contains(T.Position))
                    {
                        ItemToCraftChosenManager = ItemToCraftChosen._None;
                        IsMenuButtonPressed = !IsMenuButtonPressed;
                        InGameMenuStateManager = InGameMenuState._Game;
                        DebugMode = !DebugMode;

                        Buff.X = (int)T.Position.X - 2;
                        Buff.Y = (int)T.Position.Y - 2;
                        Buff.Width = 4;
                        Buff.Height = 4;

                        break;
                    }
                }

                if (IsMenuButtonPressed)
                {
                    if (InGameMenuStateManager == InGameMenuState._Game) InGameMenuStateManager = InGameMenuState._Settings;
                    //   spriteBatch.Draw(Inventory, new Rectangle(100, 100, 800, 400), Color.White); // Rysowanie knefla do ingame menu **
                    //InGameMenuManager.DisposeUnusedButtons(InGameMenuStateManager);
                    InGameMenuManager.DrawInGameMenu(InGameMenuStateManager, spriteBatch);

                    TouchCollectionManager = TouchPanel.GetState();
                    foreach (TouchLocation TC in TouchCollectionManager)
                    {
                        // ------------------------------ D�wi�k -------------------------------------------
                        if (InGameMenuManager.SettingsButtonsPos[0].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Settings && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            IsSoundPlaying = !IsSoundPlaying;
                            if (IsSoundPlaying) InGameMenuManager.SettingsButtons[0] = Content.Load<Texture2D>("Buttons/Button_Checked");
                            else if (!IsSoundPlaying) InGameMenuManager.SettingsButtons[0] = Content.Load<Texture2D>("Buttons/Button_UnChecked");
                            //
                            
                            // TODO
                            //
                            //
                            //
                        }
                        // ------------------------------ Zapis gry -------------------------------------------
                        if (InGameMenuManager.SettingsButtonsPos[1].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Settings && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            Gracz.SavePlayer();
                            map.SaveMap();
                            //
                            //
                            //
                            //
                            //
                        }

                        // ------------------------------ Jedzenie i picie -------------------------------------------
                        if (InGameMenuStateManager == InGameMenuState._Inventory)
                        {
                            // ----------------------------- Jedzenie ------------------------------------------------
                            if (InGameMenuManager.InventoryIconsPos[3].Contains(TC.Position) && !Buff.Contains(TC.Position) && Gracz.Materials.Food > 0)
                            {
                                Buff.X = (int)TC.Position.X - 2;
                                Buff.Y = (int)TC.Position.Y - 2;
                                Buff.Width = 4;
                                Buff.Height = 4;
                                Gracz.GLOD = 100;
                                Gracz.HP = 100;
                                Gracz.Materials.Food--;
                            }
                            // ----------------------------- Picie ------------------------------------------------
                            if (InGameMenuManager.InventoryIconsPos[4].Contains(TC.Position) && !Buff.Contains(TC.Position) && Gracz.Materials.Water > 0)
                            {
                                Buff.X = (int)TC.Position.X - 2;
                                Buff.Y = (int)TC.Position.Y - 2;
                                Buff.Width = 4;
                                Buff.Height = 4;
                                Gracz.WODA = 100;
                                Gracz.Materials.Water--;
                            }
                        }
                        // ------------------------------ Crafting -------------------------------------------
                        if (InGameMenuManager.CraftingButtonsPos[2].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Crafting && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            // \|/ Tutaj mo�esz wdupi� crafting w zale�no�ci od wybranego przedmiotu
                            switch (ItemToCraftChosenManager)
                            {
                                case ItemToCraftChosen._Hammer:
                                    try
                                    {
                                        Gracz.Tools.Find(x => x.ToolName == "Hammer").Craft(Gracz.Materials,Gracz.Tools);
                                        InGameMenuManager.CraftingIcons[5] = Content.Load<Texture2D>("Icons/Icon_Allowed");
                                        InGameMenuManager.InventoryIcons[5] = Content.Load<Texture2D>("Icons/Icon_HammerUnlocked");
                                    }
                                    catch (Tool.CantCraftException ex)
                                    {
                                        messages.CreateIndependentMessage(ex.Message, new Rectangle(50, 20, 600, 200));
                                    }
                                    break;
                                case ItemToCraftChosen._Axe:
                                    try
                                    {
                                        Gracz.Tools.Find(x => x.ToolName == "Axe").Craft(Gracz.Materials, Gracz.Tools);
                                        InGameMenuManager.CraftingIcons[7] = Content.Load<Texture2D>("Icons/Icon_Allowed");
                                        InGameMenuManager.InventoryIcons[7] = Content.Load<Texture2D>("Icons/Icon_AxeUnlocked");
                                    }
                                    catch (Tool.CantCraftException ex)
                                    {
                                        messages.CreateIndependentMessage(ex.Message, new Rectangle(50, 20, 600, 200));
                                    }
                                    break;
                                case ItemToCraftChosen._Pickaxe:
                                    try
                                    {
                                        Gracz.Tools.Find(x => x.ToolName == "Pick").Craft(Gracz.Materials, Gracz.Tools);
                                        InGameMenuManager.CraftingIcons[6] = Content.Load<Texture2D>("Icons/Icon_Allowed");
                                        InGameMenuManager.InventoryIcons[6] = Content.Load<Texture2D>("Icons/Icon_PickaxeUnlocked");
                                    }
                                    catch (Tool.CantCraftException ex)
                                    {
                                        messages.CreateIndependentMessage(ex.Message, new Rectangle(50, 20, 600, 200));
                                    }
                                    break;
                                case ItemToCraftChosen._Saw:
                                    try
                                    {
                                        Gracz.Tools.Find(x => x.ToolName == "Saw").Craft(Gracz.Materials, Gracz.Tools);
                                        InGameMenuManager.CraftingIcons[8] = Content.Load<Texture2D>("Icons/Icon_Allowed");
                                        InGameMenuManager.InventoryIcons[8] = Content.Load<Texture2D>("Icons/Icon_SawUnlocked");
                                    }
                                    catch (Tool.CantCraftException ex)
                                    {
                                        messages.CreateIndependentMessage(ex.Message, new Rectangle(50, 20, 600, 200));
                                    }
                                    break;
                                case ItemToCraftChosen._Shelter:
                                    //
                                    //
                                    //
                                    //
                                    //
                                    break;
                            }
                        }

                        // ------------------------------ Wy�wietlanie craftowanych item�w -------------------------------------------
                        if (InGameMenuStateManager == InGameMenuState._Crafting)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (InGameMenuManager.CraftingIconsPos[i].Contains(TC.Position))
                                {
                                    ItemToCraftChosenManager = (ItemToCraftChosen)i;
                                }
                            }

                        }

                        // ------------------------------ Zmiana na ekwipunek ------------------------------
                        if (InGameMenuManager.SettingsButtonsPos[2].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Settings && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            ItemToCraftChosenManager = ItemToCraftChosen._None;
                            InGameMenuStateManager = InGameMenuState._Inventory;
                        }

                        // --------------------------------- Zmiana na ustawienia ------------------------------
                        else if (InGameMenuManager.InventoryButtonsPos[0].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Inventory && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            ItemToCraftChosenManager = ItemToCraftChosen._None;
                            InGameMenuStateManager = InGameMenuState._Settings;
                        }

                        // ----------------------------------- Zmiana na crafting ---------------------------------
                        else if (InGameMenuManager.InventoryButtonsPos[1].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Inventory && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            InGameMenuStateManager = InGameMenuState._Crafting;
                        }
                        // ----------------------------------- Zmiana na ekwipunek -----------------------------------
                        else if (InGameMenuManager.CraftingButtonsPos[0].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Crafting && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            ItemToCraftChosenManager = ItemToCraftChosen._None;
                            InGameMenuStateManager = InGameMenuState._Inventory;
                        }
                        // ------------------------------------ Zmiana na questy --------------------------------------
                        else if (InGameMenuManager.CraftingButtonsPos[1].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Crafting && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            ItemToCraftChosenManager = ItemToCraftChosen._None;
                            InGameMenuStateManager = InGameMenuState._Quests;
                        }
                        // ------------------------------------ Zmiana na crafting --------------------------------------
                        else if (InGameMenuManager.QuestsButtonsPos[0].Contains(TC.Position) && InGameMenuStateManager == InGameMenuState._Quests && !Buff.Contains(TC.Position))
                        {
                            Buff.X = (int)TC.Position.X - 2;
                            Buff.Y = (int)TC.Position.Y - 2;
                            Buff.Width = 4;
                            Buff.Height = 4;
                            InGameMenuStateManager = InGameMenuState._Crafting;
                        }
                    }
                    // Musia�em to tutaj da�, bo jakbym chcia� to wypisywa� w klasie to trza by by�o pozmienia� klauzury dost�pno�ci w playerze - Juan
                    if (InGameMenuStateManager == InGameMenuState._Inventory)
                    {
                        spriteBatch.DrawString(DefaultFont, ":" + Gracz.Materials.Wood, new Vector2(150, 160), Color.Black);
                        spriteBatch.DrawString(DefaultFont, ":" + Gracz.Materials.Stone, new Vector2(150, 210), Color.Black);
                        spriteBatch.DrawString(DefaultFont, ":" + Gracz.Materials.Lianas, new Vector2(150, 260), Color.Black);
                        spriteBatch.DrawString(DefaultFont, ":" + Gracz.Materials.Food, new Vector2(150, 310), Color.Black);
                        spriteBatch.DrawString(DefaultFont, ":" + Gracz.Materials.Water, new Vector2(150, 360), Color.Black);
                    }
                    if (InGameMenuStateManager == InGameMenuState._Settings)
                    {
                        spriteBatch.DrawString(DefaultFont, SoundText, new Vector2(150, 200), Color.Black);
                        spriteBatch.DrawString(DefaultFont, SaveText, new Vector2(150, 300), Color.Black);
                    }

                    InGameMenuManager.DrawItemToCraft(spriteBatch, ItemToCraftChosenManager);

                    InGameMenuManager.DrawInGameMenuButton(spriteBatch);

                    // Wy�wietlanie nazw quest�w
                    if (InGameMenuStateManager == InGameMenuState._Quests) InGameMenuManager.DrawQuestsNames(spriteBatch, DefaultFont, Gracz.Quests);
                    if (InGameMenuStateManager == InGameMenuState._Quests) InGameMenuManager.DrawQuestDairy(spriteBatch, DefaultFont, Gracz.Player_Dairy, Gracz.Player_Dairy.dairy_notes.Count - 1);

                }


                // /\/\/\/\/\/\/\/\/\/\/\/\/\



                if (DebugMode)
                {
                    //spriteBatch.DrawString(DefaultFont, "X: " + map.GetPosition().X, new Vector2(50, 50), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Y: " + map.GetPosition().Y, new Vector2(50, 100), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Dir: " + WalkingDirection.ToString(), new Vector2(50, 150), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Square size: " + map.GetZoomValue(), new Vector2(50, 200), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Game time: " + GameHour + ":" + GameMinute, new Vector2(50, 250), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Resolution: " + ScreenX + "x" + ScreenY, new Vector2(50, 300), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Square size: " + map.GetZoomValue(), new Vector2(50, 350), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Player Pos: " + (((ScreenX / 2) - (map.GetZoomValue() / 2))) + "x" + (((ScreenY / 2)) - map.GetZoomValue()), new Vector2(50, 400), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "Player Pos: " + (((ScreenX / 2) - (map.GetZoomValue() / 2))) / 18 + "x" + (((ScreenY / 2)) - map.GetZoomValue()) / 11, new Vector2(50, 450), Color.Red);
                    //spriteBatch.DrawString(DefaultFont, "DayCycleTimer: " + DayCycleTimer + " ID:" + DayCycle, new Vector2(50, 500), Color.Red); // DayCycle TEST PacTimer
                    //spriteBatch.DrawString(DefaultFont, "PacTimer: " + PacTimer, new Vector2(50, 550), Color.Red);

                    spriteBatch.DrawString(DefaultFont, "Zdrowie: " + Gracz.HP, new Vector2(100, 50), Color.LightGreen);
                    spriteBatch.DrawString(DefaultFont, "Woda: " + Gracz.WODA, new Vector2(100, 100), Color.LightGreen);
                    spriteBatch.DrawString(DefaultFont, "Jedzenie: " + Gracz.GLOD, new Vector2(100, 150), Color.LightGreen);
                    spriteBatch.DrawString(DefaultFont, "Strach: " + Gracz.STRACH, new Vector2(100, 200), Color.LightGreen);
                }


                // Timer Cyklu Dnia
                DayCycleTimer++;
                if (DayCycleTimer >= 100000) { DayCycleTimer = 0; }



            }
            else
                main.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void GameTimeControl()
        {
            GameMinute++;
            if (GameMinute >= 60)
            {
                GameHour++;
                if (GameHour >= 24)
                {
                    GameHour = 0;
                }
                GameMinute = 0;
            }
        }


        // ---------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------- Jakie� co� ....
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            back = Content.Load<Texture2D>("Menu\\M_BACK");
            main.LoadContent(Content);
        }

        void ChangeGameTime(int H, int M)
        {
            GameHour = H;
            GameMinute = M;
            DayCycleTimer = 0;
            if (GameHour >= 5 && GameHour <= 7)
                DayCycle = 24 - ((120 - (((7 - GameHour + 1)) * 60) + ((60 - GameMinute))) / 5);
            else if (GameHour >= 17 && GameHour <= 19)
                DayCycle = ((120 - (((19 - GameHour + 1)) * 60) + ((60 - GameMinute))) / 5);
        }

    }
}

