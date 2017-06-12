using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PadControler {

    // Lista dost�pnych przycisk�w
    public enum GamePadStatus {
        Up, Down, Right, Left, A, B, ZoomOut, ZoomIn,
        IdleUp, IdleDown, IdleRight, IdleLeft, DirNone, None
    }

    // Klasa obs�uguj�ca przyciski
    class GameControler {
        List<PadButton> GameButtons; // Lista przycisk�w

        // Konstruktor
        public GameControler(GraphicsDevice graphicsDevice, int ScreenWidth, int ScreenHeight) {
            int BtnSize = ScreenWidth / 17;
            GameButtons = new List<PadButton>();
            GameButtons.Add(new PadButton(GamePadStatus.Up, graphicsDevice, "Content/UI/Pad/UP.png", new Rectangle((2 * BtnSize), ScreenHeight - (4 * BtnSize), BtnSize + (BtnSize / 2), (int)(BtnSize * 1.5))));
            GameButtons.Add(new PadButton(GamePadStatus.Down, graphicsDevice, "Content/UI/Pad/DOWN.png", new Rectangle((2 * BtnSize), ScreenHeight - (2 * BtnSize), (int)(BtnSize * 1.5), (int)(BtnSize * 1.5))));
            GameButtons.Add(new PadButton(GamePadStatus.Right, graphicsDevice, "Content/UI/Pad/RIGHT.png", new Rectangle((3 * BtnSize), ScreenHeight - (3 * BtnSize), (int)(BtnSize * 1.5), (int)(BtnSize * 1.5))));
            GameButtons.Add(new PadButton(GamePadStatus.Left, graphicsDevice, "Content/UI/Pad/LEFT.png", new Rectangle(BtnSize, ScreenHeight - (3 * BtnSize), (int)(BtnSize * 1.5), (int)(BtnSize * 1.5))));

            GameButtons.Add(new PadButton(GamePadStatus.A, graphicsDevice, "Content/UI/Pad/A.png", new Rectangle(ScreenWidth - (2 * BtnSize), ScreenHeight - (3 * BtnSize), (int)(BtnSize * 1.5), (int)(BtnSize * 1.5))));
            GameButtons.Add(new PadButton(GamePadStatus.B, graphicsDevice, "Content/UI/Pad/B.png", new Rectangle(ScreenWidth - (4 * BtnSize), ScreenHeight - (2 * BtnSize), (int)(BtnSize * 1.5), (int)(BtnSize * 1.5))));
        }

        // Sprawdzenie stanu przycisk�w
        public List<GamePadStatus> GamePadState() {
            List<GamePadStatus> ToReturn = new List<GamePadStatus>();

            TouchCollection tl = TouchPanel.GetState();
            
            foreach (TouchLocation T in tl) {
                foreach (PadButton P in GameButtons) {
                    if (P.Position.Contains(T.Position)) {
                        ToReturn.Add(P.ButonType);
                    }
                }
            }

            if (tl.Count == 0 && ToReturn.Count == 0) { // #TEST Jak co� by�o ||
                ToReturn.Add(GamePadStatus.DirNone);
                ToReturn.Add(GamePadStatus.None);
            }
            return ToReturn;
        }

        // Sprawdzenie czy zadany przycisk jest wci�niety
        public bool IsButtonPresed(GamePadStatus Button) {
            if (GamePadState().Contains(Button))
                return true;
            return false;
        }

        // Sprawdzanie czy przycisk jest klikni�ty
        public bool IsButtonClicked(GamePadStatus Button) {
            PadButton P = GameButtons [(int)Button];
            if (!IsButtonPresed(P.ButonType)) {
                P.Pressed = false;
            }

            if (IsButtonPresed(P.ButonType) && !P.Pressed) {
                P.Pressed = true;
                return true;
            }
            return false;
        }

        // Rysowanie przycisk�w
        public void Draw(SpriteBatch spriteBatch) {
            foreach (PadButton P in GameButtons) {
                spriteBatch.Draw(P.Bitmap, P.Position, Color.White);
            }
        }
    }
}