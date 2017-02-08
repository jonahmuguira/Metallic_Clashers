// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchInformation.cs" company="Metallic Clashers">
//   CLash On
// </copyright>
// <summary>
//   Defines the TypeChangeInformation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Board.Information
{
    using System.Collections.Generic;

    using UnityEngine;

    public class MatchInformation
    {
        public GemType type;

        public List<GemMono> gemMonos;

        public List<GridCollection> gridCollections;
    }
}
