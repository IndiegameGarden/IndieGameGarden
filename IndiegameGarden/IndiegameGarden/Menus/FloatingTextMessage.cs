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

        public FloatingTextMessage()
            : base("GameDescriptionFont")
        {
            ColorB = new ColorChangeBehavior();
            Add(ColorB);
            ColorB.AlphaTarget = 0.0f;
            ColorB.Alpha = 0f;
            ColorB.FadeSpeed = 0.3f;
        }

        public void FadeIn()
        {
            ColorB.AlphaTarget = 1.0f;
        }

        public void FadeOut()
        {
            ColorB.AlphaTarget = 0f;
        }

    }
}
