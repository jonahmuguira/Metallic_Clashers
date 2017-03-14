// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchInformation.cs" company="Metallic Clashers">
//   CLash On
// </copyright>
// <summary>
//   Defines the TypeChangeInformation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Combat.Board.Information
{
    using System.Collections.Generic;

    public class MatchInformation
    {
        public GemType type;

        public List<Gem> gems = new List<Gem>();

        public List<GridCollection> gridCollections = new List<GridCollection>();
    }
}
