namespace StageSelection
{
    using System.Collections.Generic;

    using UnityEngine;

    public class StageSelectionManager : SubManager<StageSelectionManager>
    {
        public List<Tree> worlds = new List<Tree>();
        protected override void Init()  //Awake for the Manager
        {
            
        }
    }
}
