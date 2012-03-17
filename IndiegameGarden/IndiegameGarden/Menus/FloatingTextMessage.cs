using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndiegameGarden.Menus
{
    /// <summary>
    /// a help message/text that pops up on screen
    /// </summary>
    public class FloatingTextMessage: GameTextBox
    {
        ColorChangeBehavior ColorB;

        public FloatingTextMessage()
            : base("GameDescriptionFont")
        {
            ColorB = new ColorChangeBehavior();
            Add(ColorB);
            ColorB.FadeTarget = 0.0f;
            ColorB.Intensity = 0f;
            ColorB.FadeSpeed = 0.3f;
        }

        public void FadeIn()
        {
            ColorB.FadeTarget = 1.0f;
        }

        public void FadeOut()
        {
            ColorB.FadeTarget = 0f;
        }

    }
}
