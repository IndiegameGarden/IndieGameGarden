using System;

using TTengine.Core;

namespace IndiegameGarden.Menus
{
    public class TimedInvisibility: Gamelet
    {
        float time = 0f;

        public TimedInvisibility(float time)
        {
            this.time = time;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            if (this.SimTime >= time)
            {
                Parent.Visible = false;
                Delete = true;
            }
        }
    }
}
